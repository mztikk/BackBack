using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Sharpie;
using Sharpie.Writer;

namespace BackBack.Generator.Registrants
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            Class mvvmRegistrant = new Class("MVVMRegistrant")
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

            string str = ClassWriter.Write(mvvmRegistrant);

            context.AddSource(mvvmRegistrant.ClassName, SourceText.From(str, Encoding.UTF8));

            Class serviceRegistree = new Class("ServiceRegistree")
                .WithAccessibility(Accessibility.Internal)
                .SetStatic(true)
                .SetNamespace("BackBack")
                .WithUsings("System", "BackBack.Common", "LightInject")
                .WithMethod(new(Accessibility.Internal, Static: true, async: false, "void", "Register", new Parameter[] { new("ServiceContainer", "container") }, (bodyWriter) =>
                {
                    bodyWriter.WriteLine($"new {mvvmRegistrant.ClassName}().Register(container);");

                    INamedTypeSymbol? IServiceRegistrant = context.Compilation.GetTypeByMetadataName("BackBack.Common.IServiceRegistrant");
                    if (IServiceRegistrant is null)
                    {
                        return;
                    }

                    IEnumerable<INamedTypeSymbol> types = GetAllTypes(context.Compilation).Where(x => x.AllInterfaces.Contains(IServiceRegistrant));
                    if (!types.Any())
                    {
                        return;
                    }

                    foreach (INamedTypeSymbol type in types)
                    {
                        bodyWriter.WriteLine($"new {type}().Register(container);");
                    }
                }));

            context.AddSource(serviceRegistree.ClassName, SourceText.From(ClassWriter.Write(serviceRegistree), Encoding.UTF8));
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
