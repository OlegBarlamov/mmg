using Epic.Data.EffectType;

namespace Epic.Core.ServerMessages
{
    public class EffectAnimationCommandFromServer : BaseCommandFromServer
    {
        public override string Command => "EFFECT_ANIMATION";

        public string EffectSpriteUrl { get; }
        public EffectAnimation AnimationType { get; }
        public int TargetRow { get; }
        public int TargetColumn { get; }
        public string SourceUnitId { get; }
        public string SourcePlayer { get; }
        public int AnimationTimeMs { get; }

        public EffectAnimationCommandFromServer(
            int turnNumber,
            string effectSpriteUrl,
            EffectAnimation animationType,
            int targetRow,
            int targetColumn,
            string sourceUnitId,
            string sourcePlayer,
            int animationTimeMs)
            : base(turnNumber)
        {
            EffectSpriteUrl = effectSpriteUrl ?? string.Empty;
            AnimationType = animationType;
            TargetRow = targetRow;
            TargetColumn = targetColumn;
            SourceUnitId = sourceUnitId;
            SourcePlayer = sourcePlayer;
            AnimationTimeMs = animationTimeMs;
        }
    }
}
