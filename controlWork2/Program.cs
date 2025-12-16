using System;

namespace controlWork2
{
    class Program
    {
        static void Main(string[] args)
        {
            Reflector.PrintStructure(typeof(string));
            Reflector.DiffClasses(typeof(string), typeof(object));
        }
    }
}