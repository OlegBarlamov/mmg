using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic.Core.Utils
{
    public static class MappingUtils
    {
        public static async Task FetchAndMapEntitiesAsync<TObject, TAssocObject>(TObject[] objects,
            Func<TObject, Guid> keySelector,
            Func<Guid, Task<TAssocObject>> fetchAssociatedObjectFunc,
            Action<TObject, TAssocObject> associateAction)
        {
            // Dictionary to store the tasks for fetching associated objects by key
            var tasksMap = new Dictionary<Guid, Task<TAssocObject>>();
            var exceptions = new List<Exception>(); // To collect all exceptions

            // List of tasks for fetching and mapping associated objects
            var tasks = objects.Select(obj =>
            {
                try
                {
                    var key = keySelector(obj);

                    // Check if the key is already being fetched (exists in tasksMap)
                    if (tasksMap.TryGetValue(key, out var existingTask))
                    {
                        // Return the existing task if it's already being fetched
                        return existingTask.ContinueWith(result =>
                        {
                            associateAction(obj, result.Result);
                        });
                    }

                    // If not fetched yet, create a new task to fetch the associated object
                    var newTask = fetchAssociatedObjectFunc(key);

                    // Add the new task to the dictionary
                    tasksMap.Add(key, newTask);

                    // Continue the task to assign the fetched associated object to the main object
                    return newTask.ContinueWith(result =>
                    {
                        associateAction(obj, result.Result);
                    });
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                    return Task.CompletedTask;
                }
            }).ToArray();

            // Await all tasks to complete
            await Task.WhenAll(tasks);
            
            if (exceptions.Any())
            {
                // Throw an aggregate exception if there were failures
                throw new AggregateException("One or more errors occurred during mapping", exceptions);
            }
        }
    }
}