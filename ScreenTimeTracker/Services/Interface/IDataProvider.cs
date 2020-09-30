namespace ScreenTimeTracker.Services.Interface
{
	internal interface IDataProvider
	{
		void StartListening();

		void StopListening();
	}
}