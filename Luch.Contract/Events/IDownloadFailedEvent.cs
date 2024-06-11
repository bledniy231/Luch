namespace Luch.Contract.Events
{
	public interface IDownloadFailedEvent
	{
		public Guid CorrelationId { get; set; }
		public string FailedMessage { get; set; }
	}
}
