using Calc.Proxima.Commands;
using Calc.Proxima.Lexemes;

namespace Calc.Test.Commands
{
    [TestClass]
    public class CalculationCommandTest
    {
        [TestMethod]
        [DataRow("=3+2", "5", DisplayName = "3+2")]
        public void Complex(string input, string output)
        {
            var command = new CalculateCommand(LexemeSet.Parse(input));
            var result = command.GetResult(new());
            Assert.AreEqual(result.String, output);
        }
    }
}
