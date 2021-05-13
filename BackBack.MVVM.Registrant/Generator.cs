using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Sharpie;
using Sharpie.Writer;

namespace BackBack.MVVM.Registrant
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            Class generatedClass = new Class("MVVMRegistrant")
                .WithBaseClass("IServiceRegistrant")
                .WithUsings("System", "BackBack.Common", "BackBack.Views", "BackBack.ViewModels", "LightInject")
                .SetNamespace("BackBack")
                .WithAccessibility(Accessibility.Internal)
                .WithMethod(new(Accessibility.Public, "void", "Register", new Parameter[] { new("ServiceContainer", "container") }, (bodyWriter) =>
               {
                   IEnumerable<INamedTypeSymbol> types = GetMVVMTypes(context.Compilation);
                   if (!types.Any())
                   {
                       return;
                   }

                   bodyWriter.Write("container");
                   bodyWriter.IndentationLevel++;
                   foreach (INamedTypeSymbol type in types)
                   {
                       bodyWriter.WriteLine();
                       bodyWriter.Write($".Register<{type.Name}>()");
                   }
                   bodyWriter.IndentationLevel--;

                   bodyWriter.EndStatement();
               }));

            string str = ClassWriter.Write(generatedClass);

            context.AddSource(generatedClass.ClassName, SourceText.From(str, Encoding.UTF8));
        }

        private static IEnumerable<INamedTypeSymbol> GetMVVMTypes(Compilation compilation)
        {
            var namespaces = new HashSet<string>() { "BackBack.ViewModels", "BackBack.Views" };
            return GetAllTypes(compilation).Where(x => namespaces.Contains(x.ContainingNamespace.ToString()));
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation)
        {
            foreach (INamedTypeSymbol symbol in GetAllPublicTypes(compilation.Assembly.GlobalNamespace))
            {
                yield return symbol;
            }

            foreach (MetadataReference item in compilation.References)
            {
                if (compilation.GetAssemblyOrModuleSymbol(item) is IAssemblySymbol assemblySymbol)
                {
                    foreach (INamedTypeSymbol symbol in GetAllPublicTypes(assemblySymbol.GlobalNamespace))
                    {
                        yield return symbol;
                    }
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetAllPublicTypes(params INamespaceOrTypeSymbol[] symbols)
        {
            var stack = new Stack<INamespaceOrTypeSymbol>(symbols);

            while (stack.Count > 0)
            {
                INamespaceOrTypeSymbol item = stack.Pop();

                if (item is INamedTypeSymbol type && type.DeclaredAccessibility == Accessibility.Public)
                {
                    yield return type;
                }

                foreach (ISymbol member in item.GetMembers())
                {
                    if (member is INamespaceOrTypeSymbol child && child.DeclaredAccessibility == Accessibility.Public && (member is not INamedTypeSymbol typeSymbol || typeSymbol.TypeParameters.Length == 0))
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
