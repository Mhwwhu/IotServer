using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd;
using BackEnd.Analysis.Entity;
using BackEnd.DbStore;
using Microsoft.Extensions.DependencyInjection;
namespace BackEnd.Analysis
{
	public class HRBRAnalysis(Context context)
	{
		private ILoggerService _logger = context.ServiceProvider!.GetRequiredService<ILoggerService>();
		private DbController _dbController = context.ServiceProvider!.GetRequiredService<DbController>();
		public async Task<AnalysisResult> StartAnalysisAsync(string topic, DateTime startTime, DateTime endTime)
		{
			throw new NotImplementedException();
			// 以一小时为单元向数据库请求数据
			var period = new TimeSpan(1, 0, 0);
			for (var start = startTime; start < endTime; startTime += period) 
			{
				var end = start + period;
				if (start + period > endTime) end = endTime;
				var sleepingData = await _dbController.GetMessagesByTimeAsync(topic, start, start + period);
				// TODO 分批次分析结果并返回result，算法应该放在另一个方法中
			}
		}
	}
}
