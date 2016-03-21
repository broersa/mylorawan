using System;
using Newtonsoft.Json;

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


        #endregion


    }
}

