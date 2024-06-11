using Luch.StateMachine.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text;

namespace Luch.StateMachine.Logger
{
	internal class StatesLogger : IStatesLogger
	{
		private readonly object _lock = new();
		private readonly string _logFilePath;

		public StatesLogger(IOptions<StatesLoggerOptions> options)
		{
			_logFilePath = options.Value.LogFileFullName;
			var logDirectory = Path.GetDirectoryName(_logFilePath);
			if (!Directory.Exists(logDirectory))
			{
				Directory.CreateDirectory(logDirectory);
			}
		}

		private void Log(string message)
		{
			lock (_lock)
			{
				using var writer = new StreamWriter(_logFilePath, true, Encoding.UTF8);
				writer.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss:fff} | {message}");
			}
		}

		public void LogInformation(string message)
		{
			Log($"INFO | {message}");
		}

		public void LogWarning(string message)
		{
			Log($"WARN | {message}");
		}

		public void LogError(string message)
		{
			Log($"ERROR | {message}");
		}

		public void LogState(Guid correlationId, string state)
		{
			Log($"INFO | CorId: {correlationId} - Current state: {state}");
		}

		public void LogTransition(Guid correlationId, string fromState, string toState)
		{
			Log($"INFO | CorId: {correlationId} - Transition from {fromState} to {toState} applied");
		}
	}

}