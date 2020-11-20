using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using MultiPlayerSnakeGame.Shared;

namespace MultiPlayerSnakeGame.Client
{
    public class GameClient : IAsyncDisposable
    {
        #region PRIVATE FIELDS
        private readonly string _hubUrl;
        private HubConnection _hubConnection;
        private readonly string gameId;
        private readonly Player player;
        #endregion

        #region CONSTRUCTOR
        public GameClient(string gameId, Player player, string siteUrl)
        {
            if (string.IsNullOrWhiteSpace(gameId))
                throw new ArgumentNullException(nameof(gameId));
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            if (string.IsNullOrWhiteSpace(siteUrl))
                throw new ArgumentNullException(nameof(siteUrl));

            this.gameId = gameId;
            this.player = player;
            _hubUrl = siteUrl.TrimEnd('/') + Constants.HUB_URL;
        }
        #endregion

        #region PUBLIC PROPERTIES
        public bool Connected { get => _hubConnection?.State == HubConnectionState.Connected; }
        public string ConnectionId { get => _hubConnection?.ConnectionId; }
        #endregion

        #region EVENTS
        public event MessageReceivedEventHandler MessageReceived;
        public event StateChangedEventHandler StateChanged;
        public event NewEggFoundEventHandler NewEggFound;
        #endregion    

        #region PUBLIC METHODS
        public async Task StartAsync()
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder().WithUrl(_hubUrl).Build();
                _hubConnection.ServerTimeout = TimeSpan.FromHours(1);
                _hubConnection.Closed += _hubConnection_Closed;
                _hubConnection.Reconnected += _hubConnection_Reconnected;
                _hubConnection.Reconnecting += _hubConnection_Reconnecting;

                _hubConnection.On<PlayerJoinedAction>(Constants.GAME_PLAYER_JOINED_CALLBACK, HandleGameActionAsync);
                _hubConnection.On<PlayerLeftAction>(Constants.GAME_PLAYER_LEFT_CALLBACK, HandleGameActionAsync);
                _hubConnection.On<PlayerPlayedAction>(Constants.GAME_ACTION_CALLBACK, HandleGameActionAsync);
                _hubConnection.On<Point>(Constants.GAME_NEW_EGG, HandleNewEggFoundAsync);

                await _hubConnection.StartAsync();

                await _hubConnection.SendAsync(Constants.GAME_PLAYER_JOINED, new PlayerJoinedAction { GameId = gameId, Payload = new List<Player> { player }, ConnectionId = _hubConnection.ConnectionId, On = DateTime.Now });
            }
            else if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                await _hubConnection.SendAsync(Constants.GAME_PLAYER_JOINED, new PlayerJoinedAction { GameId = gameId, Payload = new List<Player> { player }, ConnectionId = _hubConnection.ConnectionId, On = DateTime.Now });
            }
        }

        public async Task PlayAsync(Play play)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync(Constants.GAME_ACTION, new PlayerPlayedAction { GameId = gameId, Payload = play, ConnectionId = _hubConnection.ConnectionId, On = DateTime.Now });
            }
        }

        public async Task StopAsync()
        {
            if (_hubConnection != null && _hubConnection.State != HubConnectionState.Disconnected)
            {
                await _hubConnection.SendAsync(Constants.GAME_PLAYER_LEFT, new PlayerLeftAction { GameId = gameId, Payload = new List<Player> { player }, ConnectionId = _hubConnection.ConnectionId, On = DateTime.Now });
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }
        #endregion

        #region EVENT HANDLERS
        private async Task _hubConnection_Reconnecting(Exception arg)
        {
            StateChanged?.Invoke(this, new StateChangedEventArgs(HubConnectionState.Reconnecting, arg?.Message));
        }

        private async Task _hubConnection_Reconnected(string arg)
        {
            StateChanged?.Invoke(this, new StateChangedEventArgs(HubConnectionState.Connected, arg));
        }

        private async Task _hubConnection_Closed(Exception arg)
        {
            StateChanged?.Invoke(this, new StateChangedEventArgs(HubConnectionState.Disconnected, arg?.Message));
        }

        private async Task HandleGameActionAsync(IGameAction action)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(action));

            // await Task.Factory.FromAsync( ( asyncCallback, @object ) => 
            // this.MessageReceived.BeginInvoke( this, new MessageReceivedEventArgs(action), asyncCallback, @object ), this.MessageReceived.EndInvoke, null );
        }

        private async Task HandleNewEggFoundAsync(Point egg)
        {
            NewEggFound?.Invoke(this, new NewEggFoundEventArgs(egg));
        }
        #endregion
    }

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
    public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);
    public delegate void NewEggFoundEventHandler(object sender, NewEggFoundEventArgs e);

    public class NewEggFoundEventArgs : EventArgs
    {
        public NewEggFoundEventArgs(Point egg)
        {
            Egg = egg;
        }

        public Point Egg { get; init; }

    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(IGameAction action)
        {
            Action = action;
        }

        public IGameAction Action { get; init; }

    }

    public class StateChangedEventArgs : EventArgs
    {
        public StateChangedEventArgs(HubConnectionState state, string message)
        {
            this.State = state;
            this.Message = message;
        }

        public HubConnectionState State { get; init; }
        public string Message { get; init; }
    }
}
