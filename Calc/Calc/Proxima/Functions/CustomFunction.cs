using Calc.Proxima.Commands;
using Calc.Proxima.Lexemes;

namespace Calc.Proxima.Functions
{
    public record CustomFunction : Function
    {
        private const int c_arrayStep = 10;
        private Command[] _commands = new Command[c_arrayStep];

        public CustomFunction(string signature, string body) :
            base(signature)
        {
            Body = body;
            _commands = ParseBody(body, Parameters, new());
        }

        internal CustomFunction(string signature, Command[] commands) :
            base(signature)
        {
            Body = string.Empty;
            _commands = commands;
        }

        public string Body { get; }

        public static string Execute(string content)
        {
            Context context = new();
            Command[] commands = ParseBody(content, [], context);
            foreach (var command in commands)
                command.Execute(context);
            return context.Result.String;
        }

        internal override Box Execute(Box[] parameters, Context externalContext)
        {
            Context context = new(externalContext);
            for (int i = 0; i < parameters.Length; i++)
                context.SetVariable(Parameters[i], parameters[i]);
            foreach (var command in _commands)
                command.Execute(context);
            return context.Result;
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
                    throw new SyntaxException(current.Position.Character, "Пустая команда");
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
