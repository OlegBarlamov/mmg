using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Basic
{
    public static class BaseExtensions
    {
        private static readonly Singletone<IRandomService> RandomServiceSingletone = new Singletone<IRandomService>();
        
        public static T PickRandom<T>([NotNull] this IReadOnlyList<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count < 1)
                throw new ArgumentException("Input list is empty");

            var randomService =
                RandomServiceSingletone.GetOrCreate(() => AppContext.ServiceLocator.Resolve<IRandomService>());

            var index = randomService.NextInteger(0, list.Count);
            return list[index];
        }
    }
}