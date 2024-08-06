using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Listener.Entity
{
	public class SleepingBdataFrame
	{
		public int Id { get; set; }
		public int DataCount { get; set; }
		public List<SleepingBdata> Data { get; set; } = new();
	}
	public class SleepingOdataFrame
	{
		public int Id { get; set; }
		public int DataCount { get; set; }
		public List<SleepingOdata> Data { get; set; } = new();
	}
	public class SleepingOdata
	{
		public DateTime Timestamp { get; set; }
		public ushort[] PiezoelectricSignal { get; set; } = new ushort[25];
		public ushort PiezoresistiveSignal { get; set; }
	}
	public class SleepingBdata
	{
		public DateTime Timestamp { get; set; }
		public byte Status { get; set; }
		public byte HeartRate { get; set; }
		public byte BreatingRate { get; set; }
	}
}
