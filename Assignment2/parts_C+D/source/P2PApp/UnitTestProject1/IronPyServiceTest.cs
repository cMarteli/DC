using ClientDesktop.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {
    [TestClass]
    public class IronPyServiceTest {
        private IronPyService _ironPyService;

        [TestInitialize]
        public void Setup() {
            _ironPyService = new IronPyService();
        }

        [TestMethod]
        public void TestExecutePythonJob_ReturnsExpectedResult() {
            string pythonCode = "def test_func(var1,var2):\r\n return var1 + var2";

            string result = _ironPyService.ExecutePythonJob(pythonCode);
            Assert.AreEqual("27", result); // 23 + 4 = 27
        }

        [TestMethod]
        public void TestExecutePythonJob_NoFunctionDefined_ReturnsNull() {
            string pythonCode = @"
# No function defined
";

            string result = _ironPyService.ExecutePythonJob(pythonCode);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestExecutePythonJob_ExceptionOccurs_ReturnsNull() {
            string pythonCode = @"
def test_func(a, b):
    raise Exception('Some exception')
";

            string result = _ironPyService.ExecutePythonJob(pythonCode);
            Assert.IsNull(result);
        }
    }
}
