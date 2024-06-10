namespace Luch.StateMachine.Logger
{
	internal interface IStatesLogger
	{
		void LogState(string state);
		void LogTransition(string fromState, string toState);
		void LogError(string message);
		void LogInformation(string message);
		void LogWarning(string message);
	}
}
