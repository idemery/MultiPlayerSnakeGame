using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerSnakeGame.Shared
{
    public class GameActionBase : IGameAction
    {
        public Player Player { get; set; }
        public string GameId { get; set; }
        public DateTime On { get; set; }
        public virtual object Payload { get; set; }
        public string ConnectionId { get; set; }
    }
}
