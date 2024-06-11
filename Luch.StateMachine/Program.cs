using Luch.StateMachine.Logger;
using Luch.StateMachine.Options;
using Luch.StateMachine.States;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Luch.StateMachine
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var host = Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((ctx, cfg) =>
				{
					cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
				})
				.ConfigureServices((ctx, services) =>
				{
					services.AddMassTransit(cfg =>
					{
						cfg.AddSagaStateMachine<DownloadStateMachine, DownloadState>().InMemoryRepository();
						cfg.UsingRabbitMq((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
					})
					.AddMassTransitHostedService();

					services.Configure<StatesLoggerOptions>(ctx.Configuration.GetSection("Logger"));
					services.AddSingleton<IStatesLogger, StatesLogger>();
				})
				.Build();

			host.RunAsync().GetAwaiter().GetResult();
		}
	}
}
