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
        public int? X { get; set; }
        public int? Y { get; set; }
		public string? Host { get; set; }
		public string? Guest { get; set; }
        public string? CurrentMover { get; set; }
		public int MoveCount { get; set; }
        //[EditorBrowsable(EditorBrowsableState.Never)]
		//public string FieldData { get; set; }
		public char[,] Field { get; set; }
        //{
        //	get{
        //		char[,] tempField = new char[2,2];
        //		int rowLength = tempField.GetLength(1);
        //              for (int i = 0; i < tempField.GetLength(0); i++)
        //                  for (int j = 0; j < rowLength; j++)
        //                      tempField[i, j] = FieldData[i * rowLength + j];
        //		return tempField;
        //          } 
        //	set{
        //		StringBuilder sb = new();
        //              for (int i = 0; i < value.GetLength(0); i++)
        //                  for (int j = 0; j < value.GetLength(1); j++)
        //                      sb.Append(value[i, j]);
        //		FieldData = sb.ToString();
        //          }
        //}
        public TaskCompletionSource<bool>? Awaiter;
    }
}
