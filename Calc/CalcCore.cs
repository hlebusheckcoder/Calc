using Calc.Lexemes;
using System.Text;

namespace Calc
{
    public class CalcCore
    {
        public static LexemeSet Parse(string input)
        {
            LexemeSet result = new();
            int index = 0;

            while (index < input.Length)
            {
                char c = input[index];
                switch (c)
                {
                    case '(': result.Add(new(LexemeType.LeftBracket, c)); index++; break;
                    case ')': result.Add(new(LexemeType.RightBracket, c)); index++; break;
                    case '+': result.Add(new(LexemeType.OperationAddition, c)); index++; break;
                    case '-': result.Add(new(LexemeType.OperationSubtraction, c)); index++; break;
                    case '*': result.Add(new(LexemeType.OperationMultiplication, c)); index++; break;
                    case ':': case '/': result.Add(new(LexemeType.OperationDivision, c)); index++; break;
                    case '^': result.Add(new(LexemeType.OperationPow, c)); index++; break;
                    case '|': result.Add(new(LexemeType.AbsBracket, c)); index++; break;
                    default:
                        if (char.IsDigit(c))
                        {
                            StringBuilder builder = new StringBuilder();
                            do
                            {
                                builder.Append(c);
                                index++;
                                if (index >= input.Length) break;
                                c = input[index];
                            } while (char.IsDigit(c));
                            result.Add(new(LexemeType.Number, builder.ToString()));
                        }
                        else if (char.IsLetter(c))
                        {
                            StringBuilder builder = new StringBuilder();
                            do
                            {
                                builder.Append(c);
                                index++;
                                if (index >= input.Length) break;
                                c = input[index];
                            } while (char.IsLetter(c));
                            result.Add(new(LexemeType.Function, builder.ToString()));
                        }
                        else
                        {
                            if (c != ' ')
                                throw new Exception($"Нераспознанный символ : {c}");
                            index++;
                        }
                        break;
                }
            }

            result.Close();
            return result;
        }

        public static double Execute(string input) =>
            Execute(Parse(input));
        public static double Execute(LexemeSet input)
        {
            Lexeme item = input.Next();
            if (item.Type == LexemeType.Eof)
                throw new Exception("Пустая строка");
            else
            {
                _ = input.Back();
                return LevelThree(input);
            }
        }

        private static double LevelThree(LexemeSet input)
        {
            double result = LevelTwo(input);
            while (true)
            {
                Lexeme value = input.Next();
                switch (value.Type)
                {
                    case LexemeType.OperationAddition:
                        result += LevelTwo(input);
                        break;
                    case LexemeType.OperationSubtraction:
                        result -= LevelTwo(input);
                        break;
                    default:
                        _ = input.Back();
                        return result;
                }
            }
        }

        private static double LevelTwo(LexemeSet input)
        {
            double result = LevelOne(input);
            while (true)
            {
                Lexeme value = input.Next();
                switch (value.Type)
                {
                    case LexemeType.OperationMultiplication:
                        result *= LevelOne(input);
                        break;
                    case LexemeType.OperationDivision:
                        double temp = LevelOne(input);
                        if (temp != 0.0)
                            throw new Exception("Деление на ноль");
                        else
                            result /= temp;
                        break;
                    default:
                        _ = input.Back();
                        return result;
                }
            }
        }

        private static double LevelOne(LexemeSet input)
        {
            double result = Atom(input);
            while (true)
            {
                Lexeme value = input.Next();
                if (value.Type == LexemeType.OperationPow)
                    result = Math.Pow(result, Atom(input));
                else
                {
                    _ = input.Back();
                    return result;
                }
            }
        }

        private static double Atom(LexemeSet input)
        {
            Lexeme item = input.Next();
            switch (item.Type)
            {
                case LexemeType.Number:
                    return Convert.ToDouble(item.Value);
                case LexemeType.LeftBracket:
                    double bracketsResult = Execute(input);
                    item = input.Next();
                    if (item.Type != LexemeType.RightBracket)
                        throw new Exception("Отсутствует закрывающая скобка");
                    return bracketsResult;
                case LexemeType.AbsBracket:
                    double absResult = Execute(input);
                    item = input.Next();
                    if (item.Type != LexemeType.AbsBracket)
                        throw new Exception("Отсутствует закрывающая скобка");
                    return Math.Abs(absResult);
                default:
                    throw new Exception($"Нераспознанный символ : {item.Value}");
            }
        }
    }
}
