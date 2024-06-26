﻿using FrameworkSDK.MonoGame.Graphics.Basic;
using FrameworkSDK.MonoGame.Mvc;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Mvc
{
	public interface IView : IGraphicComponent, ISceneComponent, INamed
    {
	    object DataModel { get; }

        IController Controller { get; }

        void Destroy();

	    void SetDataModel(object dataModel);

	    void SetController(IController controller);
    }
}
