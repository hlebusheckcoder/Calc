using System.Text;

namespace Calc.Proxima.Lexemes
{
    internal class LexemeSet
    {
        private const int c_arrayStep = 10;
        private Lexeme[] _items = new Lexeme[c_arrayStep];
        private int _fullness = 0;
        private int _index = 0;

        public LexemeSet() { }

        public Lexeme Current => _items[_index];

        public bool Empty => _fullness == 0;

        public static LexemeSet Parse(string input)
        {
            LexemeSet result = new();
            int index = 0;

            while (index < input.Length)
            {
                char c = input[index];
                switch (c)
                {
                    case '(': result.Add(index + 1, LexemeType.LeftBracket, c); index++; break;
                    case ')': result.Add(index + 1, LexemeType.RightBracket, c); index++; break;
                    case '=': result.Add(index + 1, LexemeType.OperationAssignment, c); index++; break;
                    case '+': result.Add(index + 1, LexemeType.OperationAddition, c); index++; break;
                    case '-': result.Add(index + 1, LexemeType.OperationSubtraction, c); index++; break;
                    case '*': result.Add(index + 1, LexemeType.OperationMultiplication, c); index++; break;
                    case ':': case '/': result.Add(index + 1, LexemeType.OperationDivision, c); index++; break;
                    case '^': result.Add(index + 1, LexemeType.OperationPow, c); index++; break;
                    case '|': result.Add(index + 1, LexemeType.AbsBracket, c); index++; break;
                    case ',': result.Add(index + 1, LexemeType.Delimiter, c); index++; break;
                    case ';': result.Add(index + 1, LexemeType.Eoc, c); index++; break;
                    default:
                        if (char.IsDigit(c))
                        {
                            var position = index + 1;
                            bool realNumber = false;
                            StringBuilder builder = new StringBuilder();
                            do
                            {
                                builder.Append(c);
                                index++;
                                if (index >= input.Length) break;
                                c = input[index];
                                if (c == '.')
                                {
                                    if (realNumber)
                                        throw new SyntaxException(index, $"Нераспознанный символ: {c}");
                                    realNumber = true;
                                }
                            } while (char.IsDigit(c) || c == '.');
                            result.Add(position, LexemeType.Number, builder.ToString());
                        }
                        else if (char.IsLetter(c))
                        {
                            var position = index + 1;
                            StringBuilder builder = new StringBuilder();
                            do
                            {
                                builder.Append(c);
                                index++;
                                if (index >= input.Length) break;
                                c = input[index];
                            } while (char.IsLetter(c) || char.IsDigit(c));
                            result.Add(position, LexemeType.Name, builder.ToString());
                        }
                        else
                        {
                            if (c != ' ' && c != '\n')
                                throw new SyntaxException(index, $"Нераспознанный символ: {c}");
                            index++;
                        }
                        break;
                }
            }

            result.Add(index, LexemeType.Eof);
            return result;
        }

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

        public Lexeme Next() => _index < _fullness ? _items[++_index] : _items[_index];

        public void Back() => _index--;
    }
}
