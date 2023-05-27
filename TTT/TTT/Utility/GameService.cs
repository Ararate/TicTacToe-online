using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TTT.Data;
using TTT.Globals;
using TTT.Hubs;
using TTT.Models;
using TTT.Utility;

namespace TTT.Services
{
    public class GameService
    {
        private readonly GamesData _gamesData;
        private readonly IHubContext<GameHub> _hub;
        private readonly DataBase _db;

        public GameService(IHubContext<GameHub> hubContext, DataBase db, GamesData games)
        {
            _hub = hubContext;
            _db = db;
            _gamesData = games;
        }

        public ServiceResponse Create(string hostName)
        {
            if (_gamesData.Games.FirstOrDefault(x => x.Host == hostName || x.Guest == hostName) != null)
                return new ServiceResponse(ResponseType.BadRequest, "Комната уже создана");
            Game newGame = new(hostName);
            newGame.Timeout.Elapsed += (o,e) => _gamesData.Games.Remove(newGame);
            _gamesData.Games.Add(newGame);
            return new ServiceResponse(ResponseType.Success);
        }

        public async Task<ServiceResponse> Delete(string hostName)
        {
            Game? game = _gamesData.Games.FirstOrDefault(x => x.Host == hostName);
            if (game == null) 
                return new ServiceResponse(ResponseType.NotFound, "Комната не найдена");

            _gamesData.Games.Remove(game);
            await TrySendAsync(game.Guest, LC.HubMethodGetData, new GameDTO() { Message = "Игра удалена" });
            return new ServiceResponse(ResponseType.Success);
        }

        public List<Game> GetRooms()
        {
            return _gamesData.Games;
        }

        public async Task<ServiceResponse> Join(string hostName, string guestName)
        {
            Game? game = _gamesData.Games.FirstOrDefault(x => x.Host == hostName);
            if (game == null)
                return new ServiceResponse(ResponseType.NotFound, "Комната отсутствует");
            if (game.Guest != "")
                return new ServiceResponse(ResponseType.BadRequest, "Комната занята");
            if (_gamesData.Games.FirstOrDefault(x => x.Guest == guestName || x.Host == guestName) != null)
                return new ServiceResponse(ResponseType.BadRequest, "Вы уже участвуете в игре");

            game.Guest = guestName;
            Random rand = new();
            game.CurrentMover = rand.Next(1, 3) == 1 ? game.Guest : game.Host;
            await TrySendAsync(game.Host, LC.HubMethodGetGuest, new GameDTO() { Mover = game.CurrentMover });
            return new ServiceResponse(ResponseType.Success, data: game);
        }

        public async Task<ServiceResponse> Move(string moverName, int x, int y)
        {
            IEnumerable<Game> games = _gamesData.Games.Where(x => x.Host == moverName || x.Guest == moverName);
            if (!games.Any())
                return new ServiceResponse(ResponseType.NotFound, "Вы не состоите в игре");
            if (games.Count() > 1 || games.First().Host == games.First().Guest)
                return new ServiceResponse(ResponseType.ServerError, "Возникла неизвестная ошибка");

            Game game = games.First();
            if (game.CurrentMover != moverName)
                return new ServiceResponse(ResponseType.BadRequest, "Сейчас не ваш ход");
            if (x > game.Field.GetUpperBound(0) || x<0 || y> game.Field.GetUpperBound(1) || y<0)
                return new ServiceResponse(ResponseType.BadRequest, "Неверные координаты");

            char symbol = game.MoveCount % 2 == 0 ? LC.O : LC.X;
            if (game.Field[x, y] != ' ')
                return new ServiceResponse(ResponseType.BadRequest, "Клетка уже занята");
            game.Field[x,y] = symbol;

            bool mainDiag = game.Field.GetMainDiag(x, y).Has3InRow();
            bool antiDiag = game.Field.GetAntiDiag(x, y).Has3InRow();
            bool row = game.Field.GetRow(x).Has3InRow();
            bool col = game.Field.GetColumn(y).Has3InRow();

            if (row || col || mainDiag || antiDiag)
            {
                GameResult results = new()
                {
                    Draw = false,
                    GuestName = game.Guest,
                    HostName = game.Host,
                    Winner = game.CurrentMover
                };
                _db.Add(results);
                _db.SaveChanges();
                _gamesData.Games.Remove(game);
                await TrySendAsync(game.Guest, LC.HubMethodFinish, new GameDTO() { GameResult = results });
                await TrySendAsync(game.Host, LC.HubMethodFinish, new GameDTO() { GameResult = results });
                return new ServiceResponse(ResponseType.Success, data: game);
            }

            game.MoveCount++;
            if (game.MoveCount == 9)
            {
                GameResult results = new()
                {
                    Draw = true,
                    GuestName = game.Guest,
                    HostName = game.Host
                };
                _db.Add(results);
                _db.SaveChanges();
                _gamesData.Games.Remove(game);
                await TrySendAsync(game.Guest, LC.HubMethodFinish, new GameDTO() { GameResult = results });
                await TrySendAsync(game.Host, LC.HubMethodFinish, new GameDTO() { GameResult = results });
                return new ServiceResponse(ResponseType.Draw, data: game);
            }
            game.CurrentMover = moverName == game.Host ? game.Guest : game.Host;
            await TrySendAsync(game.CurrentMover, LC.HubMethodGetData, 
                new GameDTO() { 
                  X = x, 
                  Y = y, 
                });
            return new ServiceResponse(ResponseType.Continue, data: game);
        }

        private async Task TrySendAsync(string receiver, string method, GameDTO data)
        {
            if (_gamesData.Connections.TryGetValue(receiver, out _))
                await _hub.Clients.Client(_gamesData.Connections[receiver])
                    .SendAsync(method, data);
        }
    }
}
