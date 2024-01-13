namespace Calc.Proxima.Commands
{
    internal record ResultCommand : Command
    {
        private readonly Box? _value;
        private readonly string? _variableName;
        private readonly CalculateCommand? _valueCommand;
        private readonly Action<Context> _execute;

        public ResultCommand(Box value) =>
            (_value, _execute) = (value, SetResult);
        public ResultCommand(string variableName) =>
            (_variableName, _execute) = (variableName, SetResultFromVariable);
        public ResultCommand(CalculateCommand valueCommand) =>
            (_valueCommand, _execute) = (valueCommand, SetResultFromCommand);

        public override void Execute(Context context) => 
            _execute.Invoke(context);

        private void SetResult(Context context) =>
            context.SetResult((Box)_value!);

        private void SetResultFromVariable(Context context)
        {
            if (context.TryGetVariable(_variableName!, out var value))
                context.SetResult(value);
            else
                throw new Exception($"Переменная {_variableName} не объявлена");
        }

        private void SetResultFromCommand(Context context) =>
            context.SetResult(_valueCommand!.GetResult(context));
    }
}
