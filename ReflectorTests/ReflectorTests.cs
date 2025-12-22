using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using controlWork2;

namespace controlWork2.Tests
{
    [TestFixture]
    public class ReflectorTests
    {


        [Test]
        public void PrintStructure_CreatesFile()
        {
            Reflector.PrintStructure(typeof(string));

            Assert.IsTrue(File.Exists("String.cs"));
        }

        [Test]
        public void PrintStructure_FileIsNotEmpty()
        {
            Reflector.PrintStructure(typeof(string));

            var content = File.ReadAllText("String.cs");
            Assert.IsNotEmpty(content);
        }

        [Test]
        public void PrintStructure_ContainsClassDeclaration()
        {
            Reflector.PrintStructure(typeof(string));

            var content = File.ReadAllText("String.cs");
            StringAssert.Contains("class String", content);
        }


        [Test]
        public void DiffClasses_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                Reflector.DiffClasses(typeof(string), typeof(object)));
        }

        [Test]
        public void DiffClasses_WritesOutput()
        {
            var writer = new StringWriter();

            Reflector.DiffClasses(typeof(string), typeof(object), writer);

            Assert.IsNotEmpty(writer.ToString());
        }
    }
}

