using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Listener.Entity
{
	public class SleepingData
	{
		public DateTime TimeStamp { get; set; }
		public int Id { get; set; }
		public byte Seq { get; set; }
		public byte Status { get; set; }
		public byte HeartRate { get; set; }
		public byte BreatingRate { get; set; }
	}
}
