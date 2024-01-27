namespace Calc.Proxima
{
    internal enum BoxType
    {
        Double = 0,
        String = 1
    }

    internal struct Box
    {
        private double? _double;
        private string? _string;

        private Box(double value) => (_double, Type) = (value, BoxType.Double);
        private Box(string value) => (_string, Type) = (value, BoxType.String);

        public static implicit operator Box(double value) => new(value);
        public static implicit operator Box(string value) => new(value);
        public static implicit operator Box(int value) => new(Convert.ToDouble(value));

        public readonly double Double => _double ?? Convert.ToDouble(_string);
        public readonly string String => _string ?? _double?.ToString() ?? throw new Exception();
        public readonly int Int => Convert.ToInt32(Double);

        public bool IsDouble
        {
            get
            {
                if (Type == BoxType.Double)
                    return true;

                var result = double.TryParse(_string, out double value);
                if (result)
                {
                    _double = value;
                    Type = BoxType.Double;
                }
                return result;
            }
        }

        public BoxType Type { get; private set; }

        public override readonly string ToString() => String;
    }
}
