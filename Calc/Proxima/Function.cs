using System.Text;

namespace Calc.Proxima
{
    internal record Function
    {
        private readonly CommandSet _commandSet;

        public Function(string signature, string content, Context context)
        {
            Signature = signature;
            (Name, Parameters) = ParseSignature(signature);
            Content = content;
            _commandSet = ParseCommands(content, context);
        }

        public string Signature { get; }

        public string Name { get; }

        public string[] Parameters { get; }

        public string Content { get; }

        public static string Execute(string content, Context context)
        {
            return string.Empty;
        }

        public string Execute(Dictionary<string, double> parameters, Context context)
        {
            return string.Empty;
        }

        private (string Name, string[] Parameters) ParseSignature(string signature)
        {
            if (string.IsNullOrEmpty(signature))
                throw new ArgumentNullException(nameof(signature));
            else if (!char.IsLetter(signature[0]))
                throw new Exception("Имя функции должно начинаться с буквы");

            int index = 0;
            char c = signature[index];
            StringBuilder nameBuilder = new();
            do
            {
                nameBuilder.Append(c);
                index++;
                if (index >= signature.Length)
                    throw new SyntaxException(index + 1, "Сигнатура функции должна содержать круглые скобки");
                c = signature[index];
            } while (char.IsLetter(c) || char.IsDigit(c));

            if (c != '(')
                throw new SyntaxException(index + 1, "Имя функции может содержать только буквы и цифры");

            List<string> parameters = [];
            StringBuilder parameterBuilder = new();
            while (true)
            {
                index++;
                if (index >= signature.Length) break;
                c = signature[index];
                if (c == ' ') continue;
                else if ((!char.IsLetter(c) && c != ')') || (c == ')' && parameters.Count > 0))
                    throw new Exception("Имя параметра функции должно начинаться с буквы");
                parameterBuilder = new();
                while (char.IsLetter(c) || char.IsDigit(c))
                {
                    parameterBuilder.Append(c);
                    index++;
                    if (index >= signature.Length) break;
                    c = signature[index];
                }
                if ((c == ',' || c == ')') && parameterBuilder.Length > 0)
                    parameters.Add(parameterBuilder.ToString());
            }

            if (c != ')')
                throw new SyntaxException(index + 1, "Нет закрывающей скобки");

            return (nameBuilder.ToString(), parameters.ToArray());
        }

        private CommandSet ParseCommands(string content, Context context)
        {
            return new();
        }
    }
}
