using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using ScreenTimeTracker.Common.DataTypes.Models;
using ScreenTimeTracker.Common.DataTypes.Results;
using ScreenTimeTracker.Common.Entities;
using ScreenTimeTracker.Common.Extensions;
using ScreenTimeTracker.Worker.DataTypes;
using ScreenTimeTracker.Worker.Extensions;
using ScreenTimeTracker.Worker.Service.Interface;
using ScreenTimeTracker.Worker.Utils;
using ScreenTimeTracker.Worker.Utils.Interface;

namespace ScreenTimeTracker.Worker.Service
{
	internal class WorkTimesService : IWorkTimesService
	{
		private readonly ITableHelper _tableHelper;

		public WorkTimesService(TableHelperRegistry tableHelperRegistry)
		{
			_tableHelper = tableHelperRegistry.TableHelpers[Table.WorkTimes];
		}

		public async Task LogStart(string userName, LogTimeModel logTimeModel)
		{
			var timeStamp = logTimeModel.Timestamp!.Value;
			var workTime = await GetWorkTime(userName, timeStamp);

			workTime.StartTimes.Add(timeStamp);

			await InsertOrUpdateWorkTimes(workTime);
		}

		public async Task LogEnd(string userName, LogTimeModel logTimeModel)
		{
			var timeStamp = logTimeModel.Timestamp!.Value;
			var workTime = await GetWorkTime(userName, timeStamp);

			workTime.EndTimes.Add(timeStamp);

			await UpdateEndTime(userName, timeStamp, workTime);

			await InsertOrUpdateWorkTimes(workTime);
		}

		private async Task UpdateEndTime(string userName, DateTime endTime, WorkTimeEntity currentDayInfo)
		{
			DateTime lastStart;

			if (currentDayInfo.StartTimes.Any())
			{
				var orderedStartTimes = currentDayInfo
					.StartTimes
					.OrderByDescending(x => x);

				lastStart = orderedStartTimes.First();
			}
			else
			{
				// Okay, no start time on the current day. Let's check the 3 days before, in case someone did a night shift
				var lastDayInfo = await GetWorkTime(userName, endTime.AddDays(-3));

				var orderedStartTimes = lastDayInfo
					.StartTimes
					.OrderByDescending(x => x)
					.ToList();

				// In case we didn't find something for the day before, just take the input date's midnight value.
				// In change someone works > 3 days in a row -> 1) wtf 2) add that logic here
				lastStart = orderedStartTimes.Any() ? orderedStartTimes.First() : endTime.Date;
			}

			var timeSinceLastStart = endTime - lastStart;
			currentDayInfo.TotalMinutes += timeSinceLastStart.TotalMinutes;
		}

		public async IAsyncEnumerable<WorkTimes> GetWorkTimes(string userName, GetWorkTimesModel getWorkTimesModel)
		{
			var (start, end) = TimeUtils.GetRequestedDates(getWorkTimesModel.StartDate, getWorkTimesModel.EndDate);

			var workTimesForAccount = await GetWorkTimes(userName, start, end);

			foreach (var workTime in workTimesForAccount)
			{
				var startTimes = workTime.StartTimes.OrderBy(x => x).ToList();

				if (!startTimes.Any())
				{
					continue;
				}

				var endTimes = workTime.EndTimes.OrderBy(x => x).ToList();

				var midnightTime = startTimes.First().AddDays(1).Date.AddSeconds(-1);

				if (startTimes.Count > endTimes.Count)
				{
					for (var i = endTimes.Count - 1; i < endTimes.Count; ++i)
					{
						endTimes[i] = midnightTime;
					}
				}

				var timePairs = startTimes
					.Zip(endTimes)
					.Select(timeSet => new WorkTimeSpan()
					{
						Start = timeSet.First,
						End = timeSet.Second
					})
					.ToList();

				yield return new WorkTimes()
				{
					TotalWorkMinutes = workTime.TotalMinutes,
					WorkTimeSets = timePairs,
					Date = DateTime.Parse(workTime.RowKey),
				};
			}
		}

		private async Task<List<WorkTimeEntity>> GetWorkTimes(string userName, DateTime? startDate, DateTime? endDate)
		{
			var workTimesTable = await _tableHelper.GetTable();

			var partitionKey = AccountUtils.GetPartitionIndexForName(userName).ToString();

			var query = new TableQuery<WorkTimeEntity>()
				.AndWhereEquals(nameof(WorkTimeEntity.PartitionKey), partitionKey)
				.AndWhereEquals(nameof(WorkTimeEntity.UserName), userName);

			if (startDate != null)
			{
				query = query.AndWhereGreaterEqual(nameof(WorkTimeEntity.RowKey), startDate.Value.ToStorageSafeShortDateString());
			}

			if (endDate != null)
			{
				query = query.AndWhereLessEqual(nameof(WorkTimeEntity.RowKey), endDate.Value.ToStorageSafeShortDateString());
			}

			return await workTimesTable.ExecuteQueryFull(query);
		}

		private async Task<WorkTimeEntity> GetWorkTime(string userName, DateTime timeStamp)
		{
			var workTimeTable = await _tableHelper.GetTable();

			var partitionKey = AccountUtils.GetPartitionIndexForName(userName).ToString();
			var rowKey = timeStamp.Date.ToStorageSafeShortDateString();

			var query = new TableQuery<WorkTimeEntity>()
				.AndWhereEquals(nameof(WorkTimeEntity.PartitionKey), partitionKey)
				.AndWhereEquals(nameof(WorkTimeEntity.RowKey), rowKey)
				.AndWhereEquals(nameof(WorkTimeEntity.UserName), userName);

			var workTime = (await workTimeTable.ExecuteQueryFull(query)).FirstOrDefault() ?? new WorkTimeEntity()
			{
				PartitionKey = partitionKey,
				RowKey = rowKey,
				StartTimes = new List<DateTime>(),
				EndTimes = new List<DateTime>(),
				UserName = userName,
				Timestamp = timeStamp,
			};

			return workTime;
		}

		private async Task InsertOrUpdateWorkTimes(WorkTimeEntity entity)
		{
			var workTimeTable = await _tableHelper.GetTable();

			var operation = TableOperation.InsertOrMerge(entity);

			await workTimeTable.ExecuteAsync(operation);
		}
	}
}