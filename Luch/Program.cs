using Luch.StateMachine;
using Luch.StateMachine.Consumers;
using Luch.StateMachine.States;
using MassTransit;

namespace Luch;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
		builder.Services.AddAuthorization();
		builder.Services.AddMassTransit(cfg =>
		{
			cfg.AddConsumer<DownloadHandledConsumer>();
			cfg.UsingRabbitMq((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
		})
		.AddMassTransitHostedService();

		builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
		builder.Services.AddLogging(configure => configure.AddConsole());

		var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}