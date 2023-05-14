using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace TTT.Models
{
	public class Game
	{
		public Game()
		{
			Field = new char[3,3];
			for (int i = 0; i <  Field.GetLength(0); i++)
				for (int j = 0; j < Field.GetLength(1); j++)
					Field[i, j] = ' ';
        }
		public string? Host { get; set; }
		public string? Guest { get; set; }
        public string? CurrentMover { get; set; }
		public int MoveCount { get; set; }
		public char[,] Field { get; set; }
    }
}
