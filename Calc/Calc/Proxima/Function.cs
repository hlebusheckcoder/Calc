using Calc.Proxima.Commands;
using Calc.Proxima.Lexemes;

namespace Calc.Proxima
{
    public record Function
    {
        private const int c_arrayStep = 10;
        private Command[] _commands = new Command[c_arrayStep];

        public Function(string signature, string body)
        {
            (Signature, Name, Parameters) = ParseSignature(signature);
            Body = body;
            _commands = ParseBody(body, Parameters, new());
        }

        public string Signature { get; }

        public string Name { get; }

        public string[] Parameters { get; }

        public string Body { get; }

        public static string Execute(string content)
        {
            Context context = new();
            Command[] commands = ParseBody(content, [], context);
            foreach (var command in commands)
                command.Execute(context);
            return context.Result.String;
        }

        internal Box Execute(Box[] parameters, Context externalContext)
        {
            Context context = new(externalContext);
            for (int i = 0; i < parameters.Length; i++)
                context.SetVariable(Parameters[i], parameters[i]);
            foreach (var command in _commands)
                command.Execute(context);
            return context.Result;
        }

        private (string Signature, string Name, string[] Parameters) ParseSignature(string signature)
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
                throw new SyntaxException(current.Position.Number, "Ожидается открывающая скобка");

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
                throw new SyntaxException(set.Current.Position.Number, "Ожидается закрывающая скобка");

            if (set.Next().Type != LexemeType.Eof)
                throw new SyntaxException(set.Current.Position.Number, "Ожидается конец строки");

            return (signature, name, parameters.ToArray());
        }

        private static Command[] ParseBody(string body, string[] parameters, Context context)
        {
            LexemeSet set = LexemeSet.Parse(body);
            Lexeme current = set.Current;
            List<Command> commands = [];
            Command? command = null;
            while (current.Type != LexemeType.Eof)
            {
                while (current.Type != LexemeType.Eoc && current.Type != LexemeType.Eof)
                {
                    switch (current.Type)
                    {
                        case LexemeType.Name:
                            Lexeme next = set.Next();
                            if (next.Type == LexemeType.OperationAssignment)
                            {
                                command = new SetVariableCommand(current.Value, new CalculateCommand(set));
                                current = set.Next();
                            }
                            else if (current.Value == "return")
                            {
                                var semicolon = set.Next();
                                if (next.Type == LexemeType.Name && semicolon.Type == LexemeType.Eoc)
                                {
                                    command = new ResultCommand(next.Value);
                                    current = semicolon;
                                }
                                else
                                {
                                    set.Back();
                                    set.Back();
                                    command = new ResultCommand(new CalculateCommand(set));
                                    current = set.Next();
                                }

                            }
                            break;
                        default:
                            set.Back();
                            command = new CalculateCommand(set);
                            current = set.Next();
                            break;
                    }
                }
                if (command == null)
                    throw new SyntaxException(current.Position.Number, "Пустая команда");
                commands.Add(command);
                if (command is ResultCommand)
                    break;
                if (current.Type != LexemeType.Eof)
                    current = set.Next();
            }

            if (command is CalculateCommand calculateCommand)
                commands.Add(new ResultCommand(calculateCommand));


            return commands.ToArray();
        }
    }
}
