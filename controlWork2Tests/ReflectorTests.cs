using controlWork2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;

namespace controlWork2Tests
{
    [TestClass]
    public class ReflectorTests
    {
        [TestMethod]
        public void PrintStructure_CreatesFile()
        {
            if (File.Exists("String.cs"))
                File.Delete("String.cs");

            Reflector.PrintStructure(typeof(string));

            Assert.IsTrue(File.Exists("String.cs"));
        }

        [TestMethod]
        public void PrintStructure_FileContainsClass()
        {
            Reflector.PrintStructure(typeof(string));

            var content = File.ReadAllText("String.cs");

            StringAssert.Contains(content, "class String");
        }

        [TestMethod]
        public void DiffClasses_WritesOutput()
        {
            var writer = new StringWriter();

            Reflector.DiffClasses(typeof(string), typeof(object), writer);

            Assert.IsFalse(string.IsNullOrEmpty(writer.ToString()));
        }
    }
}
