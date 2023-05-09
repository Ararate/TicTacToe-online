namespace TTT
{
    public static class LC
    {
        public const string JwtIssuer = "TicTacToeApi";
        public const string JwtAudience = "Client";
        public const string JwtSecretKey = @"oxmskwi4838_*&^/\%";

        public const char O = '0';
        public const char X = 'X';

        public const string DrawStatus = "Ничья";
        public const string LostStatus = "Проигрыш";
        public const string WinStatus = "Победа";
        public const string ErrorStatus = "Ошибка";
        public const string TimeoutStatus = "Время ожидания превышено";
        public const string BeginStatus = "Соперник найден";
    }
}
