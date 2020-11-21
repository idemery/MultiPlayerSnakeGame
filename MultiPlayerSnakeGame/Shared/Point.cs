using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerSnakeGame.Shared
{
    public class Point
    {
        public Point()
        {

        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            Point point = obj as Point;
            if (point == null) return false;

            return X == point.X && Y == point.Y;
        }

        public override int GetHashCode()
        {
            return int.Parse(X.ToString() + Y.ToString());
        }
    }
}
