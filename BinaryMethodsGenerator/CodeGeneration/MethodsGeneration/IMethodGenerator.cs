using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration
{
    internal interface IMethodGenerator
    {
        Boolean IsGenerationTarget(IMethod method);
        void Generate(CSharpGeneratorContext context,
            IList<GeneratorDeclaredElement<ITypeOwner>> elements,
            CSharpElementFactory factory);
    }
}