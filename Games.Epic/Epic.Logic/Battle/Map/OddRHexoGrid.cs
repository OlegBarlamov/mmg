using System;
using Epic.Core;

namespace Epic.Logic.Battle.Map
{
    public class OddRHexoGrid
    {
        public static int Distance(IBattlePositioned p1, IBattlePositioned p2)
        {
            return Distance(p1.Row, p1.Column, p2.Row, p2.Column);
        }
        
        public static int Distance(HexoPoint p1, HexoPoint p2)
        {
            return Distance(p1.R, p1.C, p2.R, p2.C);
        }
        
        public static int Distance(int r1, int c1, int r2, int c2)
        {
            var axial1 = ToAxial(r1, c1);
            var axial2 = ToAxial(r2, c2);
            return DistanceAxial(axial1.q, axial1.r, axial2.q, axial2.r);
        }
        
        public static int DistanceAxial(int q1, int r1, int q2, int r2)
        {
            return (Math.Abs(q1 - q2) + Math.Abs(r1 - r2) + Math.Abs((q1 + r1) - (q2 + r2))) / 2;
        }
        
        public static (int q, int r) ToAxial(int row, int col)
        {
            int q = col - (int)Math.Floor((row - (row & 1)) / 2.0);
            int r = row;
            return (q, r);
        }
    }
}