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

        public void Add(int position, LexemeType type, char value) =>
            Add(new(position, type, value));
        public void Add(int position, LexemeType type, string? value = null) =>
            Add(new(position, type, value));
        public void Add(Lexeme newItem)
        {
            if (_fullness == _items.Length)
                Array.Resize(ref _items, _items.Length + c_arrayStep);

            _items[_fullness++] = newItem;
        }

        public Lexeme Next() => _items[_index++];

        public void Back() => _index--;

        public void Close()
        {
            var lastItem = _items[_fullness - 1];
            Add(new(lastItem.Position + lastItem.Value.Length, LexemeType.Eof));
        }
    }
}
