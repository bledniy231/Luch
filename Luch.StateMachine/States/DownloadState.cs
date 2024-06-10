using MassTransit;

namespace Luch.StateMachine.States
{
	public class DownloadState : SagaStateMachineInstance
	{
		public Guid CorrelationId { get; set; }
		public string CurrentState { get; set; }
		public string FileUrl { get; set; }
		public DateTime DownloadStartTime { get; set; }
		public DateTime DownloadEndTime { get; set; }
	}
}
