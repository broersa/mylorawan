using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Bekijkhet.MyBroker.Dal
{
    public interface IDal
    {
        void BeginTransaction();

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

		Task<NwkAddr> GetFreeNwkAddr();
		Task SetActiveSessionsInactive(long device);

/*
		AddDevice(device *Device) (int64, error)
		GetDevice(id int64) (*Device, error)
		GetDeviceOnDevEUI(deveui string) (*Device, error)
		GetDeviceOnAppEUIDevEUI(appeui string, deveui string) (*Device, error)
		AddSession(session *Session) (int64, error)
		GetSessionOnID(id int64) (*Session, error)
		GetSessionOnDeviceActive(device int64) (*Session, error)
		GetSessionOnDeviceDevNonceActive(device int64, devnonce string) (*Session, error)
		GetFreeNwkAddr() (*NwkAddr, error)
		SetActiveSessionsInactive(device int64) error
*/
    }
}

