using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class TypeDefinitionExtensions
    {
        public static bool IsNestedType(this ITypeDefinition typeDefinition)
        {
            return typeDefinition.Name.Contains("+");
        }
    }
}
