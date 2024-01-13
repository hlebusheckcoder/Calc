namespace Calc.Proxima.Commands
{
    internal partial record SetVariableCommand : Command
    {
        private readonly string _variableName;
        private readonly Box[]? _values;
        private readonly CalculateCommand[]? _valueCommands;
        private readonly Action<Context> _execute;

        public SetVariableCommand(string variableName, Box value) =>
            (_variableName, _values, _execute) = (variableName, [value], Set);
        public SetVariableCommand(string variableName, CalculateCommand valueCommand) =>
            (_variableName, _valueCommands, _execute) = (variableName, [valueCommand], CalculateAndSet);
        public SetVariableCommand(string variableName, Box[] values) =>
            (_variableName, _values, _execute) = (variableName, values, SetArray);
        public SetVariableCommand(string variableName, CalculateCommand[] valueCommands) =>
            (_variableName, _valueCommands, _execute) = (variableName, valueCommands, CalculateAndSetArray);

        public override void Execute(Context context) =>
            _execute.Invoke(context);

        private void Set(Context context) =>
            context.SetVariable(_variableName, _values![0]);

        private void CalculateAndSet(Context context) =>
            context.SetVariable(_variableName, _valueCommands![0].GetResult(context));

        private void SetArray(Context context)
        {
            var length = _values!.Length;
            context.SetVariable(_variableName, length);
            for (int i = 0; i < length; i++)
                context.SetVariable($"{_variableName}[{i}]", _values[i]);
        }

        private void CalculateAndSetArray(Context context)
        {
            var length = _valueCommands!.Length;
            context.SetVariable(_variableName, length);
            for (int i = 0; i < length; i++)
                context.SetVariable($"{_variableName}[{i}]", _valueCommands[i].GetResult(context));
        }
    }
}
