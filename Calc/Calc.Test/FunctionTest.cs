using Calc.Proxima;

namespace Calc.Test
{
    [TestClass]
    public class FunctionTest
    {
        private const string c_validSignature = "pow(x,y)";

        [TestMethod]
        [DataRow("pow()")]
        [DataRow("pow(x)")]
        [DataRow("pow(x,y)")]
        public void FunctionSignatureValid(string signature)
        {
            try
            {
                Function result = new(signature, string.Empty);
                Assert.IsNotNull(result);
            } catch (Exception e) { Assert.Fail(e.Message); }
        }

        [TestMethod]
        [DataRow("pow")]
        [DataRow("pow(")]
        [DataRow("pow)")]
        [DataRow("pow(x")]
        [DataRow("pow(,")]
        [DataRow("pow(,)")]
        [DataRow("pow()x")]
        [DataRow("pow(x)x")]
        [ExpectedException(typeof(SyntaxException))]
        public void FunctionSignatureInvalid(string signature)
        {
            _ = new Function(signature, string.Empty);
        }
    }
}