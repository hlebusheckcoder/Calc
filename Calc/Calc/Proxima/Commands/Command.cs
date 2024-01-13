namespace Calc.Proxima.Commands
{
    internal abstract record Command
    {
        public abstract void Execute(Context context);
    }
}
