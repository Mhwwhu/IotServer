
using BackEnd;
using BackEnd.DbStore;
using BackEnd.Listener;
using BackEnd.DbStore.Entity;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace IotServer
{
    public class Program
	{
		//	public static void Main(string[] args)
		//	{
		//		var builder = WebApplication.CreateBuilder(args);

		//		// Add services to the container.

		//		builder.Services.AddControllers();
		//		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		//		builder.Services.AddEndpointsApiExplorer();
		//		builder.Services.AddSwaggerGen();

		//		var app = builder.Build();

		//		// Configure the HTTP request pipeline.
		//		if (app.Environment.IsDevelopment())
		//		{
		//			app.UseSwagger();
		//			app.UseSwaggerUI();
		//		}

		//		app.UseHttpsRedirection();

		//		app.UseAuthorization();


		//		app.MapControllers();

		//		app.Run();
		//	}

		public static async Task Main(string[] args)
		{
			Context context = new Context();
			context.Configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();
			IServiceCollection services = new ServiceCollection();
			
			services.AddSingleton(context.Configuration);
			services.AddSingleton(context);
			Context.ConfigureServices(services);
			context.BuildServiceProvider(services);

			var mqttListener = context.ServiceProvider.GetRequiredService<MqttListener>();
			await mqttListener.StartAsync();
			//var dbController = context.ServiceProvider.GetRequiredService<DbController>();
			//var startTime = new DateTime(2024, 7, 17, 4, 30, 0);
			//var endTime = new DateTime(2024, 7, 17, 4, 40, 0);
			//var msgs = await dbController.GetMessagesByTimeAsync("Root/SleepingMonitor/Test", startTime, endTime);
			//foreach(var msg in msgs!)
			//{
			//	var s = BitConverter.ToString(msg.Message).Replace('-', ' ');
			//	Console.WriteLine(s);
			//}
		}
	}
}
