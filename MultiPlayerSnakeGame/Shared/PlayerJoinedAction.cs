using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerSnakeGame.Shared
{
    public class PlayerJoinedAction : GameActionBase
    {
        public new List<Player> Payload { get => base.Payload as List<Player>; set => base.Payload = value; }
    }
}
