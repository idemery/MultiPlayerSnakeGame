using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MultiPlayerSnakeGame.Shared;
using Microsoft.AspNetCore.SignalR;

namespace MultiPlayerSnakeGame.Server.Engine
{
    public class Game
    {
        int snakeSize = 10;
        public Game(string id, Player player)
        {
            Id = id;
            Players = new List<Player>();
            Join(player);
        }

        public string Id { get; init; }
        public List<Player> Players { get; init; }

        static Random random = new Random(0);

        private Point GetRandomLocation()
        {
            // not any random x or y inside canvas, the size of the snake point has to be taken into account
            // for example if snake point size is 10 we dont want to get any random x between 0 or 10 but either 0, 10, or 20, etc..
            var xRange = Enumerable.Range(Constants.SNAKE_SIZE, Constants.CANVAS_WIDTH).Where(x => x % Constants.SNAKE_SIZE == 0).ToList();
            var yRange = Enumerable.Range(Constants.SNAKE_SIZE, Constants.CANVAS_HEIGHT).Where(x => x % Constants.SNAKE_SIZE == 0).ToList();
            int xIndex = random.Next(xRange.Count - 1);
            int yIndex = random.Next(yRange.Count - 1);
            Point randomLocation = new Point(xRange[xIndex], yRange[yIndex]);
            return randomLocation;
        }
        private bool IsValidLocation(Point locaiton)
        {
            return !Players.Any(p => p.Snake.Position.Any(s => s.Equals(locaiton)));
        }

        public void Join(Player player)
        {
            if (!Players.Any(p => p.Id == player.Id))
            {
                Point randomLocation = GetRandomLocation();
                while (!IsValidLocation(randomLocation))
                {
                    randomLocation = GetRandomLocation();
                }

                var position = new LinkedList<Point>();
                position.AddLast(new LinkedListNode<Point>(randomLocation));
                player.Snake = new Snake
                {
                    Color = player.Color,
                    Edge = new Point(Constants.CANVAS_WIDTH, Constants.CANVAS_HEIGHT),
                    Size = snakeSize,
                    InitialLocation = randomLocation,
                    Position = position
                };

                Players.Add(player);
            }
        }

        public void Leave(Player player)
        {
            if (Players.Any(p => p.Id == player.Id))
            {
                Players.Remove(Players.First(p => p.Id == player.Id));
            }
        }
    }
}
