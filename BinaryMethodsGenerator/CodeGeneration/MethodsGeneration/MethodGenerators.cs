using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration.Implementations;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration
{
    internal sealed class MethodGenerators
    {
        public static readonly IMethodGenerator[] All =
        {
            GetBinarySizeMethodGenerator.Instance,
            ReadMethodGenerator.Instance,
            WriteMethodGenerator.Instance,
            new GetBytesMethodGenerator(),
            new SetBytesMethodGenerator(),
        };
    }
}
