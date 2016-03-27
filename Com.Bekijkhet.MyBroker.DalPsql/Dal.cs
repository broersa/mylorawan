using System;
using Npgsql;
using System.Threading.Tasks;
using Com.Bekijkhet.MyBroker.Dal;
using System.Collections.Generic;

namespace Com.Bekijkhet.MyBroker.DalPsql
{
    public class Dal : IDal
    {
        private NpgsqlConnection _SqlConnection;
        private NpgsqlTransaction _SqlTransaction;
        private string _ConnectionString; 

        public Dal(DalConfig config)
        {
            _ConnectionString = config.Connection;
        }

        public async Task BeginTransaction()
        {
            await Connect();
            _SqlTransaction = _SqlConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _SqlTransaction.Commit();
            _SqlTransaction.Dispose();
        }

        public void RollbackTransaction()
        {
            _SqlTransaction.Rollback();
            _SqlTransaction.Dispose();
        }

        public void Close()
        {
            if (_SqlConnection != null)
            {
                _SqlConnection.Dispose(); // Will also close
                _SqlConnection = null;
            }
        }

        private async Task Connect()
        {
            try {
            if (_SqlConnection == null) _SqlConnection = new NpgsqlConnection(_ConnectionString);
            if (_SqlConnection.State == System.Data.ConnectionState.Closed) await _SqlConnection.OpenAsync();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public async Task<long> AddApplication(Application application)
        {
            long returnValue = -1;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("insert into applications (appname, appeui) values (@appname, @appeui) returning appkey;", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@appname", application.Name);
                cmd.Parameters.AddWithValue("@appeui", application.AppEUI);

                returnValue = (long) await cmd.ExecuteScalarAsync();
            }
            return returnValue;
        }

        public async Task<Application> GetApplication(long id)
        {
            Application returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT appkey, appname, appeui from applications where appkey = @appkey", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@appkey", id);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new Application()
                    { 
                        Id = dr.GetInt64(0),
                        Name = dr.GetString(1),
                        AppEUI = dr.GetString(2)
                    };
                }
            }
            return returnvalue;
        }

        public async Task<Application> GetApplicationOnName(string name)
        {
            Application returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT appkey, appname, appeui from applications where appname = @appname", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@appname", name);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new Application()
                        { 
                            Id = dr.GetInt64(0),
                            Name = dr.GetString(1),
                            AppEUI = dr.GetString(2)
                        };
                }
            }
            return returnvalue;
        }

        public async Task<Application> GetApplicationOnAppEUI(string appeui)
        {
            Application returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT appkey, appname, appeui from applications where appeui = @appeui", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@appeui", appeui);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new Application()
                        { 
                            Id = dr.GetInt64(0),
                            Name = dr.GetString(1),
                            AppEUI = dr.GetString(2)
                        };
                }
            }
            return returnvalue;
        }

        public async Task<long> AddDevice(Device device)
        {
            long returnValue = -1;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("insert into devices (devapp, deveui, devappkey) values (@devapp, @deveui, @devappkey) returning devkey;", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@devapp", device.Application);
                cmd.Parameters.AddWithValue("@deveui", device.DevEUI);
                cmd.Parameters.AddWithValue("@devappkey", device.AppKey);

                returnValue = (long) await cmd.ExecuteScalarAsync();
            }
            return returnValue;
        }

        public async Task<Device> GetDevice(long id)
        {
            Device returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT devkey, devapp, deveui, devappkey from devices where devkey = @devkey", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@devkey", id);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new Device()
                    { 
                        Id = dr.GetInt64(0),
                        Application = dr.GetInt64(1),
                        DevEUI = dr.GetString(2),
                        AppKey = dr.GetString(3)
                    };
                }
            }
            return returnvalue;
        }

        public async Task<Device> GetDeviceOnDevEUI(string deveui)
        {
            Device returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT devkey, devapp, deveui, devappkey from devices where deveui = @deveui", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@deveui", deveui);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new Device()
                    { 
                        Id = dr.GetInt64(0),
                        Application = dr.GetInt64(1),
                        DevEUI = dr.GetString(2),
                        AppKey = dr.GetString(3)
                    };
                }
            }
            return returnvalue;
        }

        public async Task<Device> GetDeviceOnAppEUIDevEUI(string appeui, string deveui)
        {
            Device returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT devkey, devapp, deveui, devappkey from devices JOIN applications on devapp=appkey WHERE appeui=@appeui AND deveui=@deveui", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@appeui", appeui);
                cmd.Parameters.AddWithValue("@deveui", deveui);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new Device()
                    { 
                        Id = dr.GetInt64(0),
                        Application = dr.GetInt64(1),
                        DevEUI = dr.GetString(2),
                        AppKey = dr.GetString(3)
                    };
                }
            }
            return returnvalue;
        }

        public async Task<long> AddSession(Session session)
        {
            long returnValue = -1;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("insert into sessions (sesdev, sesdevnonce, sesappnonce, sesnwk, sesnwkskey, sesappskey, sesactive) values (@sesdev, @sesdevnonce, @sesappnonce, @sesnwk, @sesnwkskey, @sesappskey, @sesactive) returning seskey;", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@sesdev", session.Device);
                cmd.Parameters.AddWithValue("@sesdevnonce", session.DevNonce);
                cmd.Parameters.AddWithValue("@sesappnonce", session.AppNonce);
                cmd.Parameters.AddWithValue("@sesnwk", session.NwkAddr);
                cmd.Parameters.AddWithValue("@sesnwkskey", session.NwkSKey);
                cmd.Parameters.AddWithValue("@sesappskey", session.AppSKey);
                cmd.Parameters.AddWithValue("@sesactive", session.Active);

                returnValue = (long) await cmd.ExecuteScalarAsync();
            }
            return returnValue;
        }

        public async Task<Session> GetSession(long id) 
        {
            Session returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT seskey, sesdev, sesdevnonce, sesappnonce, sesnwk, sesnwkskey, sesappskey, sesactive from sessions where seskey=@seskey", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@seskey", id);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new Session()
                    { 
                        Id = dr.GetInt64(0),
                        Device = dr.GetInt64(1),
                        DevNonce = dr.GetString(2),
                        AppNonce = dr.GetString(3),
                        NwkAddr = dr.GetInt64(4),
                        NwkSKey = dr.GetString(5),
                        AppSKey = dr.GetString(6),
                        Active = dr.GetDateTime(7)
                    };
                }
            }
            return returnvalue;
        }

        public async Task<Session> GetSessionOnDeviceActive(long device)
        {
            Session returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT seskey, sesdev, sesdevnonce, sesappnonce, sesnwk, sesnwkskey, sesappskey, sesactive from sessions where sesdev=@sesdev and sesactive > now()", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@sesdev", device);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new Session()
                    { 
                        Id = dr.GetInt64(0),
                        Device = dr.GetInt64(1),
                        DevNonce = dr.GetString(2),
                        AppNonce = dr.GetString(3),
                        NwkAddr = dr.GetInt64(4),
                        NwkSKey = dr.GetString(5),
                        AppSKey = dr.GetString(6),
                        Active = dr.GetDateTime(7)
                    };
                }
            }
            return returnvalue;
        }

        public async Task<Session> GetSessionOnDeviceDevNonceActive(long device, string devnonce)
        {
            Session returnvalue = null;
            try {
            await this.Connect();
            using (var cmd = new NpgsqlCommand("SELECT seskey, sesdev, sesdevnonce, sesappnonce, sesnwk, sesnwkskey, sesappskey, sesactive from sessions where sesdev=@sesdev and sesdevnonce=@sesdevnonce and sesactive > now()", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@sesdev", device);
                cmd.Parameters.AddWithValue("@sesdevnonce", devnonce);
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        return null;
                    returnvalue = new Session()
                    { 
                        Id = dr.GetInt64(0),
                        Device = dr.GetInt64(1),
                        DevNonce = dr.GetString(2),
                        AppNonce = dr.GetString(3),
                        NwkAddr = dr.GetInt64(4),
                        NwkSKey = dr.GetString(5),
                        AppSKey = dr.GetString(6),
                        Active = dr.GetDateTime(7)
                    };
                }
            }
            } catch (Exception e) {
                throw;
            }
            return returnvalue;
        }

        public async Task<NwkAddr> GetFreeNwkAddr()
        {
            NwkAddr returnvalue = null;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("select nwkkey, nwkaddr from nwkaddrs left outer join sessions on sesnwk=nwkkey where sesactive is null or sesactive <= now() limit 1", _SqlConnection, _SqlTransaction))
            {
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (!(await dr.ReadAsync()))
                        throw new NotFoundException();
                    returnvalue = new NwkAddr()
                    { 
                        Id = dr.GetInt64(0),
                        NetworkAddress = dr.GetInt64(1)
                    };
                }
            }
            return returnvalue;
        }

        public async Task SetActiveSessionsInactive(long device)
        {
            await this.Connect();
            using (var cmd = new NpgsqlCommand("update sessions set sesactive = now() where sesdev=@sesdev and sesactive > now()", _SqlConnection, _SqlTransaction))
            {
                cmd.Parameters.AddWithValue("@sesdev", device);
                await cmd.ExecuteScalarAsync();
            }
        }
    }
}
