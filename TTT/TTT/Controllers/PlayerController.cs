using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TTT.Data;
using TTT.Models;
using TTT.Utility;

namespace TTT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : Controller
    {
        private readonly Context _context;
        private readonly JwtGenerator _jwtGenerator;
        private readonly MD5Encoding _md5;
        public PlayerController(Context context, JwtGenerator generator, MD5Encoding md5Encoding)
        {
            _context = context;
            _jwtGenerator = generator;
            _md5 = md5Encoding;
        }
        [HttpPost]
        public async Task<IActionResult> Register(PlayerDTO regData)
        {
            if (!ModelState.IsValid) 
                return BadRequest(regData);
            if (await _context.Players.FirstOrDefaultAsync(x => x.Name == regData.Name) != null)
                return BadRequest("Имя занято");
            regData.Password = _md5.Encode(regData.Password);
            await _context.Players.AddAsync(new Player(regData));
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Login(PlayerDTO regData)
        {
            regData.Password = _md5.Encode(regData.Password);
            if (await _context.Players.FirstOrDefaultAsync(x => x.Name == regData.Name && x.Password == regData.Password) != null)
                return BadRequest("Неверные пароль или имя");
            return Ok(_jwtGenerator.GetToken(regData.Name));
        }
    }
}
