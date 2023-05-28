using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TTT.Data;
using TTT.Models;
using TTT.Utility;
#nullable disable
namespace TTT.Controllers
{
    /// <summary>
    /// Авторизация и аутентификация
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : Controller
    {
        private readonly DataBase _context;
        private readonly JwtService _jwt;
        private readonly MD5Encoding _md5;
        public PlayerController(DataBase context, JwtService jwt, MD5Encoding md5Encoding)
        {
            _context = context;
            _jwt = jwt;
            _md5 = md5Encoding;
        }
        /// <summary>
        /// Зарегистрировать аккаунт
        /// </summary>
        /// <param name="regData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(PlayerDTO regData)
        {
            if (!ModelState.IsValid) 
                return BadRequest(regData);
            if (await _context.Players.FirstOrDefaultAsync(x => x.Name == regData.Name) != null)
                return BadRequest("Имя занято");
            regData.Password = _md5.Encode(regData.Password);
            await _context.Players.AddAsync(new Player(regData) { RefreshExpireTime = DateTime.Now, RefreshToken=""});
            await _context.SaveChangesAsync();
            return Ok();
        }
        /// <summary>
        /// Войти в аккаунт и получить токены
        /// </summary>
        /// <param name="regData"></param>
        /// <returns>{accessToken, refreshToken}</returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(PlayerDTO regData)
        {
            regData.Password = _md5.Encode(regData.Password);
            Player player= await _context.Players.FirstOrDefaultAsync(x => x.Name == regData.Name && x.Password == regData.Password);
            if (player == null)
                return BadRequest("Неверные пароль или имя");
            string refToken = _jwt.GetRefreshToken();
            string token = _jwt.GetToken(regData.Name);
            player.RefreshToken = refToken;
            player.RefreshExpireTime = DateTime.Now.AddDays(7);
            _context.SaveChanges();
            return Ok(new { accessToken = token, refreshToken = refToken });
        }
        /// <summary>
        /// Обновить токены 
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>{accessToken, refreshToken}</returns>
        [HttpPut]
        [Route("RefreshTokens")]
        public IActionResult RefreshTokens(TokensDTO tokens)
        {
            string name = _jwt.GetName(tokens.accessToken);
            Player player = _context.Players.SingleOrDefault(x => x.Name == name);
            if (player == null || tokens.refreshToken != player.RefreshToken || player.RefreshExpireTime <= DateTime.Now)
                return BadRequest();
            string newToken = _jwt.GetToken(name);
            string newRefresh = _jwt.GetRefreshToken();

            player.RefreshToken = newRefresh;
            player.RefreshExpireTime = DateTime.Now.AddDays(7);
            _context.SaveChanges();
            return Ok(new { accessToken = newToken, refreshToken = newRefresh });
        }
    }
}
