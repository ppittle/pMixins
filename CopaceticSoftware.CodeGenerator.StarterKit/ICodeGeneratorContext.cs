using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.NRefactory;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace CopaceticSoftware.CodeGenerator.StarterKit
{
    public interface ICodeGeneratorContext
    {
        CSharpFile Source { get; }
        CSharpAstResolver TypeResolver { get; }
        ISolutionManager SolutionManager { get; }
    }

    public class CodeGeneratorContext : ICodeGeneratorContext
    {
        public CSharpFile Source { get; set; }

        public CSharpAstResolver TypeResolver { get; set; }

        public ISolutionManager SolutionManager { get; set; }
    }
}
