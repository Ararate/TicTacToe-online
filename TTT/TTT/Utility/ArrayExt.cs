using System.Runtime.InteropServices;
using System;
using Microsoft.AspNetCore.Components.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace TTT.Utility
{
    public static class ArrayExt
    {
        public static T[] GetRow<T>(this T[,] array, int row)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException("array");

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }
        public static T[] GetColumn<T>(this T[,] array, int col)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException("array");

            int cols = array.GetLength(1);
            int rows = array.GetLength(0);
            T[] result = new T[rows];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf<T>();
            int offset = size * col;
            for (int i = 0; i < rows; i++)
                Buffer.BlockCopy(array, i * cols * size + offset, result, i * size, size);

            return result;
        }
        public static T[] GetMainDiag<T>(this T[,] array, int x, int y)
        {
            int ty = x - y <= 0 ? 0 : x-y;
            int tx = y - x <= 0 ? 0 : y-x;

            int len1 = array.GetLength(0) - ty;
            int len2 = array.GetLength(1) - tx;
            int length = len1 < len2 ? len1 : len2;
            T[] res = new T[length];

            for (int i = 0;i < length; i++)
                res[i] = array[tx + i, ty + i];
            return res;
        }
        public static T[] GetAntiDiag<T>(this T[,] array, int x, int y)
        {
            int boundY = array.GetUpperBound(1);
            int ty = x + y >= boundY ? boundY : x + y;
            int tx = x- (boundY - y) < 0 ? 0 : x-(boundY - y);

            int len1 = ty + 1;
            int len2 = array.GetLength(0) - tx;
            int length = len1 < len2 ? len1 : len2;
            T[] res = new T[length];

            for (int i = 0; i < length; i++)
                res[length- i-1] = array[tx + i, ty - i];
            return res;
        }
    }
}
