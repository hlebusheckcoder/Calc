using Calc.Proxima;
using Calc.Proxima.Functions;

namespace Calc.Test
{
    [TestClass]
    public class FunctionTest
    {
        [TestMethod]
        [DataRow("pow()", DisplayName = "pow()")]
        [DataRow("pow(x)", DisplayName = "pow(x)")]
        [DataRow("pow(x,y)", DisplayName = "pow(x,y)")]
        public void SignatureValid(string signature)
        {
            try
            {
                CustomFunction result = new(signature, string.Empty);
                Assert.IsNotNull(result);
            } catch (Exception e) { Assert.Fail(e.Message); }
        }

        [TestMethod]
        [DataRow("pow", DisplayName = "pow")]
        [DataRow("pow(", DisplayName = "pow(")]
        [DataRow("pow)", DisplayName = "pow)")]
        [DataRow("pow(x", DisplayName = "pow(x")]
        [DataRow("pow(,", DisplayName = "pow(,")]
        [DataRow("pow(,)", DisplayName = "pow(,)")]
        [DataRow("pow()x", DisplayName = "pow()x")]
        [DataRow("pow(x)x", DisplayName = "pow(x)x")]
        [ExpectedException(typeof(SyntaxException))]
        public void SignatureInvalid(string signature)
        {
            _ = new CustomFunction(signature, string.Empty);
        }
    }
}