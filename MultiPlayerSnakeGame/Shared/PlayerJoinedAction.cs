using System.Collections.Generic;

namespace MultiPlayerSnakeGame.Shared
{
    public class PlayerJoinedAction : GameActionBase
    {
        public new List<Player> Payload { get => base.Payload as List<Player>; set => base.Payload = value; }
    }
}
