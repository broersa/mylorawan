using System;
using Newtonsoft.Json;
using System.Text;

namespace Com.Bekijkhet.Semtech
{
    public class SemtechImpl : ISemtech
    {
        #region ISemtech implementation

        public Identifier GetIdentifier (byte[] packet)
        {
            switch (packet [3]) {
            case 0x00:
                return Identifier.PUSH_DATA;
            case 0x01:
                return Identifier.PUSH_ACK;
            case 0x02:
                return Identifier.PULL_DATA;
            case 0x03:
                return Identifier.PULL_RESP;
            case 0x04:
                return Identifier.PULL_ACK;
            }
            throw new InvalidIdentifierException ();
        }

        public PushData UnmarshalPushData(byte[] packet)
        {
            var returnvalue = new PushData();
            returnvalue.ProtocolVersion = packet[0];
            returnvalue.RandomToken = new byte[2];
            Buffer.BlockCopy(packet, 1, returnvalue.RandomToken, 0, 2);
            returnvalue.Identifier = GetIdentifier(packet);
            returnvalue.GatewayMACAddress = new byte[8];
            Buffer.BlockCopy(packet, 4, returnvalue.GatewayMACAddress, 0, 8);
            var json = new byte[packet.Length - 12];
            Buffer.BlockCopy(packet, 12, json, 0, packet.Length - 12);
            returnvalue.Json = JsonConvert.DeserializeObject<PushDataJson>(System.Text.Encoding.Default.GetString(json));
            return returnvalue;
        }

        public PullData UnmarshalPullData(byte[] packet)
        {
            var returnvalue = new PullData();
            returnvalue.ProtocolVersion = packet[0];
            returnvalue.RandomToken = new byte[2];
            Buffer.BlockCopy(packet, 1, returnvalue.RandomToken, 0, 2);
            returnvalue.Identifier = GetIdentifier(packet);
            returnvalue.GatewayMACAddress = new byte[8];
            Buffer.BlockCopy(packet, 4, returnvalue.GatewayMACAddress, 0, 8);
            return returnvalue;
        }

        public byte[] MarshalPushAck(byte[] randomtoken)
        {
            return new byte[4] { 0x01, randomtoken[0], randomtoken[1], (byte)Identifier.PUSH_ACK };
        }


        public byte[] MarshalPullAck(byte[] randomtoken)
        {
            return new byte[4] { 0x01, randomtoken[0], randomtoken[1], (byte)Identifier.PULL_ACK };
        }

        public byte[] MarshalPullResp(PullResp pullresp)
        {
            var json = JsonConvert.SerializeObject(pullresp.Txpk);
            var bytes = StringToByteArray(json);
            var returnvalue = new byte[4 + bytes.Length];
            Buffer.SetByte(returnvalue, 0, pullresp.ProtocolVersion);
            Buffer.SetByte(returnvalue, 3, (byte)pullresp.Identifier);
            Buffer.BlockCopy(bytes, 0, returnvalue, 4, bytes.Length);
            return returnvalue;
        }

        #endregion

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}

