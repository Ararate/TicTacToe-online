using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Threading.Tasks;
using TTT.Data;
using TTT.Globals;
using TTT.Hubs;
using TTT.Models;
using TTT.Services;
using TTT.Utility;

namespace TTT.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class GameController : Controller
    {
        private readonly GameService _gameService;
        private readonly Context _db;
        private readonly IHubContext<GameHub> _hubContext;
        public GameController(GameService gameService, Context context, IHubContext<GameHub> hubContext) 
        {
            _gameService = gameService;
            _db = context;
            _hubContext = hubContext;
        }
        [Authorize]
        [HttpPost]
        public async Task CreateGame() 
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
            {
                await Response.WriteAsync("Не авторизован");
                Response.Body.Flush();
                return;
            }
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            ServiceResponse resp= _gameService.Create(name);
            if (resp.Status == ResponseType.NotFound)
            {
                await Response.WriteAsync(resp.Message);
                Response.Body.Flush();
                return;
            }
            if (resp.Status == ResponseType.BadRequest)
            {
                await Response.WriteAsync(resp.Message);
                Response.Body.Flush();
                return;
            }

            await Response.WriteAsync("Игра создана, ожидайте соперника");
            Response.Body.Flush();
            bool gotAGuest = await _gameService.Wait(resp.Data);
            if (gotAGuest)
                await Response.WriteAsync(LC.BeginStatus);
            else
            {
                await Response.WriteAsync(LC.TimeoutStatus);
                Response.Body.Flush();
                _gameService.Delete(name);
                return;
            }
            await Response.WriteAsJsonAsync(new {Opponent = resp.Data.Guest});

            Response.Body.Flush();
            return;
        }
        [Authorize]
        [HttpDelete]
        public IActionResult DeleteGame()
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = _gameService.Delete(name);
            if (resp.Status == ResponseType.NotFound)
                return NotFound(resp.Message);
            return Ok();
        }
        [HttpGet]
        public IActionResult GetRooms()
        {
            return Ok(_gameService.GetRooms().Where(x=> x.Guest == null).Select(x=>new {hostName = x.Host}));
        }

        [Authorize]
        [HttpGet]
        public IActionResult JoinRoom(string hostName)
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = _gameService.Join(hostName, name);
            return Ok(new { Opponent = resp.Data.Host });
        }
        [Authorize]
        [HttpPost]
        public IActionResult MakeMove(int x, int y)
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = _gameService.Move(name, x, y);
            
            switch (resp.Status){
                case ResponseType.NotFound:
                    return NotFound(resp.Message);

                case ResponseType.ServerError:
                    resp.Data.Awaiter.TrySetResult(false);
                    _gameService.Delete(resp.Data.Host);
                    return StatusCode(500);

                case ResponseType.Continue:
                    resp.Data.Awaiter.TrySetResult(false);
                    return Ok(new GameDTO(x,y,opponent: resp.Data.CurrentMover));

                case ResponseType.BadRequest:
                    return BadRequest(resp.Message);

                case ResponseType.Draw:
                    resp.Data.Awaiter.TrySetResult(false);
                    GameResult results = new() {
                        Draw = true,
                        GuestName = resp.Data.Guest,
                        HostName = resp.Data.Host
                    };
                    _db.Add(results);
                    _db.SaveChanges();
                    _gameService.Delete(resp.Data.Host);
                    return Ok(results);

                case ResponseType.Finish:
                    resp.Data.Awaiter.TrySetResult(false);
                    results = new()
                    {
                        Draw = false,
                        GuestName = resp.Data.Guest,
                        HostName = resp.Data.Host,
                        Winner = resp.Data.CurrentMover
                    };
                    _db.Add(results);
                    _db.SaveChanges();
                    _gameService.Delete(resp.Data.Host);
                    return Ok(results);
            }

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task TrackField()
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
            {
                await Response.WriteAsync("Не авторизован");
                Response.Body.Flush();
                return;
            }
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            Response.Headers.Add("Content-Type", "text/event-stream");
            Game? game = _gameService.GetGame(name);
            if (game == null)
            {
                await Response.WriteAsync("Игра не найдена");
                Response.Body.Flush();
                return;
            }
            while (true)
            {
                if(await _gameService.Wait(game))
                {
                    await Response.WriteAsJsonAsync(new GameDTO(game.X.Value, game.Y.Value,opponent: name == game.Host ? game.Guest : game.Host));
                    Response.Body.Flush();
                    return;
                }
                else
                {
                    await Response.WriteAsJsonAsync(new GameDTO(game.X.Value, game.Y.Value ,opponent: name == game.Host ? game.Guest : game.Host));
                    Response.Body.Flush();
                    return;
                }
            }
        }
    }
}
