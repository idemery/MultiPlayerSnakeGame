namespace MultiPlayerSnakeGame.Shared
{
    public class PlayerPlayedAction : GameActionBase
    {
        public new Play Payload { get => base.Payload as Play; set => base.Payload = value; }
    }
}
