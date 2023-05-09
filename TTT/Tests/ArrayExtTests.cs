using TTT;
using TTT.Utility;

namespace Tests
{
    public class ArrayExtTests
    {
        private char[,] a = new char[3, 3] { { 'A', 'S', 'D' }, 
                                             { 'Z', 'X', 'C' }, 
                                             { 'E', 'R', 'T' } };
        [Fact]
        public void ArrayExtGetSecondRow()
        {
            char[] res = a.GetRow(1);

            Assert.Equal("ZXC", new String(res));
        }
        [Fact]
        public void ArrayExtGetSecondCol()
        {
            char[] res = a.GetColumn(1);

            Assert.Equal("SXR", new String(res));
        }
    }
}