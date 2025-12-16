using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace controlWork2
{
    public class Reflector
    {
        public static void PrintStructure(Type someClass)
        {
            using var writer = new StreamWriter($"{someClass.Name}.cs");

            writer.WriteLine("using System;");
            writer.WriteLine();

            string className = someClass.Name;
            if (someClass.IsGenericType)
            {
                className = className.Split('`')[0];
                className += "<" + string.Join(", ",
                    someClass.GetGenericArguments().Select(t => t.Name)) + ">";
            }

            string visibility = someClass.IsPublic ? "public" : "internal";

            string inheritance = "";

            if (someClass.BaseType != null && someClass.BaseType != typeof(object))
            {
                inheritance = someClass.BaseType.Name;
            }

            var interfaces = someClass.GetInterfaces();
            if (interfaces.Length > 0)
            {
                if (inheritance.Length > 0)
                {
                    inheritance += ", ";
                }
                inheritance += string.Join(", ", interfaces.Select(i => i.Name));
            }

            if (inheritance.Length > 0)
            {
                inheritance = " : " + inheritance;
            }

            writer.WriteLine($"{visibility} class {className}{inheritance}");
            writer.WriteLine("{");

            var flags = BindingFlags.Instance |
                        BindingFlags.Static |
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.DeclaredOnly;

            var fields = someClass.GetFields(flags);
            foreach (var field in fields)
            {
                writer.WriteLine(
                    $"    {GetVisibility(field)} {(field.IsStatic ? "static " : "")}{field.FieldType.Name} {field.Name};"
                );
            }

            var methods = someClass.GetMethods(flags)
                                   .Where(m => !m.IsSpecialName);

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                string paramString = string.Join(", ",
                    parameters.Select(p => $"{p.ParameterType.Name} {p.Name}")
                );

                writer.WriteLine(
                    $"    {GetVisibility(method)} {(method.IsStatic ? "static " : "")}{method.ReturnType.Name} {method.Name}({paramString})"
                );
                writer.WriteLine("    {");

                if (method.ReturnType != typeof(void))
                {
                    writer.WriteLine($"        return default({method.ReturnType.Name});");
                }

                writer.WriteLine("    }");
            }

            writer.WriteLine("}");
        }

        public static void DiffClasses(Type a, Type b)
        {
            DiffClasses(a, b, Console.Out);
        }

        public static void DiffClasses(Type a, Type b, TextWriter writer)
        {
            var flags = BindingFlags.Instance |
                        BindingFlags.Static |
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.DeclaredOnly;

            writer.WriteLine("===== FIELDS =====");

            var fieldsA = a.GetFields(flags);
            var fieldsB = b.GetFields(flags);

            writer.WriteLine("Only in A:");
            foreach (var fa in fieldsA)
            {
                if (!fieldsB.Any(fb => SameField(fa, fb)))
                {
                    writer.WriteLine($"  {fa.Name}");
                }
            }

            writer.WriteLine("Only in B:");
            foreach (var fb in fieldsB)
            {
                if (!fieldsA.Any(fa => SameField(fa, fb)))
                {
                    writer.WriteLine($"  {fb.Name}");
                }
            }

            writer.WriteLine("Different:");
            foreach (var fa in fieldsA)
            {
                var fb = fieldsB.FirstOrDefault(f => f.Name == fa.Name);
                if (fb != null && !SameField(fa, fb))
                {
                    writer.WriteLine($"  {fa.Name}");
                }
            }

            writer.WriteLine();
            writer.WriteLine("===== METHODS =====");

            var methodsA = a.GetMethods(flags).Where(m => !m.IsSpecialName);
            var methodsB = b.GetMethods(flags).Where(m => !m.IsSpecialName);

            writer.WriteLine("Only in A:");
            foreach (var ma in methodsA)
            {
                if (!methodsB.Any(mb => SameMethod(ma, mb)))
                {
                    writer.WriteLine($"  {ma.Name}");
                }
            }

            writer.WriteLine("Only in B:");
            foreach (var mb in methodsB)
            {
                if (!methodsA.Any(ma => SameMethod(ma, mb)))
                {
                    writer.WriteLine($"  {mb.Name}");
                }
            }

            writer.WriteLine("Different:");
            foreach (var ma in methodsA)
            {
                var mb = methodsB.FirstOrDefault(m => m.Name == ma.Name);
                if (mb != null && !SameMethod(ma, mb))
                {
                    writer.WriteLine($"  {ma.Name}");
                }
            }
        }

        private static bool SameField(FieldInfo a, FieldInfo b)
        {
            return a.Name == b.Name &&
                   a.FieldType == b.FieldType &&
                   a.IsStatic == b.IsStatic &&
                   a.IsPublic == b.IsPublic;
        }

        private static bool SameMethod(MethodInfo a, MethodInfo b)
        {
            if (a.Name != b.Name)
            {
                return false;
            }
            if (a.ReturnType != b.ReturnType)
            {
                return false;
            }
            if (a.IsStatic != b.IsStatic)
            {
                return false;
            }
            if (a.IsPublic != b.IsPublic)
            {
                return false;
            }

            var pa = a.GetParameters();
            var pb = b.GetParameters();

            if (pa.Length != pb.Length)
            {
                return false;
            }

            for (int i = 0; i < pa.Length; i++)
            {
                if (pa[i].ParameterType != pb[i].ParameterType)
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetVisibility(FieldInfo f)
        {
            if (f.IsPublic)
            {
                return "public";
            }
            if (f.IsPrivate)
            {
                return "private";
            }
            if (f.IsFamily)
            {
                return "protected";
            }
            return "internal";
        }

        private static string GetVisibility(MethodInfo m)
        {
            if (m.IsPublic)
            {
                return "public";
            }
            if (m.IsPrivate)
            {
                return "private";
            }
            if (m.IsFamily)
            {
                return "protected";
            }
            return "internal";
        }
    }
}