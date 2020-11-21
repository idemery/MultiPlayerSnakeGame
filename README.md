# Multiplayer Snake Game
A multiplayer snake game using .NET 5 Blazor and SignalR that supports rooms and multiple games.

![Multiplayer Snake Game](./MultiplayerSnakeGame.gif)

## Introduction
This project is a way to learn Blazor and SignalR, and maybe to show case how using C# instead of Javascript is more easier and simpler. The nuget package [BlazorExtensions Canvas](https://github.com/BlazorExtensions/Canvas) is used for interfacing the HTML canvas.

## How to play
- Run the web application, it's a Blazor hosted on ASP.NET Core so there is 3 projects Client, Server, and Shared. Run the server.
- Click on Start to start playing (press Enter to Start/Stop).
- You can play as single or click on Invite your friends after it appears, it will open another page with a query to the same game you started, you may share this URL with your friends to join the same game.
- Press on the arrow keys and target the black sqaure.
- If you loose by hitting the edges, yourself, or an opponent press Enter or click Start to join again loosing your tail.

## How it works
There is only one razor page in the **Client** project `Index.razor` which listens on `'/{GameId?}'`. The is an optional parameter property and is considered the SignalR `Group` name. The game generates one if not provided.

This index page uses and handles a `GameClient` instance that acts as proxy to the SignalR hub `GameHub` and fires events when the hub calls the client back.

The hub uses an injected `EngineService` singleton which holds a C# `ConcurrentDictionary<string, Game>` where the key string is GameId (group name).

The game value instance holds a list of players where each player has an `Id` (signalr connection id), `Color` (generated randomly by the client when joining the game for the first time), and `Snake`.

The `Snake` instance holds a C# `LinkedList` (good luck with javascript) that holds nodes of Points where the `Point` is X and Y integer properties.

Moving the snake is as simple as `linkedList.AddLast(newLocation); linkedList.RemoveFirst();` and then drawing the list points on the canvas.

