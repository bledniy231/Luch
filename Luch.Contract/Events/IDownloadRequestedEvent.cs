namespace Luch.Contract.Events
{
    public interface IDownloadRequestedEvent
    {
        public Guid CorrelationId { get; set; }
        public string FileUrl { get; set; }
    }
}
