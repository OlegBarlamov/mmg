using System;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Mvc
{
    public class MvcComponentGroup : IMvcComponentGroup
    {
        [CanBeNull] public object Model { get; set; }
        [CanBeNull] public IController Controller { get; set; }
        [CanBeNull] public IView View { get; set; }

	    public override string ToString()
	    {
		    return string.Format(NullFormatProvider.Instance, "{0}<->{1}<->{2}", View, Controller, Model);
	    }

	    public static MvcComponentGroup FromView([NotNull] IView view)
	    {
		    if (view == null) throw new ArgumentNullException(nameof(view));
		    
		    return new MvcComponentGroup
		    {
			    View = view,
			    Controller = view.Controller,
			    Model = view.DataModel
		    };
	    }
	    
	    public static MvcComponentGroup FromController([NotNull] IController controller)
	    {
		    if (controller == null) throw new ArgumentNullException(nameof(controller));
		    
		    return new MvcComponentGroup
		    {
			    View = controller.View,
			    Controller = controller,
			    Model = controller.DataModel
		    };
	    }
    }
}