using MassTransit;

namespace Luch;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
		builder.Services.AddMassTransit(x =>
		{
			x.AddSagaStateMachine<DownloadStateMachine, DownloadState>()
				.InMemoryRepository();

			x.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host("rabbitmq://localhost", h =>
				{
					h.Username("guest");
					h.Password("guest");
				});

				cfg.ConfigureEndpoints(context);
			});
		});

		builder.Services.AddMassTransitHostedService();

		builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Run();
    }
}