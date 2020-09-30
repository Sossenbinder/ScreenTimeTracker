using System;
using ScreenTimeTracker.Services.Interface;

namespace ScreenTimeTracker.Services
{
	internal class TrackingService : IDisposable
	{
		private readonly IDataProvider _dataProvider;

		public TrackingService(IDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
		}

		public void StartListening()
		{
			_dataProvider.StartListening();
		}

		public void Dispose()
		{
			_dataProvider.StopListening();
		}
	}
}