using Luch.Contract.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Luch.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class FilesController(IPublishEndpoint publishEndpoint) : ControllerBase
	{
		[HttpPost]
		public async Task<IActionResult> SendFileUri([FromQuery] string fileUri)
		{
			Guid requestId = Guid.NewGuid();
			await publishEndpoint.Publish<IDownloadNeededEvent>(new
			{
				FileUrl = fileUri,
				CorrelationId = requestId
			});
			
			return Ok(requestId);
		}
	}
}
