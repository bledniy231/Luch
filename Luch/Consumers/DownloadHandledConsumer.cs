using Luch.Contract.Events;
using MassTransit;

namespace Luch.StateMachine.Consumers
{
	public class DownloadHandledConsumer(ILogger<DownloadHandledConsumer> logger) : IConsumer<IDownloadHandled>
	{
		public async Task Consume(ConsumeContext<IDownloadHandled> context)
		{
			var message = context.Message;

			if (message.FailedMessage != null)
			{
				logger.LogWarning($"FAILED. Received IDownloadHandled event: CorrelationId = {message.CorrelationId}, Message = {message.FailedMessage}");
			}
			else
			{
				logger.LogInformation($"SUCCESSED. Received IDownloadHandled event: CorrelationId = {message.CorrelationId}, InternalFullFileName = {message.InternalFullFileName}");
			}

			await Task.CompletedTask;
		}
	}
}
