using System.Threading.Tasks;

namespace NetExtensions
{
    public static class TaskExtensions
    {
        public static bool IsFinished(this Task task)
        {
            return task.IsCanceled || task.IsCompleted || task.IsFaulted;
        }
    }
}
