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
        private readonly List<Game> _games;
        private readonly IHubContext<GameHub> _hub;
        private readonly DataBase _db;

        public GameService(IHubContext<GameHub> hubContext, DataBase db, List<Game> games)
        {
            _hub = hubContext;
            _db = db;
            _games = games;
        }

        public ServiceResponse Create(string hostName)
        {
            if (_games.FirstOrDefault(x => x.Host == hostName || x.Guest == hostName) != null)
                return new ServiceResponse(ResponseType.BadRequest, "Комната уже создана");
            Game newGame = new() { 
                Host = hostName 
            };
            _games.Add(newGame);
            return new ServiceResponse(ResponseType.Success);
        }

        public async Task<ServiceResponse> Delete(string hostName)
        {
            Game? game = _games.FirstOrDefault(x => x.Host == hostName);
            if (game == null) 
                return new ServiceResponse(ResponseType.NotFound, "Комната не найдена");

            _games.Remove(game);
            GameDTO dto = new GameDTO() { Message = "Игра удалена" };
            if (!string.IsNullOrEmpty(game.Guest))
                await _hub.Clients.User(game.Guest)
                    .SendAsync(LC.HubMethodGetData, dto);
            return new ServiceResponse(ResponseType.Success);
        }

        public List<Game> GetRooms()
        {
            return _games;
        }

        public async Task<ServiceResponse> Join(string hostName, string guestName)
        {
            Game? game = _games.FirstOrDefault(x => x.Host == hostName);
            if (game == null)
                return new ServiceResponse(ResponseType.NotFound, "Комната отсутствует");
            if (game.Guest != null)
                return new ServiceResponse(ResponseType.BadRequest, "Комната занята");
            if (_games.FirstOrDefault(x => x.Guest == guestName || x.Host == guestName) != null)
                return new ServiceResponse(ResponseType.BadRequest, "Вы уже участвуете в игре");

            game.Guest = guestName;
            Random rand = new();
            game.CurrentMover = rand.Next(1, 2) == 1 ? game.Guest : game.Host;
            await _hub.Clients.User(hostName)
                .SendAsync(LC.HubMethodGetGuest, new GameDTO() { Opponent = guestName });
            return new ServiceResponse(ResponseType.Success, data: game);
        }

        public async Task<ServiceResponse> Move(string moverName, int x, int y)
        {
            IEnumerable<Game> games = _games.Where(x => x.Host == moverName || x.Guest == moverName);
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

            bool mainDiag = Has3InRow(game.Field.GetMainDiag(x, y));
            bool antiDiag = Has3InRow(game.Field.GetAntiDiag(x, y));
            bool row = Has3InRow(game.Field.GetRow(y));
            bool col = Has3InRow(game.Field.GetColumn(x));

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
                await _hub.Clients.User(game.Guest)
                    .SendAsync(LC.HubMethodGetData, new GameDTO() { GameResult = results });
                await _hub.Clients.User(game.Host)
                    .SendAsync(LC.HubMethodGetData, new GameDTO() { GameResult = results });
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
                await _hub.Clients.User(game.Guest)
                    .SendAsync(LC.HubMethodGetData, new GameDTO() { GameResult = results });
                await _hub.Clients.User(game.Host)
                    .SendAsync(LC.HubMethodGetData, new GameDTO() { GameResult = results });
                return new ServiceResponse(ResponseType.Draw, data: game);
            }
            game.CurrentMover = moverName == game.Host ? game.Guest : game.Host;
            await _hub.Clients.User(game.CurrentMover)
                .SendAsync(LC.HubMethodGetData, new GameDTO() { X=x,Y=y,Opponent=moverName });
            return new ServiceResponse(ResponseType.Continue, data: game);
        }

        private bool Has3InRow(char[] row)
        {
            for (int i = 0; i < row.Length - 2; i++)
                if (row[i] == row[i + 1] && row[i] == row[i + 2])
                    return true;
            return false;
        }
    }
}
