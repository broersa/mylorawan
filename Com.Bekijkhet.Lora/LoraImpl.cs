using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

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
            return returnvalue;
        }

        public JoinRequest UnmarshalJoinRequestAndValidate(byte[] appkey, byte[] message)
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

            var mic = AESCMAC(appkey, message);

            if (!(returnvalue.Mic[0] == mic[0] &&
                  returnvalue.Mic[1] == mic[1] &&
                  returnvalue.Mic[2] == mic[2] &&
                  returnvalue.Mic[3] == mic[3]))
            {
                throw new InvalidMICException();
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

