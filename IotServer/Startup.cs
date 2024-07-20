namespace IotServer
{
	public class Startup
	{
		// 用于配置依赖注入容器
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers(); // 添加控制器服务
		}

		// 用于配置HTTP请求管道
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage(); // 开发环境使用详细错误页面
			}

			app.UseRouting(); // 启用路由中间件

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers(); // 映射控制器路由
			});
		}
	}
}
