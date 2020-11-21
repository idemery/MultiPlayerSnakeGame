using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MultiPlayerSnakeGame.Server.Engine;
using MultiPlayerSnakeGame.Shared;

namespace MultiPlayerSnakeGame.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly EngineService engine;
        public GameHub(EngineService engine)
        {
            this.engine = engine;
        }

        public async Task PlayerJoinedAsync(PlayerJoinedAction action)
        {

            await engine.HandlePlayerJoinedAsync(this, action);
        }

        public async Task PlayerLeftAsync(PlayerLeftAction action)
        {
            await engine.HandlePlayerLeftAsync(this, action);
        }

        public async Task GameActionAsync(PlayerPlayedAction action)
        {
            await engine.HandleGameActionAsync(this, action);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await engine.HandlePlayerLeftAsync(this, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
