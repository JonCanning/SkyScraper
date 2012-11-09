using System;
using System.Threading.Tasks;

namespace SkyScraper
{
    public interface ITaskRunner
    {
        void Run(Action action);
        Task Run(Task task);
        void WaitForAllTasks();
    }
}