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

            Position.AddLast(newLocation);

            LastPosition = new List<Point> { Position.First.Value };
            
            Position.RemoveFirst();

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
