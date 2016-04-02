using System;

namespace Com.Bekijkhet.MyBroker.Dal
{
	public class Session
	{
		public long Id {get;set;}
		public long Device { get; set; }
		public long DevAddr {get;set;}
		public string DevNonce {get;set;}
		public string AppNonce {get;set;}
		public string NwkSKey {get;set;}
		public string AppSKey {get;set;}
		public DateTime Active {get;set;}
	}
}

