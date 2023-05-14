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
    [Route("[controller]")]
    public class GameController : Controller
    {
        private readonly GameService _gameService;
        
        public GameController(GameService gameService, IHubContext<GameHub> hubContext) 
        {
            _gameService = gameService;
        }
        [Authorize]
        [HttpPost]
        [Route("CreateGame")]
        public IActionResult CreateGame() 
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return BadRequest("Не авторизован");

            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            ServiceResponse resp= _gameService.Create(name);
            if(resp.Status == ResponseType.NotFound || resp.Status == ResponseType.ServerError || resp.Status == ResponseType.BadRequest)
                return BadRequest(resp.Message);

            return Ok("Игра создана, ожидайте соперника");
        }
        [Authorize]
        [HttpDelete]
        [Route("DeleteGame")]
        public async Task<IActionResult> DeleteGame()
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = await _gameService.Delete(name);
            if (resp.Status == ResponseType.NotFound || resp.Status == ResponseType.ServerError || resp.Status == ResponseType.BadRequest)
                return BadRequest(resp.Message);
            return Ok();
        }
        [HttpGet]
        [Route("GetRooms")]
        public IActionResult GetRooms()
        {
            return Ok(_gameService.GetRooms().Where(x=> x.Guest == null).Select(x=>new {hostName = x.Host}));
        }

        [Authorize]
        [HttpGet]
        [Route("JoinRoom")]
        public async Task<IActionResult> JoinRoom(string hostName)
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = await _gameService.Join(hostName, name);
            if (resp.Status == ResponseType.NotFound || resp.Status == ResponseType.ServerError || resp.Status == ResponseType.BadRequest)
                return BadRequest(resp.Message);
            return Ok(new { Opponent = resp.Data.Host });
        }
        [Authorize]
        [HttpPost]
        [Route("MakeMove")]
        public async Task<IActionResult> MakeMove(int x, int y)
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return Unauthorized();
            string name = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            ServiceResponse resp = await _gameService.Move(name, x, y);

            if (resp.Status == ResponseType.NotFound || resp.Status == ResponseType.ServerError || resp.Status == ResponseType.BadRequest)
                return BadRequest(resp.Message);

            return Ok();
        }
    }
}
