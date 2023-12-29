using Calc;

while (true)
{
    var input = Console.ReadLine();
    try
    {
        var result = CalcCore.Execute(input ?? string.Empty);
        Console.Write("Ответ: ");
        Console.WriteLine(result);
    }
    catch (Exception e)
    {
        Console.Write("Ошибка: ");
        Console.WriteLine(e.Message);
    }
    Console.ReadKey();
    Console.Clear();
}
