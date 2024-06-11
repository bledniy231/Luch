namespace Luch.Contract.Events
{
	public interface IDownloadNeededEvent
	{
		public Guid CorrelationId { get; set; }
		public string FileUrl { get; set; }
	}
}
