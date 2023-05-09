namespace TTT.Models
#nullable disable
{
    public class GameDTO
    {
        public string Message { get; set; }
        public string Opponent { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public GameDTO(int x, int y, string status = null, string opponent = null)
        {
            X = x;
            Y = y;
            Message = status;
            Opponent = opponent;
        }
        public GameDTO(string message)
        {
            Message = message;
        }

    }
}
