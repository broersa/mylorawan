using System;

namespace Com.Bekijkhet.MyBroker.Dal
{
	public class Device
	{
		public long Id { get; set; }
		public long Application { get; set; }
		public string DevEUI { get; set; }
		public string AppKey { get; set; }
	}
}

