using Calc.Proxima.Functions;

namespace Calc.Proxima
{
    internal class Context
    {
        public static readonly Dictionary<string, Function> Functions = new()
        {
            { "pi", new BaseFunction("pi()", (parameters, context) =>
                {
                    int characters = 2;
                    if (parameters.Length > 0 && parameters[0].IsDouble)
                        characters = parameters[0].Int > 15 ? 15 : parameters[0].Int;
                    return Math.Round(Math.PI, characters);
                }) },
            { "abs", new BaseFunction("abs(value)", (parameters, context) =>
                {
                    Box result;
                    if (parameters.Length == 0)
                        throw new Exception("Значение функции abs не может быть пустым");
                    if (parameters[0].IsDouble)
                        result = Math.Abs(parameters[0].Double);
                    else
                        result = parameters[0];
                    return result;
                }) },
        };

        private readonly Dictionary<string, Box> _variables = [];
        private readonly Context? _parent;

        public Context() { }
        public Context(Context parent) => _parent = parent;

        public Box Result { get; private set; }

        public static bool TryRegister(Function function) =>
            Functions.TryAdd(function.Name, function);

        public static bool HasFunction(string name, out Function? function) =>
            Functions.TryGetValue(name, out function);

        public void SetVariable(string name, Box value)
        {
            if (!_variables.TryAdd(name, value))
                _variables[name] = value;
        }

        public bool TryGetVariable(string name, out Box value) =>
            _variables.TryGetValue(name, out value);

        public Box SetResult(Box value) => Result = value;
    }
}
