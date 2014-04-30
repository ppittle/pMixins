using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.Common.Infrastructure;

namespace CopaceticSoftware.CodeGenerator.StarterKit
{
    public interface ICodeGeneratorContextFactory
    {
        ICodeGeneratorContext GenerateContext(string sourceCode, string sourceFileName, string projectFilePath);
    }

    public class CodeGeneratorContextFactory : ICodeGeneratorContextFactory
    {
        private readonly ISolutionManager _solutionManager;

        public CodeGeneratorContextFactory(ISolutionManager solutionManager)
        {
            Ensure.ArgumentNotNull(solutionManager, "solutionManager");

            _solutionManager = solutionManager;
        }

        public ICodeGeneratorContext GenerateContext(
            string sourceCode, string sourceFileName, string projectFilePath)
        {
            var sourceFile =
                _solutionManager.AddOrUpdateCodeGeneratorFileSource(
                    projectFilePath, sourceFileName, sourceCode);

            return new CodeGeneratorContext
                       {
                           Source = sourceFile,
                           TypeResolver = sourceFile.CreateResolver()
                       };
        }
    }
}