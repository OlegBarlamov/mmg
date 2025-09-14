using Epic.Core.Services.BattleObstacles;
using JetBrains.Annotations;

namespace Epic.Server.Resources;

public class BattleObstacleResource
{
    public int Column { get; }
    public int Row { get; }
    public int Width { get; }
    public int Height { get; }
    public bool[][] Mask { get; }

    public BattleObstacleResource(IBattleObstacleObject obstacleObject)
    {
        Column = obstacleObject.Column;
        Row = obstacleObject.Row;
        Width = obstacleObject.Width;
        Height = obstacleObject.Height;
        Mask = GetMask(obstacleObject.Mask);
    }

    private static bool[][] GetMask(bool[,] mask)
    {
        var result = new bool[mask.GetLength(0)][];
        for (int i = 0; i < mask.GetLength(0); i++)
        {
            result[i] = new bool[mask.GetLength(1)];
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                result[i][j] = mask[i, j];
            }
        }
        return result;
    }
}