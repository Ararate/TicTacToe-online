using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TTT.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        
        public async Task JoinGame(string hostName, string guestName)
        {
            await Clients.User(hostName).SendAsync("WaitGuest", guestName);
        }
        public async Task Move(int x, int y)
        {

        }
    }
}
