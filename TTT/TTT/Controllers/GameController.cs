using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
#nullable disable
namespace TTT.Controllers
{
    /// <summary>
    /// Игра
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService, IHubContext<GameHub> hubContext)
        {
            _gameService = gameService;
        }
        /// <summary>
        /// Создать игру. При подключении гостя вызывает signalr метод GetGuest({string guestName})
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("Create")]
        public IActionResult Create()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity == null)
                return BadRequest("Не авторизован");

            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            ServiceResponse resp = _gameService.Create(name);
            if (resp.Status == ResponseType.NotFound || resp.Status == ResponseType.ServerError || resp.Status == ResponseType.BadRequest)
                return BadRequest(resp.Message);

            return Ok("Игра создана, ожидайте соперника");
        }
        /// <summary>
        /// Удалить игру
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = await _gameService.Delete(name);
            if (resp.Status == ResponseType.NotFound || resp.Status == ResponseType.ServerError || resp.Status == ResponseType.BadRequest)
                return BadRequest(resp.Message);
            return Ok();
        }
        /// <summary>
        /// Получить список свободных комнат
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRooms")]
        public IActionResult GetRooms()
        {
            return Ok(_gameService.GetRooms().Where(x => x.Guest == "").Select(x => new { hostName = x.Host }));
        }
        /// <summary>
        /// Получить поле игры
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("GetField")]
        public IActionResult GetField()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            Game game = _gameService.GetRooms().FirstOrDefault(x => x.Host == name || x.Guest == name);
            if (game == null)
                return BadRequest("Игры не существует");
            char[,] field = game.Field;
            char[][] res = new char[field.GetLength(0)][];
            for (int i = 0; i < field.GetLength(0); i++)
            {
                res[i] = new char[field.GetLength(1)];
                for (int j = 0; j < field.GetLength(1); j++)
                    res[i][j] = field[i, j];
            }
            return Ok(res);
        }
        /// <summary>
        /// Присоединиться к игре
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("JoinRoom")]
        public async Task<IActionResult> JoinRoom(GameDTO dto)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = await _gameService.Join(dto.HostName, name);
            if (resp.Status == ResponseType.NotFound || resp.Status == ResponseType.ServerError || resp.Status == ResponseType.BadRequest)
                return BadRequest(resp.Message);
            return Ok(new { Mover = resp.Data.CurrentMover });
        }
        /// <summary>
        /// Сделать ход. Отправляет оппоненту signalr метод GetGameData({int x,int y}).
        /// Если игра завершена, отправляет обоим игрокам signalr метод FinishGame({ gameResults {bool draw, string winner}})
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("MakeMove")]
        public async Task<IActionResult> MakeMove(GameDTO dto)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = await _gameService.Move(name, dto.X, dto.Y);

            if (resp.Status == ResponseType.NotFound || resp.Status == ResponseType.ServerError || resp.Status == ResponseType.BadRequest)
                return BadRequest(resp.Message);

            return Ok();
        }
    }
}