using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetExtensions;

namespace Console.Core.Commands
{
    public class AttributeConsoleCommandsSearcher
    {
        public IEnumerable<Type> SearchCommands()
        {
            return AppDomain.CurrentDomain.GetAllTypes()
                .Where(type => type.GetCustomAttribute<RegisterConsoleCommandAttribute>() != null);
        }
    }
}