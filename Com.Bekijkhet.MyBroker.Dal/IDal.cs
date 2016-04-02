using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Bekijkhet.MyBroker.Dal
{
    public interface IDal
    {
        Task BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        void Close();

        Task<long> AddApplication(Application application);
		Task<Application> GetApplication(long id);
		Task<Application> GetApplicationOnName(string name);
		Task<Application> GetApplicationOnAppEUI(string appeui);

		Task<long> AddDevice(Device device);
		Task<Device> GetDevice(long id);
		Task<Device> GetDeviceOnDevEUI(string deveui);
		Task<Device> GetDeviceOnAppEUIDevEUI(string appeui, string deveui);

		Task<long> AddSession(Session session);
		Task<Session> GetSession(long id);
		Task<Session> GetSessionOnDeviceActive(long device);
		Task<Session> GetSessionOnDeviceDevNonceActive(long device, string devnonce);
        Task<Session> GetSessionOnNwkIdNwkAddrActive(long netid, long nwkaddr);

        Task<DevAddr> GetFreeDevAddr(long netid);
		Task SetActiveSessionsInactive(long device);

    }
}

