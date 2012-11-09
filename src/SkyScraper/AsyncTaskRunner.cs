using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class AsyncTaskRunner : ITaskRunner
    {
        readonly ConcurrentDictionary<int, Task> tasks = new ConcurrentDictionary<int, Task>();

        public void Run(Action action)
        {
            var task = Task.Factory.StartNew(action);
            Run(task);
        }

        public async Task Run(Task task)
        {
            tasks.AddOrUpdate(task.GetHashCode(), task, (i, t) => t);
            await task;
            Task outTask;
            tasks.TryRemove(task.GetHashCode(), out outTask);
        }

        public void WaitForAllTasks()
        {
            Task.WaitAll(tasks.Values.ToArray());
        }
    }
}