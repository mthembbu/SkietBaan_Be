using NUnit.Framework;
using SkietbaanBE.Controllers;

namespace Tests
{
    public class Tests
    {
        private ValuesController controller = new ValuesController();
        
        [Test]
        public void TestGetWithParameters() {
            var response = controller.Get(2);
            Assert.AreEqual("value", response);
        }

        [Test]
        public void TestGetNoParam() {
            var response = controller.Get();
            string[] expected = new string[] { "value1", "value4" };

            Assert.AreEqual(expected, response);
        }

    }
}