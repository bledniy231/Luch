namespace Luch.Contract.Requests
{
	public class DownloadRequest
	{
		public Guid CorrelationId { get; set; }
		public string FileUrl { get; set; }
	}
}
