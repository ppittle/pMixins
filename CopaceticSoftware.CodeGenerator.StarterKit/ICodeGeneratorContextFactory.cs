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
        private readonly ISolutionExtender _solutionExtender;

        public CodeGeneratorContextFactory(ISolutionExtender solutionExtender)
        {
            Ensure.ArgumentNotNull(solutionExtender, "solutionExtender");

            _solutionExtender = solutionExtender;
        }

        public ICodeGeneratorContext GenerateContext(
            string sourceCode, string sourceFileName, string projectFilePath)
        {
            var sourceFile =
                _solutionExtender.AddOrUpdateProjectItemFile(
                    projectFilePath, sourceFileName, sourceCode);

            return new CodeGeneratorContext
                       {
                           Source = sourceFile,
                           TypeResolver = sourceFile.CreateResolver()
                       };
        }
    }
}