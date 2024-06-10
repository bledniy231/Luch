using Luch.StateMachine.Logger;
using Luch.StateMachine.Models;
using Luch.StateMachine.States;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Luch.StateMachine
{
	internal class Program
	{
		async static void Main(string[] args)
		{
			var host = Host.CreateDefaultBuilder(args)
				.ConfigureServices((ctx, services) =>
				{
					services.AddMassTransit(cfg =>
					{
						cfg.AddSagaStateMachine<DownloadStateMachine, DownloadState>().InMemoryRepository();
						cfg.UsingRabbitMq((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
					})
					.AddMassTransitHostedService();

					services.Configure<StatesLoggerConfigModel>(ctx.Configuration.GetSection("Logger"));
					services.AddSingleton<IStatesLogger, StatesLogger>();
				})
				.Build();

			await host.RunAsync();
		}
	}
}
