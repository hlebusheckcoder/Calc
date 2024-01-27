namespace Calc.Proxima.Functions
{
    internal record BaseFunction : Function
    {
        private readonly Func<Box[], Context, Box> _action;

        public BaseFunction(string signature, Func<Box[], Context, Box> action) :
            base(signature) => _action = action;

        internal override Box Execute(Box[] parameters, Context externalContext) =>
            _action.Invoke(parameters, externalContext);
    }
}
