using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BackEnd;
using BackEnd.Analysis.Entity;
using BackEnd.DbStore;
using BackEnd.Listener.Entity;
using Microsoft.Extensions.DependencyInjection;
namespace BackEnd.Analysis
{
	public class HRBRAnalysis(Context context)
	{
		private ILoggerService _logger = context.ServiceProvider!.GetRequiredService<ILoggerService>();
		private DbController _dbController = context.ServiceProvider!.GetRequiredService<DbController>();
		public async Task<AnalysisResult> StartAnalysisAsync(string topic, DateTime startTime, DateTime endTime)
		{

			var sleepingData = await _dbController.GetMessagesByTimeAsync(topic, startTime, endTime);

			//参数分配
			var conf1 = context.Configuration!.GetSection("SleepingData");
			var conf2 = context.Configuration.GetSection("Algo");
			int HROffSet = int.Parse(conf1["HeartRateOffset"]!);
			int RROffSet = int.Parse(conf1["BreathingRateOffset"]!);
			int HRMinLimit = int.Parse(conf2["HRMinLimit"]!);
			int HRMaxLimit = int.Parse(conf2["HRMaxLimit"]!);
			int RRMinLimit = int.Parse(conf2["RRMinLimit"]!);
			int RRMaxLimit = int.Parse(conf2["RRMaxLimit"]!);
			int WrongData = int.Parse(conf2["WrongData"]!);
			double WrongRate = double.Parse(conf2["WrongRate"]!);
			int[] HRresultArray = new int[HRMaxLimit];
			int[] RRresultArray = new int[RRMaxLimit];
			int HRSize = 0;
			int RRSize = 0;
			foreach (var message in sleepingData!)
			{

				if (message.Message[HROffSet] != 0 && (message.Message[HROffSet] <= HRMinLimit || message.Message[HROffSet] >= HRMaxLimit) && WrongData > 0)
				{
					WrongData--;
					HRresultArray[message.Message[HROffSet]]++;
					HRSize++;
				}
				else if (message.Message[HROffSet] <= HRMinLimit || message.Message[HROffSet] >= HRMaxLimit)
				{
					;
				}
				else
				{
					HRresultArray[message.Message[HROffSet]]++;
					HRSize++;
				}
			}
			WrongData = int.Parse(conf2["WrongData"]!);
			foreach (var message in sleepingData)
			{
				int RR = message.Message[RROffSet] / 10;
				if (RR != 0 && (RR <= RRMinLimit || RR >= RRMaxLimit) && WrongData > 0)
				{
					WrongData--;
					RRresultArray[RR]++;
					RRSize++;
				}
				else if (RR <= RRMinLimit || RR >= RRMaxLimit)
				{
					;
				}
				else
				{
					RRresultArray[RR]++;
					RRSize++;
				}
			}

			//计算模块
			int HRmin = 0, HRmax = 0;
			int _HRmin = 0, _HRmax = 0;
			double HRave = 0;
			int RRmin = 0, RRmax = 0;
			int _RRmin = 0, _RRmax = 0;
			double RRave = 0;
			//HR
			for (int i = 1; i < HRresultArray.Length; i++)
			{
				HRave += (double)(i * HRresultArray[i]) / HRSize;
			}
			bool Flag = true;
			for (int j = 1; j < HRresultArray.Length; j++)
			{
				if (HRresultArray[j] > 0 && Flag) { HRmin = j; Flag = false; };
				if (HRresultArray[j] > 0 && HRresultArray[j] > HRSize * WrongRate) { _HRmin = j; break; }
			}
			Flag = true;
			for (int j = HRresultArray.Length - 1; j > 0; j--)
			{
				if (HRresultArray[j] > 0 && Flag) { HRmax = j; Flag = false; };
				if (HRresultArray[j] > 0 && HRresultArray[j] > HRSize * WrongRate) { _HRmax = j; break; }
			}
			//RR
			for (int i = 1; i < RRresultArray.Length; i++)
			{
				RRave += (double)(i * RRresultArray[i]) / RRSize;
			}
			Flag = true;
			for (int j = 1; j < RRresultArray.Length; j++)
			{
				if (RRresultArray[j] > 0 && Flag) { RRmin = j; Flag = false; };
				if (RRresultArray[j] > 0 && RRresultArray[j] > RRSize * WrongRate) { _RRmin = j; break; }
			}
			Flag = true;
			for (int j = RRresultArray.Length - 1; j > 0; j--)
			{
				if (RRresultArray[j] > 0 && Flag) { RRmax = j; Flag = false; };
				if (RRresultArray[j] > 0 && RRresultArray[j] > RRSize * WrongRate) { _RRmax = j; break; }
			}
			var analysisResult = new AnalysisResult()
			{
				HRdata = HRresultArray,
				RRdata = RRresultArray,
				HRmax = HRmax,
				HRmin = HRmin,
				HRrange = HRave,
				RRmax = RRmax,
				RRmin = RRmin,
				RRrange = RRave,
			};
			return analysisResult;
		}

	}

}
