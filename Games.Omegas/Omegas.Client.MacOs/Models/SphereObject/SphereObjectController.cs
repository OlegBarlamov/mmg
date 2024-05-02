using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;
using NetExtensions.Helpers;
using Omegas.Client.MacOs.Services;

namespace Omegas.Client.MacOs.Models.SphereObject
{
    public class SphereObjectController : SphereObjectGenericController<SphereObjectData>
    {
        public SphereObjectController([NotNull] OmegaGameService gameService) : base(gameService)
        {
        }
    }
    
    public class SphereObjectGenericController<T> : Controller<T> where T : SphereObjectData
    {
        private readonly List<SphereObjectData> _collidingSpheresToRemove = new List<SphereObjectData>();
        
        private OmegaGameService GameService { get; }

        public SphereObjectGenericController([NotNull] OmegaGameService gameService)
        {
            GameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            foreach (var sphere in DataModel.CollidingSpheres)
            {
                if (DataModel.Dead)
                    break;
                
                if (!sphere.Dead)
                {
                    if ((sphere.Position - DataModel.Position).LengthSquared() >
                        MathExtended.Sqr(sphere.Size + DataModel.Size))
                    {
                        _collidingSpheresToRemove.Add(sphere);
                    }
                    
                    GameService.HandleConsumption(DataModel, sphere, gameTime);
                }
                else
                {
                    _collidingSpheresToRemove.Add(sphere);
                }
            }
            
            DataModel.CollidingSpheres.RemoveRange(_collidingSpheresToRemove);
            _collidingSpheresToRemove.Clear();
        }
    }
}