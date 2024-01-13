using Calc.Proxima.Lexemes;

namespace Calc.Proxima.Commands
{
    internal record CalculateCommand : Command
    {

        private readonly Operation _operation;
        private Box _result;
        private bool _executed = false;

        public CalculateCommand(LexemeSet lexemeSet)
        {
            _operation = Compile(lexemeSet);
        }

        public override void Execute(Context context) =>
            _result = _operation.Execute(context);

        public Box GetResult(Context context)
        {
            if (!_executed)
            {
                Execute(context);
                _executed = true;
            }

            return _result!;
        }

        private Operation Compile(LexemeSet lexemes)
        {
            Lexeme item = lexemes.Next();
            if (item.Type == LexemeType.Eof)
                throw new Exception("Пустая строка");
            else
            {
                lexemes.Back();
                return LevelThree(lexemes);
            }
        }

        private Operation LevelThree(LexemeSet lexemes)
        {
            Operation result = LevelTwo(lexemes);
            while (true)
            {
                Lexeme value = lexemes.Next();
                switch (value.Type)
                {
                    case LexemeType.OperationAddition:
                        result = new AdditionOperation(result, LevelTwo(lexemes));
                        break;
                    case LexemeType.OperationSubtraction:
                        result = new SubtractionOperation(result, LevelTwo(lexemes));
                        break;
                    default:
                        lexemes.Back();
                        return result;
                }
            }
        }

        private Operation LevelTwo(LexemeSet lexemes)
        {
            Operation result = LevelOne(lexemes);
            while (true)
            {
                Lexeme value = lexemes.Next();
                switch (value.Type)
                {
                    case LexemeType.OperationMultiplication:
                        result = new MultiplicationOperation(result, LevelOne(lexemes));
                        break;
                    case LexemeType.OperationDivision:
                        result = new DivisionOperation(result, LevelOne(lexemes));
                        break;
                    default:
                        lexemes.Back();
                        return result;
                }
            }
        }

        private Operation LevelOne(LexemeSet lexemes)
        {
            Operation result = Atom(lexemes);
            while (true)
            {
                Lexeme value = lexemes.Next();
                if (value.Type == LexemeType.OperationPow)
                    result = new PowOperation(result, Atom(lexemes));
                else
                {
                    lexemes.Back();
                    return result;
                }
            }
        }

        private Operation Atom(LexemeSet lexemes)
        {
            Lexeme item = lexemes.Next();
            Operation result;
            switch (item.Type)
            {
                case LexemeType.Number:
                    result = new ValueOperation(item.Value);
                    break;
                case LexemeType.Name:
                    result = new VariableValueOperation(item.Value);
                    break;
                case LexemeType.OperationSubtraction:
                    var next = lexemes.Next();
                    switch (next.Type)
                    {
                        case LexemeType.Number:
                            result = new InvertOperation(new ValueOperation(next.Value));
                            break;
                        case LexemeType.Name:
                            result = new InvertOperation(new VariableValueOperation(next.Value));
                            break;
                        case LexemeType.LeftBracket:
                            Operation subtractionBracketsResult = Compile(lexemes);
                            item = lexemes.Next();
                            if (item.Type != LexemeType.RightBracket)
                                throw new Exception($"Отсутствует закрывающая скобка ({item.Position})");
                            result = new InvertOperation(subtractionBracketsResult);
                            break;
                        case LexemeType.AbsBracket:
                            Operation subtractionAbsResult = Compile(lexemes);
                            item = lexemes.Next();
                            if (item.Type != LexemeType.AbsBracket)
                                throw new Exception($"Отсутствует закрывающая скобка ({item.Position})");
                            result = new InvertOperation(new AbsOperation(subtractionAbsResult));
                            break;
                        default:
                            throw new Exception($"Нераспознанное выражение ({item.Position}): {item.Value}");
                    }
                    break;
                case LexemeType.LeftBracket:
                    Operation bracketsResult = Compile(lexemes);
                    item = lexemes.Next();
                    if (item.Type != LexemeType.RightBracket)
                        throw new Exception($"Отсутствует закрывающая скобка ({item.Position})");
                    result = bracketsResult;
                    break;
                case LexemeType.AbsBracket:
                    Operation absResult = Compile(lexemes);
                    item = lexemes.Next();
                    if (item.Type != LexemeType.AbsBracket)
                        throw new Exception($"Отсутствует закрывающая скобка ({item.Position})");
                    result = new AbsOperation(absResult);
                    break;
                default:
                    throw new Exception($"Нераспознанное выражение ({item.Position}): {item.Value}");
            }

            return result;
        }

        #region Operations

        #region Base

        private abstract record Operation
        {
            public abstract Box Execute(Context context);
        }

        private abstract record OperationOwner : Operation
        {
            protected OperationOwner(Operation operation) =>
                Operation = operation;

            protected Operation Operation { get; }
        }

        private abstract record MultiOperationOwner : Operation
        {
            protected MultiOperationOwner(Operation firstOperation, Operation secondOperation) =>
                (FirstOperation, SecondOperation) = (firstOperation, secondOperation);

            protected Operation FirstOperation { get; }
            protected Operation SecondOperation { get; }
        }

        #endregion Base

        private record ValueOperation : Operation
        {
            private readonly Box _value;

            public ValueOperation(Box value) => _value = value;

            public override Box Execute(Context context) => _value;
        }

        private record VariableValueOperation : Operation
        {
            private readonly string _variableName;

            public VariableValueOperation(string variableName) => _variableName = variableName;

            public override Box Execute(Context context)
            {
                _ = context.TryGetVariable(_variableName, out Box result);
                return result;
            }
        }

        private record InvertOperation : OperationOwner
        {
            public InvertOperation(Operation operation) : base(operation) { }

            public override Box Execute(Context context)
            {
                Box result = Operation.Execute(context);

                if (result.IsDouble)
                    result = -result.Double;
                else
                {
                    char[] chars = result.String.ToCharArray();
                    Array.Reverse(chars);
                    result = new string(chars);
                }

                return result;
            }
        }

        private record AbsOperation : OperationOwner
        {
            public AbsOperation(Operation operation) : base(operation) { }

            public override Box Execute(Context context)
            {
                Box result = Operation.Execute(context);

                if (result.IsDouble)
                    result = Math.Abs(result.Double);

                return result;
            }
        }

        private record AdditionOperation : MultiOperationOwner
        {
            public AdditionOperation(Operation firstTerm, Operation secondTerm) :
                base(firstTerm, secondTerm)
            { }

            public override Box Execute(Context context)
            {
                Box firstTermBox = FirstOperation.Execute(context);
                Box secondTermBox = SecondOperation.Execute(context);

                Box result;

                if (firstTermBox.IsDouble && secondTermBox.IsDouble)
                    result = firstTermBox.Double + secondTermBox.Double;
                else
                    result = firstTermBox.String + firstTermBox.String;

                return result;
            }
        }

        private record SubtractionOperation : MultiOperationOwner
        {
            public SubtractionOperation(Operation diminished, Operation subtraction) :
                base(diminished, subtraction) { }

            public override Box Execute(Context context)
            {
                Box diminishedBox = FirstOperation.Execute(context);
                Box subtraction = SecondOperation.Execute(context);

                Box result;

                if (diminishedBox.IsDouble && subtraction.IsDouble)
                    result = diminishedBox.Double - subtraction.Double;
                else
                {
                    if (diminishedBox.String.Contains(subtraction.String))
                        result = diminishedBox.String.Replace(subtraction.String, null);
                    else
                        result = diminishedBox.String + subtraction.String;
                }

                return result;
            }
        }

        private record MultiplicationOperation : MultiOperationOwner
        {
            public MultiplicationOperation(Operation firstMultiplier, Operation secondMultiplier) :
                base(firstMultiplier, secondMultiplier) { }

            public override Box Execute(Context context)
            {
                var firstBox = FirstOperation.Execute(context);
                var secondBox = SecondOperation.Execute(context);

                Box result;

                if (firstBox.IsDouble && secondBox.IsDouble)
                    result = firstBox.Double * secondBox.Double;
                else if (firstBox.IsDouble)
                    result = string.Concat(Enumerable.Repeat(secondBox.String, (int)firstBox.Double));
                else if (secondBox.IsDouble)
                    result = string.Concat(Enumerable.Repeat(firstBox.String, (int)secondBox.Double));
                else
                    result = firstBox.String + secondBox.String;

                return result;
            }
        }

        private record DivisionOperation : MultiOperationOwner
        {
            public DivisionOperation(Operation divisor, Operation quotient)
                : base(divisor, quotient) { }

            public override Box Execute(Context context)
            {
                Box divisorBox = FirstOperation.Execute(context);
                Box quotientBox = SecondOperation.Execute(context);

                Box result;

                if (divisorBox.IsDouble && quotientBox.IsDouble)
                {
                    if (quotientBox.Double == 0.0)
                        throw new Exception($"Деление на ноль");
                    else 
                        result = divisorBox.Double / quotientBox.Double;
                }
                else
                    result = divisorBox.String + quotientBox.String;

                return result;
            }
        }

        private record PowOperation : MultiOperationOwner
        {
            public PowOperation(Operation value, Operation exponentiation) :
                base(value, exponentiation)
            { }

            public override Box Execute(Context context)
            {
                var valueBox = FirstOperation.Execute(context);
                var exponentiation = SecondOperation.Execute(context);

                Box result;

                if (valueBox.IsDouble && exponentiation.IsDouble)
                    result = Math.Pow(valueBox.Double, exponentiation.Double);
                else
                    result = valueBox.String + exponentiation.String;

                return result;
            }
        }

        #endregion Operations
    }
}
