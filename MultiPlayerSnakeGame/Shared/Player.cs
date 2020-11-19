using System;

namespace MultiPlayerSnakeGame.Shared
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public DateTime JoinedOn { get; set; }
        public Snake Snake { get; set; }
    }
}
