namespace Calc.Lexemes
{
    public class LexemeSet
    {
        private const int c_arrayStep = 10;
        private Lexeme[] _items = new Lexeme[c_arrayStep];
        private int _fullness = 0;
        private int _index = 0;

        public LexemeSet() { }

        public Lexeme Current => _items[_index];

        public bool Empty => _fullness == 0;

        public void Add(Lexeme newItem)
        {
            if (_fullness == _items.Length)
                Array.Resize(ref _items, _items.Length + c_arrayStep);

            _items[_fullness++] = newItem;
        }

        public Lexeme Next() => _items[_index++];

        public Lexeme Back() => _items[_index--];

        public void Close() => Add(new(LexemeType.Eof));
    }
}
