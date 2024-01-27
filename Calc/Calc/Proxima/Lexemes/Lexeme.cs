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
        Delimiter = 13, // ,
        String = 14 // "content" или 'content'
    }

    internal record Lexeme
    {
        public Lexeme(int character, LexemeType type, char value) :
            this((0, character), type, value.ToString()) { }
        public Lexeme(int character, LexemeType type, string? value = null) :
            this((0, character), type, value) { }
        public Lexeme((int Line, int Character) position, LexemeType type, char value) :
            this(position, type, value.ToString()) { }
        public Lexeme((int Line, int Character) position, LexemeType type, string? value = null) =>
            (Position, Type, Value) = (position, type, value ?? string.Empty);

        public (int Line, int Character) Position { get; }

        public LexemeType Type { get; }

        public string Value { get; }
    }
}
