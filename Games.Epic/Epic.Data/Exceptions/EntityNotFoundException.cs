using System;

namespace Epic.Data.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(IRepository repository, string requestInfo) 
            : base($"The entity {repository.EntityName} not found in {repository.Name} by {requestInfo}")
        {
            
        }
    }
}