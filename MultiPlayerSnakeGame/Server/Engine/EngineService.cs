using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MultiPlayerSnakeGame.Shared;

namespace MultiPlayerSnakeGame.Server.Engine
{
    public class EngineService
    {
        private static readonly List<Game> games = new List<Game>();
        //ConcurrentDictionary<string, object> cond = new ConcurrentDictionary<string, object>();
        public async Task HandleGameActionAsync(Hub hub, PlayerPlayedAction action)
        {
            action.ConnectionId = hub.Context.ConnectionId;
            var game = games.Find(g => g.Id == action.GameId);
            if (game == default(Game)) return;

            action.Player = game.Players.Find(p => p.Id == hub.Context.ConnectionId);

            bool moved = false;
            switch (action.Payload.KeyCode)
            {
                case "ArrowLeft":
                    moved = action.Player.Snake.GoLeft();
                    break;
                case "ArrowUp":
                    moved = action.Player.Snake.GoUp();
                    break;
                case "ArrowRight":
                    moved = action.Player.Snake.GoRight();
                    break;
                case "ArrowDown":
                    moved = action.Player.Snake.GoDown();
                    break;
                default:
                    break;
            }

            if (moved)
            {
                bool hit = false;
                foreach (var opponent in game.Players)
                {
                    if (opponent.Id == action.ConnectionId)
                    {
                        if (opponent.Snake.Position.Where(po => !po.Equals(opponent.Snake.Position.Last.Value)).Any(p => p.Equals(action.Player.Snake.Position.Last.Value)))
                        {
                            hit = true;
                            break;
                        }
                    }
                    else
                    {
                        if (opponent.Snake.Position.Any(p => p.Equals(action.Player.Snake.Position.Last.Value)))
                        {
                            hit = true;
                            break;
                        }
                    } 
                }

                if (hit)
                {
                    hub.Context.Abort();
                }
                else
                {
                    await hub.Clients.Group(action.GameId).SendAsync(Constants.GAME_ACTION_CALLBACK, action);
                }
            }
            else
            {
                hub.Context.Abort();
            }
        }

        public async Task HandlePlayerJoinedAsync(Hub hub, PlayerJoinedAction action)
        {

            Player player = action.Payload.First();

            player.Id = hub.Context.ConnectionId;

            if (games.Any(g => g.Id == action.GameId))
            {
                var game = games.Find(g => g.Id == action.GameId);
                game.Join(player);
                action.Payload = game.Players;
            }
            else
            {
                try
                {
                    Game newGame = new Game(action.GameId, player);
                    games.Add(newGame);
                }
                catch (Exception ex)
                {

                    throw;
                }
                
            }

            await hub.Groups.AddToGroupAsync(hub.Context.ConnectionId, action.GameId);

            action.Player = games.Find(g => g.Id == action.GameId)?.Players.Find(p => p.Id == hub.Context.ConnectionId);

            await hub.Clients.Group(action.GameId).SendAsync(Constants.GAME_PLAYER_JOINED_CALLBACK, action);
        }
        public async Task HandlePlayerLeftAsync(Hub hub, PlayerLeftAction action)
        {
            Player player = action.Payload.First();

            player.Id = hub.Context.ConnectionId;

            if (games.Any(g => g.Id == action.GameId))
            {
                var game = games.Find(g => g.Id == action.GameId);
                game.Leave(player);
                action.Payload = game.Players;
            }

            await hub.Groups.RemoveFromGroupAsync(hub.Context.ConnectionId, action.GameId);

            action.Player = games.Find(g => g.Id == action.GameId)?.Players.Find(p => p.Id == hub.Context.ConnectionId);

            await hub.Clients.Group(action.GameId).SendAsync(Constants.GAME_PLAYER_LEFT_CALLBACK, action);
        }

        public async Task HandlePlayerLeftAsync(Hub hub, string connectionId)
        {
            Game game = games.Find(g => g.Players.Any(p => p.Id == connectionId));
            if (game == default(Game)) return;

            Player player = game.Players.Find(p => p.Id == connectionId);
            if (player == default(Player)) return;

            await HandlePlayerLeftAsync(hub, new PlayerLeftAction { GameId = game.Id, On = DateTime.Now, Payload = new List<Player> { player } });
        }
    }
}
