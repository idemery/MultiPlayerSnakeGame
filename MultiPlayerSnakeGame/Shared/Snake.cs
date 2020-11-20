using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MultiPlayerSnakeGame.Shared
{
    public class Snake
    {
        public string Color { get; set; }
        public int Size { get; set; }
        public Point Edge { get; set; }
        public Point InitialLocation { get; set; }
        public List<Point> LastPosition { get; set; }
        public LinkedList<Point> Position { get; set; }

        private void Shift()
        {
            if (Position.Count < 2) return;

            var current = Position.Last;

            while (current.Previous != null)
            {
                Point temp = current.Previous.Value;
                current.Previous.Value = current.Value;
                current.Value = temp;
                current = current.Previous;
            }
        }

        private bool IsOutOfEdge(Point location)
        {
            return location.X < 0 || location.X >= Edge.X || location.Y < 0 || location.Y >= Edge.Y;
        }

        private bool Move(Point newLocation)
        {
            if (IsOutOfEdge(newLocation))
            {
                return false;
            }

            Point[] points = new Point[Position.Count];
            Position.CopyTo(points, 0);
            LastPosition = points.ToList();

            Shift();
            Position.Last.Value = new Point(newLocation.X, newLocation.Y);

            return true;
        }

        public bool GoUp()
        {
            Point newLocation = new Point(Position.Last.Value.X, Position.Last.Value.Y - Size);

            return Move(newLocation);
        }
        public bool GoDown()
        {
            Point newLocation = new Point(Position.Last.Value.X, Position.Last.Value.Y + Size);

            return Move(newLocation);
        }
        public bool GoLeft()
        {
            Point newLocation = new Point(Position.Last.Value.X - Size, Position.Last.Value.Y);

            return Move(newLocation);
        }
        public bool GoRight()
        {
            Point newLocation = new Point(Position.Last.Value.X + Size, Position.Last.Value.Y);

            return Move(newLocation);
        }

        public void Eat(Point egg)
        {
            Position.AddBefore(Position.First, LastPosition.First());
            LastPosition.RemoveAt(0);
        }
    }
}
