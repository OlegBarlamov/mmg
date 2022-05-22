using System;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public class MvcTypesDeclaration
    {
        [CanBeNull]
        public Type View { get; }
        [CanBeNull]
        public Type Model { get; }
        [CanBeNull]
        public Type Controller { get; }
        
        public MvcTypesDeclaration([CanBeNull] Type model, [CanBeNull] Type view, [CanBeNull] Type controller)
        {
            Model = model;
            View = view;
            Controller = controller;
        }

        public override string ToString()
        {
            return string.Format(NullFormatProvider.Instance, "{0}<->{1}<->{2}", View, Controller, Model);
        }
    }
}