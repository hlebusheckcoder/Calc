using Calc.Proxima.Lexemes;

namespace Calc.Test
{
    [TestClass]
    public class LexemeSetTest
    {

        [TestMethod]
        [DataRow("\"word\"", DisplayName = "word")]
        [DataRow("\"many words\"", DisplayName = "many words")]
        public void StringTest(string input)
        {
            var lexeme = LexemeSet.Parse(input).Current;
            if (lexeme.Type != LexemeType.String)
                Assert.Fail($"Тип лексемы {lexeme.Type} не совпадает с ожидаемым {nameof(LexemeType.String)}");
            else if (lexeme.Value != input[1..^1])
                Assert.Fail($"Содержимое лексемы не совпадает с ожидаемым: {lexeme.Value}");
            else
                Assert.IsTrue(true);
        }
    }
}
