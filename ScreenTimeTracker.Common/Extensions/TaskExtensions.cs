using System.Threading.Tasks;

namespace ScreenTimeTracker.Common.Extensions
{
	public static class TaskExtensions
	{
		public static Task IgnoreTaskCancelledException(this Task task)
		{
			return task.ContinueWith(t => t.Exception?.Handle(exc => exc is TaskCanceledException));
		}
	}
}