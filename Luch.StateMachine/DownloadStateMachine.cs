using Luch.Contract.Events;
using Luch.Contract.Requests;
using Luch.StateMachine.States;
using MassTransit;

namespace Luch.StateMachine
{
	public class DownloadStateMachine : MassTransitStateMachine<DownloadState>
	{
		public DownloadStateMachine()
		{
			InstanceState(s => s.CurrentState);

			Event(() => DownloadRequested, c =>
			{
				c.CorrelateById(ctx => ctx.Message.CorrelationId);
				c.SelectId(ctx => ctx.Message.CorrelationId);
			});

			Initially(When(DownloadRequested)
				.Then(ctx =>
				{
					ctx.Saga.FileUrl = ctx.Message.FileUrl;
					ctx.Saga.DownloadStartTime = DateTime.UtcNow;
				})
				.TransitionTo(Downloading));

			During(Downloading, 
				When(DownloadRequested)
				.Then(ctx =>
				{
					ctx.Saga.FileUrl = ctx.Message.FileUrl;
					ctx.Saga.DownloadStartTime = DateTime.UtcNow;
				}),
				When(DownloadCompleted)
				.Then(ctx =>
				{
					ctx.Saga.DownloadEndTime = DateTime.UtcNow;
				})
				.TransitionTo(Downloaded));

			SetCompletedWhenFinalized();
		}

		public State Downloading { get; private set; }
		public State Downloaded { get; private set; }

		public Event<DownloadRequest> DownloadRequested { get; private set; }
		public Event<DownloadCompletedEvent> DownloadCompleted { get; private set; }
	}
}
