using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace NetExtensions
{
    public static class ReflectionHelper
    {
        public static bool IsSubClassOf(this Type type, params Type[] types)
        {
            return types.Any(type.IsSubclassOf);
        }

        public static IEnumerable<Type> GetAllTypes([NotNull] this AppDomain appDomain)
        {
            if (appDomain == null) throw new ArgumentNullException(nameof(appDomain));
            var assemblies = appDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Type[] types = new Type[0];

                try
                {
                    types = assembly.GetTypes();
                }
                catch (Exception)
                {
                    //TODO log
                    // ignored
                }

                foreach (var type in types)
                    yield return type;
            }
        }
    }
}
