using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using System.Threading.Tasks;
using TTT.Models;

namespace TTT.Hubs
{
    [SignalRHub]
    public class GameHub : Hub
    {
        [SignalRMethod]
        public GameDTO GetGuest(GameDTO dto)
        {
            return dto;
        }
    }
}
