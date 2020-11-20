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
        private static readonly ConcurrentDictionary<string, Game> games = new ConcurrentDictionary<string, Game>();
        public async Task HandleGameActionAsync(Hub hub, PlayerPlayedAction action)
        {
            action.ConnectionId = hub.Context.ConnectionId;
            var game = games[action.GameId];

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
                        if (action.Player.Snake.Position.Count < 2) continue;

                        var current = opponent.Snake.Position.Last;
                        while (current.Previous != null)
                        {
                            if (current.Previous.Value.Equals(action.Player.Snake.Position.Last.Value))
                            {
                                hit = true;
                                break;
                            }
                            current = current.Previous;
                        }

                        if (hit) break;
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
                    await HandlePlayerLeftAsync(hub, hub.Context.ConnectionId, lost: true);
                }
                else
                {
                    if (action.Player.Snake.Position.Last.Value.Equals(game.Egg))
                    {
                        action.Player.Snake.Eat(game.Egg);
                        await hub.Clients.Group(action.GameId).SendAsync(Constants.GAME_NEW_EGG, game.NewEgg());
                    }

                    await hub.Clients.Group(action.GameId).SendAsync(Constants.GAME_ACTION_CALLBACK, action);
                }
            }
            else
            {
                hub.Context.Abort();
                await HandlePlayerLeftAsync(hub, hub.Context.ConnectionId, lost: true);
            }
        }

        public async Task HandlePlayerJoinedAsync(Hub hub, PlayerJoinedAction action)
        {

            Player player = action.Payload.First();

            player.Id = hub.Context.ConnectionId;


            Game game = games.GetOrAdd(action.GameId, new Game(action.GameId, player));

            game.Join(player);
            action.Payload = game.Players;

            await hub.Groups.AddToGroupAsync(hub.Context.ConnectionId, action.GameId);

            action.Player = games[action.GameId]?.Players.Find(p => p.Id == hub.Context.ConnectionId);

            await hub.Clients.Group(action.GameId).SendAsync(Constants.GAME_PLAYER_JOINED_CALLBACK, action);

            await hub.Clients.Caller.SendAsync(Constants.GAME_NEW_EGG, game.Egg);
        }
        public async Task HandlePlayerLeftAsync(Hub hub, PlayerLeftAction action)
        {
            Player player = action.Payload.First();

            player.Id = hub.Context.ConnectionId;

            var game = games[action.GameId];
            game.Leave(player);
            action.Payload = game.Players;

            await hub.Groups.RemoveFromGroupAsync(hub.Context.ConnectionId, action.GameId);

            action.Player = player;

            await hub.Clients.Group(action.GameId).SendAsync(Constants.GAME_PLAYER_LEFT_CALLBACK, action);
        }

        public async Task HandlePlayerLeftAsync(Hub hub, string connectionId, bool lost = false)
        {
            Game game = games.FirstOrDefault(g => g.Value.Players.Any(p => p.Id == connectionId)).Value;
            if (game == null) return;

            Player player = game.Players.Find(p => p.Id == connectionId);
            game.Leave(player);

            await hub.Groups.RemoveFromGroupAsync(hub.Context.ConnectionId, game.Id);
            await hub.Clients.Group(game.Id).SendAsync(Constants.GAME_PLAYER_LEFT_CALLBACK, new PlayerLeftAction { GameId = game.Id, On = DateTime.Now, Player = player, ConnectionId = hub.Context.ConnectionId, Payload = game.Players, Lost = lost });
        }
    }
}
