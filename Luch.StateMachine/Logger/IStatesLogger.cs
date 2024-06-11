namespace Luch.StateMachine.Logger
{
	public interface IStatesLogger
	{
		void LogState(Guid correlationId, string state);
		void LogTransition(Guid correlationId, string fromState, string toState);
		void LogError(string message);
		void LogInformation(string message);
		void LogWarning(string message);
	}
}
