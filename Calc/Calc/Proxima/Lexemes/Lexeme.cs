namespace Calc.Proxima.Lexemes
{
    internal enum LexemeType
    {
        Eof = 0, // конец строки
        Eoc = 1, // конец команды
        Number = 2, // число
        Name = 3, // имя
        LeftBracket = 4, // (
        RightBracket = 5, // )
        OperationAssignment = 6, // =
        OperationAddition = 7, // +
        OperationSubtraction = 8, // -
        OperationMultiplication = 9, // *
        OperationDivision = 10, // : или /
        OperationPow = 11, // ^
        AbsBracket = 12, // |
        Delimiter = 13 // ,
    }

    internal record Lexeme
    {
        public Lexeme(int number, LexemeType type, char value) :
            this((0, number), type, value.ToString()) { }
        public Lexeme(int number, LexemeType type, string? value = null) :
            this((0, number), type, value) { }
        public Lexeme((int Line, int Number) position, LexemeType type, char value) :
            this(position, type, value.ToString()) { }
        public Lexeme((int Line, int Number) position, LexemeType type, string? value = null) =>
            (Position, Type, Value) = (position, type, value ?? string.Empty);

        public (int Line, int Number) Position { get; }

        public LexemeType Type { get; }

        public string Value { get; }
    }
}
