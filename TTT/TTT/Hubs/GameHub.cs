using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTT.Models;
using TTT.Utility;

namespace TTT.Hubs
{
    [SignalRHub]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GameHub : Hub
    {
        private readonly GamesData _gamesData;
        private readonly JwtService _jwt;
        public GameHub(GamesData games, JwtService jwt)
        {
            _gamesData = games;
            _jwt = jwt;
        }

        public void GetConnectionId(string accessToken)
        {
            string? name = _jwt.GetName(accessToken);
            if (name == null || Context.ConnectionId==null) return;
            Game? game = _gamesData.Games.FirstOrDefault(x => x.Host == name);
            if (game != null)
                game = _gamesData.Games.FirstOrDefault(x => x.Guest == name);
            _gamesData.Connections.Add(name, Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string key = _gamesData.Connections.FirstOrDefault(x=>x.Value==Context.ConnectionId).Key;
            _gamesData.Connections.Remove(key);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
