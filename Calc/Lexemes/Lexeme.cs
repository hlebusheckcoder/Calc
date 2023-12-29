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
        public Lexeme(LexemeType type, char value) :
            this(type, value.ToString()) { }
        public Lexeme(LexemeType type, string? value = null) =>
            (Type, Value) = (type, value ?? string.Empty);

        public LexemeType Type { get; }

        public string Value { get; }
    }
}
