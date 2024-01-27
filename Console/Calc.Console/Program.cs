using Calc.Proxima;
using Calc.Proxima.Functions;
using System.Globalization;
using System.Text;

Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

while (true)
{
    try
    {
        StringBuilder builder = new();
        string? input;
        do
        {
            input = Console.ReadLine();
            builder.Append(input).Append('\n');
        } while (input?.Trim().StartsWith("return") != true);
        input = builder.ToString();
        var result = CustomFunction.Execute(input ?? string.Empty);
        Console.Write("Ответ: ");
        Console.WriteLine(result);
    }
    catch (SyntaxException e)
    {
        Console.Write($"Ошибка ({e.Line}, {e.Position}): ");
        Console.WriteLine(e.Message);
    }
    catch (Exception e)
    {
        Console.Write("Ошибка: ");
        Console.WriteLine(e.Message);
    }
    Console.ReadKey();
    Console.Clear();
}
