using Calc.Proxima.Commands;
using Calc.Proxima.Lexemes;

namespace Calc.Proxima.Functions
{
    public abstract record Function
    {
        public Function(string signature)
        {
            Signature = signature;
            (Name, Parameters) = ParseSignature(signature);
        }

        public string Signature { get; }

        public string Name { get; }

        public string[] Parameters { get; }

        internal abstract Box Execute(Box[] parameters, Context externalContext);

        protected (string Name, string[] Parameters) ParseSignature(string signature)
        {
            LexemeSet set = LexemeSet.Parse(signature);
            Lexeme current = set.Current;

            if (current.Type == LexemeType.Eof)
                throw new SyntaxException("Пустая строка");
            else if (current.Type != LexemeType.Name)
                throw new SyntaxException("Имя функции не распознано");

            string name = current.Value;
            current = set.Next();
            if (current.Type != LexemeType.LeftBracket)
                throw new SyntaxException(current.Position.Character, "Ожидается открывающая скобка");

            List<string> parameters = [];
            Lexeme next;
            do
            {
                current = set.Next();
                next = set.Next() ?? current;
                if (current.Type == LexemeType.Name && (next.Type == LexemeType.Delimiter || next.Type == LexemeType.RightBracket))
                    parameters.Add(current.Value);
                else
                    set.Back();
            } while (next.Type == LexemeType.Delimiter);

            if (set.Current.Type != LexemeType.RightBracket)
                throw new SyntaxException(set.Current.Position.Character, "Ожидается закрывающая скобка");

            if (set.Next().Type != LexemeType.Eof)
                throw new SyntaxException(set.Current.Position.Character, "Ожидается конец строки");

            return (name, parameters.ToArray());
        }
    }
}
