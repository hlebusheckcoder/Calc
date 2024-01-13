namespace Calc.Proxima
{
    internal class Context
    {
        public static readonly Dictionary<string, Function> Functions = [];

        private readonly Dictionary<string, Box> _variables = [];
        private readonly Context? _parent;

        public Context() { }
        public Context(Context parent) => _parent = parent;

        public Box Result { get; private set; }

        public static bool TryRegister(Function function) =>
            Functions.TryAdd(function.Name, function);

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
