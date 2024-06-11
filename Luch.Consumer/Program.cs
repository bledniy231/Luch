using Luch.Consumer.Consumers;
using Luch.Consumer.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Luch.Consumer
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
						cfg.AddConsumer<DownloadRequestConsumer>();
						cfg.UsingRabbitMq((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
					})
					.AddMassTransitHostedService();

					services.Configure<FilesOptions>(ctx.Configuration.GetSection("FilesOprtions"));
				})
				.Build();

			host.RunAsync().GetAwaiter().GetResult();
		}
	}
}
