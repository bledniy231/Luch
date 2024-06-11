namespace Luch.Contract.Events
{
	public interface IDownloadHandled
	{
		public Guid CorrelationId { get; set; }
		public string? InternalFullFileName { get; set; }
		public string? FailedMessage { get; set; }
	}
}
