using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace Com.Bekijkhet.Lora
{
    public class LoraImpl : ILora
    {
        public LoraImpl()
        {
        }

        #region ILora implementation

        public MType GetMType(byte mhdr)
        {
            switch (mhdr >> 5) {
            case 0:
                return MType.JoinRequest;
            case 1:
                return MType.JoinAccept;
            case 2:
                return MType.UnconfirmedDataUp;
            case 3:
                return MType.UnconfirmedDataDown;
            case 4:
                return MType.ConfirmedDataUp;
            case 5:
                return MType.ConfirmedDataDown;
            }
            throw new InvalidMTypeException();
        }

        public JoinRequest UnmarshalJoinRequest(byte[] message)
        {
            var returnvalue = new JoinRequest();
            returnvalue.Mhdr = UnmarshalMhdr(message[0]);
            returnvalue.AppEUI = new byte[8];
            Buffer.BlockCopy(message, 1, returnvalue.AppEUI, 0, 8);
            returnvalue.DevEUI = new byte[8];
            Buffer.BlockCopy(message, 9, returnvalue.DevEUI, 0, 8);
            returnvalue.DevNonce = new byte[2];
            Buffer.BlockCopy(message, 17, returnvalue.DevNonce, 0, 2);
            returnvalue.Mic = new byte[4];
            Buffer.BlockCopy(message, 19, returnvalue.Mic, 0, 4); 
            return returnvalue;
        }

        public JoinRequest UnmarshalJoinRequestAndValidate(byte[] appkey, byte[] message)
        {
            var returnvalue = UnmarshalJoinRequest(message);

            var micpart = new byte[19];
            Buffer.BlockCopy(message, 0, micpart, 0, 19);

            var mic = AESCMAC(appkey, micpart);

            if (!(returnvalue.Mic[0] == mic[0] &&
                  returnvalue.Mic[1] == mic[1] &&
                  returnvalue.Mic[2] == mic[2] &&
                  returnvalue.Mic[3] == mic[3]))
            {
                throw new InvalidMICException();
            }
            return returnvalue;
        }

        public byte[] GetAppNonce() 
        {
            Random rnd = new Random();
            Byte[] b = new Byte[3];
            rnd.NextBytes(b);
            return b;
        }

        public byte[] MarshalJoinAccept(JoinAccept joinaccept, byte[] appkey)
        {
            // for now we skip the optional cflist
            var buffer = new byte[12];
            Buffer.BlockCopy(joinaccept.AppNonce, 0, buffer, 0, 3);
            Buffer.BlockCopy(joinaccept.NetId, 0, buffer, 3, 3);
            Buffer.BlockCopy(BitConverter.GetBytes(joinaccept.DevAddr), 0, buffer, 6, 4);
            Buffer.SetByte(buffer, 10, joinaccept.DlSettings);
            Buffer.SetByte(buffer, 11, joinaccept.RxDelay);

            var micdata = new byte[13];
            Buffer.SetByte(micdata, 0, MarshalMhdr(joinaccept.Mhdr));
            Buffer.BlockCopy(buffer, 0, micdata, 1, 12);

            var mic = AESCMAC(appkey, micdata);

            var ja = new byte[16];
            Buffer.BlockCopy(buffer, 0, ja, 0, 12);
            Buffer.BlockCopy(mic, 0, ja, 12, 4);

            var crypted = Decrypt(ja, appkey);

            var returnmessage = new byte[1 + crypted.Length + 4];
            Buffer.SetByte(returnmessage, 0, MarshalMhdr(joinaccept.Mhdr));
            Buffer.BlockCopy(crypted, 0, returnmessage, 1, crypted.Length);
            Buffer.BlockCopy(mic, 0, returnmessage, 1+crypted.Length, 4);

            return returnmessage;
        }

        private static byte[] Encrypt(byte[] data, byte[] key) {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB; // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length);
        }

        private static byte[] Decrypt(byte[] data, byte[] key) {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB; // http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
            rDel.Padding = PaddingMode.None;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length);
        }

        public byte[] GetNwkSKey(byte[] appkey, byte[] appnonce, byte[] netid, byte[] devnonce)
        {
            var returnvalue = new byte[16];
            var data = new byte[16];
            data[0]=0x01;
            Buffer.BlockCopy(appnonce, 0, data, 1, 3);
            Buffer.BlockCopy(netid, 0, data, 4, 3);
            Buffer.BlockCopy(devnonce, 0, data, 7, 2);

            Buffer.BlockCopy(Encrypt(data, appkey), 0, returnvalue, 0, 16);
            return returnvalue;
        }

        public byte[] GetAppSKey(byte[] appkey, byte[] appnonce, byte[] netid, byte[] devnonce)
        {
            var returnvalue = new byte[16];
            var data = new byte[16];
            data[0]=0x02;
            Buffer.BlockCopy(appnonce, 0, data, 1, 3);
            Buffer.BlockCopy(netid, 0, data, 4, 3);
            Buffer.BlockCopy(devnonce, 0, data, 7, 2);

            Buffer.BlockCopy(Encrypt(data, appkey), 0, returnvalue, 0, 16);
            return returnvalue;
        }

        public UnconfirmedDataUp UnmarshalUnconfirmedDataUp(byte[] message)
        {
            var returnvalue = new UnconfirmedDataUp();
            returnvalue.Mhdr = UnmarshalMhdr(message[0]);

            var fctrl = UnmarshalFCtrlUplink(message[5]);

            var fhdr = new byte[7 + fctrl.FOptsLen];
            Buffer.BlockCopy(message, 1, fhdr, 0, 7 + fctrl.FOptsLen);
            returnvalue.Fhdr = UnmarshalFhdrUplink(fhdr, fctrl);
            if (message.Length - 1/*mhdr*/ - 7/*fhdr*/ - fctrl.FOptsLen - 4/*mic*/ > 0) {
                returnvalue.FPort = message[1 + 7 + fctrl.FOptsLen];
                var frmpayloadsize = message.Length - 1/*mhdr*/ - 7/*fhdr*/ - fctrl.FOptsLen - 1/*fport*/ - 4/*mic*/;
                returnvalue.FRMPayload = new byte[frmpayloadsize];
                Buffer.BlockCopy(message, 1 + 7 + fctrl.FOptsLen + 1, returnvalue.FRMPayload, 0, frmpayloadsize);
            }
            returnvalue.Mic = new byte[4];
            Buffer.BlockCopy(message, message.Length - 4, returnvalue.Mic, 0, 4);
            return returnvalue;
        }

        public UnconfirmedDataUp UnmarshalUnconfirmedDataUpAndValidate(byte[] nwkskey, byte[] message)
        {
            var returnvalue = UnmarshalUnconfirmedDataUp(message);

            var data = new byte[16 + message.Length-4];
            data[0] = 0x49;
            data[5] = 0x00;
            Buffer.BlockCopy(MarshalDevAddr(returnvalue.Fhdr.DevAddr), 0, data, 6, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt32(returnvalue.Fhdr.FCnt)), 0, data, 10, 4);
            data[15] = (byte)(message.Length-4);

            Buffer.BlockCopy(message, 0, data, 16, message.Length-4);

            var mic = AESCMAC(nwkskey, data);

            if (!(returnvalue.Mic[0] == mic[0] &&
                returnvalue.Mic[1] == mic[1] &&
                returnvalue.Mic[2] == mic[2] &&
                returnvalue.Mic[3] == mic[3]))
            {
                throw new InvalidMICException();
            }

            return returnvalue;
        }

        public byte[] DecryptFRMPayload(byte[] key, UnconfirmedDataUp data)
        {
            var returnvalue = new byte[data.FRMPayload.Length];
            int j = 0;
            byte i = 1;
            while (j < data.FRMPayload.Length) {
                var partsize = data.FRMPayload.Length - (j * 16) >= 16 ? 16 : data.FRMPayload.Length - (j * 16);
                var part = new byte[partsize];
                Buffer.BlockCopy(data.FRMPayload, j, part, 0, partsize);

                var buf = new byte[16];
                buf[0] = 0x01;
                buf[5] = 0x00;
                Buffer.BlockCopy(MarshalDevAddr(data.Fhdr.DevAddr), 0, buf, 6, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(Convert.ToUInt32(data.Fhdr.FCnt)), 0, buf, 10, 4);
                buf[15] = i;

                var key2 = Encrypt(buf, key);
                for (int x = 0; x < partsize; x++) {
                    returnvalue[(j * 16) + x] = (byte)(part[x] ^ key2[x]);
                }

                j += 16;
            }
            return returnvalue;
        }


        #endregion

        private Mhdr UnmarshalMhdr(byte mhdr)
        {
            return new Mhdr() {
                MType = GetMType(mhdr),
                Major = (byte)(mhdr & 0x03)
            };
        }

        private byte MarshalMhdr(Mhdr mhdr)
        {
            return (byte)((((byte)mhdr.MType) << ((byte)5)) + mhdr.Major);
        }

        private FCtrlUplink UnmarshalFCtrlUplink(byte fctrl)
        {
            return new FCtrlUplink() {
                ADR = (fctrl & 128) == 128,
                ADRACKReq = (fctrl & 64) == 64,
                ACK = (fctrl & 32) == 32,
                FOptsLen = (byte)(fctrl & (8 + 4 + 2 + 1))
            };
        }
         
        private FhdrUplink UnmarshalFhdrUplink(byte[] fhdr, FCtrlUplink fctrl)
        {
            var returnvalue = new FhdrUplink();
            var devaddr = new byte[4];
            Buffer.BlockCopy(fhdr, 0, devaddr, 0, 4);
            returnvalue.DevAddr = UnmarshalDevAddr(devaddr);
            returnvalue.FCtrl = fctrl;
            returnvalue.FCnt = BitConverter.ToUInt16(fhdr, 5);
            if (fctrl.FOptsLen > 0) {
                returnvalue.FOpts = new byte[fctrl.FOptsLen];
                Buffer.BlockCopy(fhdr, 7, returnvalue.FOpts, 0, fctrl.FOptsLen);
            }
            return returnvalue;
        }

        private DevAddr UnmarshalDevAddr(byte[] devaddr)
        {
            var returnvalue = new DevAddr();
            returnvalue.NwkId = (byte)(devaddr[0] >> 1);
            devaddr[0] = (byte)(devaddr[0] & 1);
            returnvalue.NwkAddr = BitConverter.ToUInt32(devaddr, 0);
            return returnvalue;
        }

        public byte[] MarshalDevAddr(DevAddr devaddr)
        {
            return BitConverter.GetBytes(Convert.ToUInt32((devaddr.NwkId * 16777216) + devaddr.NwkAddr));
        }


        private byte[] AESEncrypt(byte[] key, byte[] iv, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }
            }
        }

        private byte[] Rol(byte[] b)
        {
            byte[] r = new byte[b.Length];
            byte carry = 0;

            for (int i = b.Length - 1; i >= 0; i--)
            {
                ushort u = (ushort)(b[i] << 1);
                r[i] = (byte)((u & 0xff) + carry);
                carry = (byte)((u & 0xff00) >> 8);
            }

            return r;
        }

        private byte[] AESCMAC(byte[] key, byte[] data)
        {
            // SubKey generation
            // step 1, AES-128 with key K is applied to an all-zero input block.
            byte[] L = AESEncrypt(key, new byte[16], new byte[16]);

            // step 2, K1 is derived through the following operation:
            byte[] FirstSubkey = Rol(L); //If the most significant bit of L is equal to 0, K1 is the left-shift of L by 1 bit.
            if ((L[0] & 0x80) == 0x80)
                FirstSubkey[15] ^= 0x87; // Otherwise, K1 is the exclusive-OR of const_Rb and the left-shift of L by 1 bit.

            // step 3, K2 is derived through the following operation:
            byte[] SecondSubkey = Rol(FirstSubkey); // If the most significant bit of K1 is equal to 0, K2 is the left-shift of K1 by 1 bit.
            if ((FirstSubkey[0] & 0x80) == 0x80)
                SecondSubkey[15] ^= 0x87; // Otherwise, K2 is the exclusive-OR of const_Rb and the left-shift of K1 by 1 bit.

            // MAC computing
            if (((data.Length != 0) && (data.Length % 16 == 0)) == true)
            {
                // If the size of the input message block is equal to a positive multiple of the block size (namely, 128 bits),
                // the last block shall be exclusive-OR'ed with K1 before processing
                for (int j = 0; j < FirstSubkey.Length; j++)
                    data[data.Length - 16 + j] ^= FirstSubkey[j];
            }
            else
            {
                // Otherwise, the last block shall be padded with 10^i
                byte[] padding = new byte[16 - data.Length % 16];
                padding[0] = 0x80;

                data = data.Concat<byte>(padding.AsEnumerable()).ToArray();

                // and exclusive-OR'ed with K2
                for (int j = 0; j < SecondSubkey.Length; j++)
                    data[data.Length - 16 + j] ^= SecondSubkey[j];
            }

            // The result of the previous process will be the input of the last encryption.
            byte[] encResult = AESEncrypt(key, new byte[16], data);

            byte[] HashValue = new byte[16];
            Array.Copy(encResult, encResult.Length - HashValue.Length, HashValue, 0, HashValue.Length);

            return HashValue;
        }


    }
}

