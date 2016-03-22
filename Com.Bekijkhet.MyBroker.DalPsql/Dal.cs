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

        public async Task<List<Broker>> GetBrokers()
        {
            var returnlist = new List<Broker>();
            await Connect();
            using (var Cmd = new NpgsqlCommand("SELECT brokey, broname, broendpoint from brokers", _SqlConnection, _SqlTransaction))
            {
                using (var Dr = await Cmd.ExecuteReaderAsync())
                {
                    while (await Dr.ReadAsync())
                    {
                        returnlist.Add(new Broker() { 
                            Id = Dr.GetInt64(0),
                            Name = Dr.GetString(1),
                            Endpoint = Dr.GetString(2)
                        });
                    }
                }
            }
            return returnlist;
        }

        public async Task<long> AddApplication(Application application)
        {
            long returnValue = -1;
            await this.Connect();
            using (var cmd = new NpgsqlCommand("insert into application (appname, appeui) values (@appname, @appeui) returning appkey;", this.connection))
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
            using (var cmd = new NpgsqlCommand("SELECT appkey, appname, appeui from application where appkey = @appkey", _SqlConnection, _SqlTransaction))
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
            using (var cmd = new NpgsqlCommand("SELECT appkey, appname, appeui from application where appname = @appname", _SqlConnection, _SqlTransaction))
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
            using (var cmd = new NpgsqlCommand("SELECT appkey, appname, appeui from application where appeui = @appeui", _SqlConnection, _SqlTransaction))
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
        }

        public async Task<Device> GetDevice(long id)
        {
        }

        public async Task<Device> GetDeviceOnDevEUI(string deveui)
        {
        }

        public async Task<Device> GetDeviceOnAppEUIDevEUI(string appeui, string deveui)
        {
        }

        public async Task<long> AddSession(Session session)
        {
        }

        public async Task<Session> GetSession(long id) 
        {
        }

        public async Task<Session> GetSessionOnDeviceActive(long device)
        {
        }

        public async Task<Session> GetSessionOnDeviceDevNonceActive(long device, string devnonce)
        {
        }

        public async Task<NwkAddr> GetFreeNwkAddr()
        {
        }

        public async Task SetActiveSessionsInactive(long device)
        {
        }
    }
}
