using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Analysis.Entity
{
	public class AnalysisResult
	{
		public double HRrange, RRrange;
		public int HRmin, HRmax, RRmin, RRmax;
		public int[] HRdata, RRdata;
	}
}
