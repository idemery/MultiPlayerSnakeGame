using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerSnakeGame.Shared
{
    public static class Constants
    {
        public const string HUB_URL = "/snakehub";
        public const string GAME_PLAYER_JOINED = "PlayerJoinedAsync";
        public const string GAME_PLAYER_JOINED_CALLBACK = "PlayerJoinedCallback";

        public const string GAME_PLAYER_LEFT = "PlayerLeftAsync";
        public const string GAME_PLAYER_LEFT_CALLBACK = "PlayerLeftCallback";

        public const string GAME_ACTION = "GameActionAsync";
        public const string GAME_ACTION_CALLBACK = "GameActionCallback";

        public const string GAME_PLAYER_LOST = "YouLost";
        public const string GAME_PLAYER_WON = "YouWon";
        public const string GAME_NEW_EGG = "NewEgg";

        public const string PING_TEST = "PingTest";
        public const string PONG_TEST = "PongTest";

        public const int CANVAS_WIDTH = 500;
        public const int CANVAS_HEIGHT = 500;
        public const int SNAKE_SIZE = 10;
        public const int TIMER_ELAPSE = 100;
    }
}
