using System;
using System.Collections.Generic;

namespace TTT.Models
{
    public class GamesData
    {
        public readonly List<Game> Games = new List<Game>();
        public readonly Dictionary<string, string> Connections = new();
    }
}
