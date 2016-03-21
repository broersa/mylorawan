using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Bekijkhet.MyRouter.Dal
{
    public interface IDal
    {
        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        void Close();

        Task<List<Broker>> GetBrokers();
    }
}

