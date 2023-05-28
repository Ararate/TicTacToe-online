using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TTT
{
    public static class LC
    {
        public const string JwtIssuer = "TicTacToeApi";
        public const string JwtAudience = "Client";
        public const string JwtSecretKey = @"oxmskwi4838_*&^/\%";

        public const char O = 'O';
        public const char X = 'X';

        public const string HubMethodGetData = "GetGameData";//(X,Y)
        public const string HubMethodFinish = "FinishGame";//(Results)
        public const string HubMethodGetGuest = "GetGuest";//(Mover)
    }
}
