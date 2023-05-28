namespace TTT.Models
#nullable disable
{
    public class GameDTO
    {
        public char[,] Field;
        public string HostName { get; set; }
        public string Message { get; set; }
        public string Mover { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public GameResult GameResult { get; set; }
        public GameDTO()
        {
        }

    }
}
