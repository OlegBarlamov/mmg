using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.GameStructure;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Graphics.Pipeline
{
    internal class GraphicsPipeline : IGraphicsPipeline
    {
        [NotNull, ItemNotNull]
        public List<IGraphicsPass> Steps { get; } = new List<IGraphicsPass>
        {
            new DefaultPipelinePass()
        };

        private IComponentsByPassAggregator Aggregator { get; }
        private IGraphicsPipelineContextFactory ContextFactory { get; }

        internal GraphicsPipeline([NotNull] IComponentsByPassAggregator aggregator,
            [NotNull] IGraphicsPipelineContextFactory contextFactory)
        {
            Aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public void Process(GameTime gameTime, IReadOnlyCollection<IGraphicComponent> components)
        {
            var componentsAggregatedByPasses = Aggregator.Aggregate(Steps, components);

            using (var context = ContextFactory.Create())
            {
                foreach (var grouping in componentsAggregatedByPasses)
                {
                    var pass = grouping.Key;
                    var targetComponents = grouping.Value;

                    try
                    {
                        ProcessPass(pass, gameTime, targetComponents, context);
                    }
                    catch (Exception e)
                    {
                        //TODO
                    }
                }
            }
        }

        private void ProcessPass(IGraphicsPass pass, GameTime gameTime,
            IReadOnlyCollection<IGraphicComponent> componentsForThisPass, IGraphicsPipelineContext context)
        {
            pass.Prepare(context);
            pass.Process(gameTime, componentsForThisPass);
            pass.End();
        }
    }
}
