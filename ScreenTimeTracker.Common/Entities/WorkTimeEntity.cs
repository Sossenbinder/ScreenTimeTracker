using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace ScreenTimeTracker.Common.Entities
{
	public class WorkTimeEntity : SerializeableBaseEntity
	{
		public double TotalMinutes { get; set; }

		public string UserName { get; set; }

		public string StartTimesSerialized
		{
			get => SerializeList(_startTimes);
			set => DeserializeList(ref _startTimes, value);
		}

		private List<DateTime> _startTimes = null!;

		[IgnoreProperty]
		public List<DateTime> StartTimes
		{
			get => _startTimes;
			set => _startTimes = value;
		}

		public string EndTimesSerialized
		{
			get => SerializeList(EndTimes);
			set => DeserializeList(ref _endTimes, value);
		}

		private List<DateTime> _endTimes = null!;

		[IgnoreProperty]
		public List<DateTime> EndTimes
		{
			get => _endTimes;
			set => _endTimes = value;
		}
	}
}