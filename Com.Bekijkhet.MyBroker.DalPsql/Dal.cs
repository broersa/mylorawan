using System;
using Npgsql;
using System.Threading.Tasks;
using Com.Bekijkhet.MyRouter.Dal;
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

        public void BeginTransaction()
        {
            Connect();
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

        private void Connect()
        {
            try {
            if (_SqlConnection == null) _SqlConnection = new NpgsqlConnection(_ConnectionString);
            if (_SqlConnection.State == System.Data.ConnectionState.Closed) _SqlConnection.Open();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public async Task<List<Broker>> GetBrokers()
        {
            var returnlist = new List<Broker>();
            Connect();
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

            
    }
}

