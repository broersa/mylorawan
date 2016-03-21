using System;

namespace Com.Bekijkhet.MyRouter.DalPsql
{
    public class DalConfig
    {
        public string Connection { get; private set; }

        public DalConfig(string connection)
        {
            Connection = connection;
        }
    }
}

