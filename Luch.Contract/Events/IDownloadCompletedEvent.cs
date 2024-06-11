namespace Luch.Contract.Events
{
	public interface IDownloadCompletedEvent
	{
		public Guid CorrelationId { get; set; }
		public string InternalFullFileName { get; set; }
	}
}
