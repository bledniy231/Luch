using Luch.Contract.Events;
using Luch.StateMachine.Logger;
using Luch.StateMachine.States;
using MassTransit;

namespace Luch.StateMachine
{
	public class DownloadStateMachine : MassTransitStateMachine<DownloadState>
	{
		private readonly IStatesLogger _logger;

		public DownloadStateMachine(IStatesLogger logger)
		{
			_logger = logger;

			InstanceState(s => s.CurrentState);

			Event(() => DownloadNeeded, c =>
			{
				c.CorrelateById(ctx => ctx.Message.CorrelationId);
				c.SelectId(ctx => ctx.Message.CorrelationId);
			});

			Event(() => DownloadRequested, c =>
			{
				c.CorrelateById(ctx => ctx.Message.CorrelationId);
				c.SelectId(ctx => ctx.Message.CorrelationId);
			});

			Event(() => DownloadCompleted, c =>
			{
				c.CorrelateById(ctx => ctx.Message.CorrelationId);
				c.SelectId(ctx => ctx.Message.CorrelationId);
			});

			Event(() => DownloadFailed, c =>
			{
				c.CorrelateById(ctx => ctx.Message.CorrelationId);
				c.SelectId(ctx => ctx.Message.CorrelationId);
			});

			Initially(
				When(DownloadNeeded)
				.Then(ctx =>
				{
					ctx.Saga.FileUrl = ctx.Message.FileUrl;
					ctx.Saga.DownloadStartTime = DateTime.UtcNow;
					_logger.LogInformation($"CorId: {ctx.Message.CorrelationId} - Received request to download file: {ctx.Message.FileUrl}");
				})
				.PublishAsync(ctx => ctx.Init<IDownloadRequestedEvent>(new
				{
					ctx.Saga.CorrelationId,
					ctx.Saga.FileUrl
				}))
				.Then(ctx =>
				{
					_logger.LogTransition(ctx.Message.CorrelationId, "Initial", "Downloading");
					_logger.LogState(ctx.Message.CorrelationId, "Downloading");
				})
				.TransitionTo(Downloading));

			During(Downloading, 
				When(DownloadNeeded)
				.Then(ctx =>
				{
					ctx.Saga.FileUrl = ctx.Message.FileUrl;
					ctx.Saga.DownloadStartTime = DateTime.UtcNow;
					_logger.LogInformation($"CorId: {ctx.Message.CorrelationId} - During \"Downloading\" received new request to download file: {ctx.Message.FileUrl}");
				})
				.Then(ctx =>
				{
					_logger.LogTransition(ctx.Message.CorrelationId, "Downloading", "Downloading");
					_logger.LogState(ctx.Message.CorrelationId, "Downloading");
				})
				.TransitionTo(Downloading));

			During(Downloading,
				When(DownloadRequested)
				.Then(ctx =>
				{

				}));

			During(Downloading,
				When(DownloadCompleted)
				.PublishAsync(ctx => ctx.Init<IDownloadHandled>(new
				{
					ctx.Message.CorrelationId,
					ctx.Message.InternalFullFileName
				}))
				.Then(ctx =>
				{
					ctx.Saga.DownloadEndTime = DateTime.UtcNow;
					_logger.LogInformation($"CorId: {ctx.Message.CorrelationId} - Downloading file completed. Full filename: {ctx.Message.InternalFullFileName}");
					_logger.LogTransition(ctx.Message.CorrelationId, "Downloading", "SuccessfullyDownloaded");
					_logger.LogState(ctx.Message.CorrelationId, "SuccessfullyDownloaded");
				})
				.TransitionTo(SuccessfullyDownloaded)
				.Finalize());

			During(Downloading,
				When(DownloadFailed)
				.PublishAsync(ctx => ctx.Init<IDownloadHandled>(new
				{
					ctx.Message.CorrelationId,
					ctx.Message.FailedMessage
				}))
				.Then(ctx =>
				{
					ctx.Saga.DownloadEndTime = DateTime.UtcNow;
					_logger.LogInformation($"CorId: {ctx.Message.CorrelationId} - Downloading file failed. Reason: {ctx.Message.FailedMessage}");
					_logger.LogTransition(ctx.Message.CorrelationId, "Downloading", "DownloadingFailed");
					_logger.LogState(ctx.Message.CorrelationId, "DownloadingFailed");
				})
				.TransitionTo(DownloadingFailed)
				.Finalize());

			SetCompletedWhenFinalized();
		}

		public State Downloading { get; private set; }
		public State SuccessfullyDownloaded { get; private set; }
		public State DownloadingFailed { get; private set; }

		public Event<IDownloadNeededEvent> DownloadNeeded { get; private set; }
		public Event<IDownloadRequestedEvent> DownloadRequested { get; private set; }
		public Event<IDownloadCompletedEvent> DownloadCompleted { get; private set; }
		public Event<IDownloadFailedEvent> DownloadFailed { get; private set; }
	}
}
