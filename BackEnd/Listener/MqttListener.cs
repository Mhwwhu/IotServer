using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.DataStructure;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using BackEnd.DbStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Listener
{
	public class MqttListener
	{
		private readonly string _tag = "MqttListener";
		private string _host;
		private int _port;
		private string _clientId;
		private IMqttClient _client;
		private ListenerAction _listenerAction;
		private ILoggerService _logger;
		private Context _context;
		private ConcurrentQueue<MqttApplicationMessage> _pubMessages = new();
		private ConcurrentRadixTree<string, Func<MqttApplicationMessage, Task>> _msgHandlerTree = new();

		public MqttListener(Context context, IConfiguration config, ILoggerService logger)
		{
			_logger = logger;
			var section = config.GetSection("Mqtt");
			_host = section["BrokerHost"]!;
			_port = int.Parse(section["Port"]!);
			_clientId = section["ClientId"]!;
			_context = context;
			_listenerAction = new ListenerAction(context, _pubMessages);
			ConstructTree();
		}
		private void ConstructTree()
		{
			_msgHandlerTree.Insert(["Root", "SleepingMonitor"], _listenerAction.SleepingMonitorAction);
		}

		public async Task StartAsync()
		{
			var factory = new MqttFactory();
			var client = factory.CreateMqttClient();
			var options = new MqttClientOptionsBuilder()
				.WithTcpServer(_host, _port)
				.WithClientId(_clientId)
				.WithCleanSession()
				.WithTimeout(new TimeSpan(0, 0, 10))
				.Build();
			client.DisconnectedAsync += async e =>
			{
				var connResult = await client.ConnectAsync(options);
				while (connResult.ResultCode != MqttClientConnectResultCode.Success)
				{
					connResult = await client.ConnectAsync(options);
					_logger.Error(_tag, $"Failed connecting to broker: {_host}:{_port}");
				} 
			};
			var connResult = await client.ConnectAsync(options);
			if (connResult.ResultCode != MqttClientConnectResultCode.Success)
			{
				_logger.Error(_tag, $"Failed connecting to broker: {_host}:{_port}");
				return;
			}
			await client.SubscribeAsync("#");
			client.ApplicationMessageReceivedAsync += OnReceiveMessageAsync;
			_client = client;
			InitTopics();
			//InitAdmin();
			while (true)
			{
				if (_pubMessages.TryDequeue(out var message))
				{
					try
					{
						await _client.PublishAsync(message);
					}
					catch (Exception)
					{
						while (connResult.ResultCode != MqttClientConnectResultCode.Success) ;
						await _client.PublishAsync(message);
					}
				}
			}
		}
		public void InitTopics()
		{
			var dbController = _context.ServiceProvider.GetService<DbController>()!;
			var topic = "Root/SleepingMonitor/Test";
			if (dbController.GetTopicAsync(topic).GetAwaiter().GetResult() == null)
			{
				dbController.CreateTopicAsync(topic).GetAwaiter().GetResult();
			}
			
		}
		public void InitAdmin()
		{
			var dbController = _context.ServiceProvider.GetService<DbController>()!;
			dbController.RegisterUserAsync("Admin", "000000").GetAwaiter().GetResult();
		}
		private async Task OnReceiveMessageAsync(MqttApplicationMessageReceivedEventArgs e)
		{
			var matchedActions = _msgHandlerTree.PrefixMatch(e.ApplicationMessage.Topic.Split('/'));
			foreach(var action in matchedActions)
			{
				await action.Value!(e.ApplicationMessage);
			}
		}
	}
}
