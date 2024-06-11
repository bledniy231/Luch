using Luch.Consumer.Options;
using Luch.Contract.Events;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;

namespace Luch.Consumer.Consumers
{
    internal class DownloadRequestConsumer(IOptions<FilesOptions> options) : IConsumer<IDownloadRequestedEvent>
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _internalFilesDir = Path.Combine(Environment.CurrentDirectory, options.Value.InternalFilesDir);

		public async Task Consume(ConsumeContext<IDownloadRequestedEvent> context)
        {
            if (!Directory.Exists(_internalFilesDir))
            {
                Directory.CreateDirectory(_internalFilesDir);
            }

            var url = context.Message.FileUrl;
            var fileName = context.Message.CorrelationId + Path.GetFileName(new Uri(url).AbsolutePath);
            var filePath = Path.Combine(_internalFilesDir, fileName);

            using (var response = await _httpClient.GetAsync(url))
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    await context.Publish<IDownloadFailedEvent>(new
                    {
                        context.Message.CorrelationId,
                        FailedMessage = $"Failed to download file, Exception: {e.Message}"
                    });
                    return;
                }

                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await response.Content.CopyToAsync(fs);
            }

            Console.WriteLine($"File downloaded to: {filePath}");

            await context.Publish<IDownloadCompletedEvent>(new
            {
                context.Message.CorrelationId,
				InternalFullFileName = filePath
            });
        }
    }
}
