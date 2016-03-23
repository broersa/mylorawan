﻿using System;
using Com.Bekijkhet.MyBroker.Bll;
using System.Threading.Tasks;
using Com.Bekijkhet.Lora;
using System.Text;

namespace Com.Bekijkhet.MyBroker.BllImpl
{
    public class Bll : IBll
    {
        private Com.Bekijkhet.MyBroker.Dal.IDal _dal;
        private ILora _lora;

        public Bll(Com.Bekijkhet.MyBroker.Dal.IDal dal, ILora lora)
        {
            _dal = dal;
            _lora = lora;
        }

        #region IBll implementation

        public async Task<byte[]> ProcessJoinRequest(byte[] data)
        {
            byte[] returnvalue;
            try {
                await _dal.BeginTransaction();
                var joinrequest = _lora.UnmarshalJoinRequest(data);
                var device = await _dal.GetDeviceOnAppEUIDevEUI(ByteArrayToString(joinrequest.AppEUI), ByteArrayToString(joinrequest.DevEUI));
                var appkey = StringToByteArray(device.AppKey);
                var joinrequestvalidated = _lora.UnmarshalJoinRequestAndValidate(appkey, data);
                var devnonce = ByteArrayToString(joinrequestvalidated.DevNonce);
                if ((await _dal.GetSessionOnDeviceDevNonceActive(device.Id, devnonce))==null)
                {
                    throw new SessionAllreadyActiveException();
                }
                var freenwkaddr = await _dal.GetFreeNwkAddr();
                var appnonce = _lora.GetAppNonce();
                await _dal.SetActiveSessionsInactive(device.Id);
                await _dal.AddSession(new Com.Bekijkhet.MyBroker.Dal.Session() {
                    Device = device.Id,
                    NwkAddr = freenwkaddr.NetworkAddress,
                    DevNonce = devnonce,
                    AppNonce = ByteArrayToString(appnonce),
                    NwkSKey = "",
                    AppSKey = "",
                    Active = DateTime.UtcNow
                });
                var joinaccept = new JoinAccept() {
                    Mhdr = new Mhdr() { MType = MType.JoinAccept, Major=1},
                    AppNonce = appnonce,
                    NetId = GetNetId(),
                    DevAddr = Convert.ToUInt32((GetNwkId()*16777216)+freenwkaddr.NetworkAddress),
                    DlSettings = GetDlSettings(),
                    RxDelay = GetRxDelay(),
                    CfList = GetCfList()
                };

                returnvalue = _lora.MarshalJoinAccept(joinaccept, appkey);
                        

                _dal.CommitTransaction();
            }
            catch (Exception e) {
                _dal.RollbackTransaction();
                throw;
            }
            finally {
                _dal.Close();
            }
            return returnvalue;
        }

        #endregion
        
        private static byte[] GetNetId() {
            return new byte[3];
        }

        private static byte GetNwkId() {
            return 0;
        }

        private static byte GetDlSettings() {
            return 0 + 0 + 7; // 7 = max datarate?
        }

        private static byte GetRxDelay() {
            return 0;
        }

        private static byte[] GetCfList() {
            return new byte[16];
        }

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

