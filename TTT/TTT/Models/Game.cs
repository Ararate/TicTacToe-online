using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TTT.Models
{
	public class Game
	{
		public Game(string host)
		{
			Field = new char[3,3];
			for (int i = 0; i <  Field.GetLength(0); i++)
				for (int j = 0; j < Field.GetLength(1); j++)
					Field[i, j] = ' ';
			Host = host;
			Timeout = new(TimeSpan.FromMinutes(5).TotalMilliseconds);
        }
		public string Host { get; set; }
		public string Guest { get; set; } = string.Empty;
        public string? CurrentMover {
			get => currentMover; 
            set { 
				Timeout.Stop(); 
				Timeout.Start(); 
				currentMover = value;
			} }
		private string? currentMover;

        public int MoveCount { get; set; }
		public char[,] Field { get; set; }
		public Timer Timeout { get; set; }
    }
}
