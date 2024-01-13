namespace Calc.Proxima
{
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) :
            this(0, 0, message) { }
        public SyntaxException(int position, string message) :
            this(0, position, message) { }
        public SyntaxException(int line, int position, string message) : base(message) =>
            (Line, Position) = (line, position);

        public int Line { get; }
        public int Position { get; }
    }
}
