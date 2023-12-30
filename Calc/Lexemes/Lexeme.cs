namespace Calc.Lexemes
{
    public enum LexemeType
    {
        Eof = 0,
        Number = 1,
        Function = 2,
        LeftBracket = 3,
        RightBracket = 4,
        OperationAddition = 5,
        OperationSubtraction = 6,
        OperationMultiplication = 7,
        OperationDivision = 8,
        OperationPow = 9,
        AbsBracket = 10
    }

    public record Lexeme
    {
        public Lexeme(int position, LexemeType type, char value) :
            this(position, type, value.ToString()) { }
        public Lexeme(int position, LexemeType type, string? value = null) =>
            (Position, Type, Value) = (position, type, value ?? string.Empty);

        public int Position { get; }

        public LexemeType Type { get; }

        public string Value { get; }
    }
}
