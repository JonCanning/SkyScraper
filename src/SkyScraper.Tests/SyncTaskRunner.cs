using System;
using System.Threading.Tasks;

namespace SkyScraper.Tests
{
    class SyncTaskRunner : ITaskRunner
    {
        public void Run(Action action)
        {
            action();
        }

        public Task Run(Task task)
        {
            task.Start();
            return task;
        }

        public void WaitForAllTasks()
        {

        }
    }
}