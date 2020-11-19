using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerSnakeGame.Shared
{
    public class PlayerPlayedAction : GameActionBase
    {
        public new Play Payload { get => base.Payload as Play; set => base.Payload = value; }
    }
}
