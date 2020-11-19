using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerSnakeGame.Shared
{
    public interface IGameAction
    {
        Player Player { get; set; }
        string GameId { get; set; }
        string ConnectionId { get; set; }
        DateTime On { get; set; }
        object Payload { get; set; }
    }
}
