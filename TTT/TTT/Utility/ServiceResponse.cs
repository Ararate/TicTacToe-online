using TTT.Globals;
using TTT.Models;

namespace TTT.Utility
{
    public class ServiceResponse
    {
        public ServiceResponse(ResponseType status, string? message = null, Game? data = null)
        {
            Message = message;
            Status = status;
            Data = data;
        }
        public string? Message { get; set; }
        public ResponseType Status { get; set; }
        public Game? Data { get; set; } = null;
    }
}
