using BackEnd.DbStore;
using BackEnd.DbStore.Entity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackEnd.Listener.Entity;
using Microsoft.Extensions.Options;


namespace BackEnd.Listener
{
	// 规定所有的Message均来自已存在的topic，如果有新的topic产生，则需要通过http请求注册topic
	public class ListenerAction
	{
		private Context _context;
		private ILoggerService _logger;
		private DbController _dbController;
		private ConcurrentQueue<MqttApplicationMessage> _pubQueue;
		private JsonSerializerOptions _jsonSerializerOptions;
		public ListenerAction(Context context, ConcurrentQueue<MqttApplicationMessage> pubQueue)
		{
			_context = context;
			_pubQueue = pubQueue;
			_logger = context.ServiceProvider.GetRequiredService<ILoggerService>();
			_dbController = context.ServiceProvider.GetRequiredService<DbController>();
			_jsonSerializerOptions = new JsonSerializerOptions
			{
				WriteIndented = true
			};
		}
		public async Task SleepingMonitorAction(MqttApplicationMessage message)
		{
			var payload = message.PayloadSegment.ToArray();
			var topicName = message.Topic;
			ulong unixTimeStamp;
			if(BitConverter.IsLittleEndian)
			{
				var timeStampArr = new ArraySegment<byte>(payload, 0, sizeof(long)).ToArray();
				unixTimeStamp = BitConverter.ToUInt64(timeStampArr, 0);
			}
			else
			{
				var timeStampArr = new ArraySegment<byte>(payload, 0, sizeof(long)).ToArray();
				Array.Reverse(timeStampArr);
				unixTimeStamp = BitConverter.ToUInt64(timeStampArr, 0);
			}
			try
			{
				DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp);
				// 将UTC时间转换为当前时区的时间
				DateTime localDateTime = dateTimeOffset.ToLocalTime().DateTime;
				int id = await _dbController.PutMessageAsync(localDateTime, topicName, payload);

				var dataContainer = new SleepingData()
				{
					Id = id,
					TimeStamp = localDateTime,
					Seq = payload[8],
					Status = payload[9],
					HeartRate = payload[10],
					BreatingRate = payload[11]
				};
				string jsonString = JsonSerializer.Serialize(dataContainer, _jsonSerializerOptions);
				var forwardTopic = "User/" + string.Join('/', topicName.Split('/').Skip(1));
				var forwardMessage = new MqttApplicationMessageBuilder()
					.WithPayload(jsonString)
					.WithTopic(forwardTopic)
					.WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
					.Build();
				_pubQueue.Enqueue(forwardMessage);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
