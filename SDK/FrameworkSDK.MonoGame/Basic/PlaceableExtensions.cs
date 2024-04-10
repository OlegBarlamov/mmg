namespace FrameworkSDK.MonoGame.Basic
{
    public static class PlaceableExtensions
    {
        public static void CopyWorldParameters(this IPlaceable3D placeable, IPlaceable3D targetPlaceableObject)
        {
            placeable.SetPosition(targetPlaceableObject.Position);
            placeable.Scale = targetPlaceableObject.Scale;
            placeable.Rotation = targetPlaceableObject.Rotation;
        }
    }
}