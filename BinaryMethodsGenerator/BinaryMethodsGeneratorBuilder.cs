using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration;

namespace ReSharperPlugins.BinaryMethodsGenerator
{
    [GeneratorBuilder("BinaryMethods", typeof (CSharpLanguage))]
    public class BinaryMethodsGeneratorBuilder : GeneratorBuilderBase<CSharpGeneratorContext>
    {
        private readonly CodeAnnotationsCache _codeAnnotationsCache;

        public BinaryMethodsGeneratorBuilder(CodeAnnotationsCache codeAnnotationsCache)
        {
            _codeAnnotationsCache = codeAnnotationsCache;
        }

        public override double Priority
        {
            get { return 0; }
        }

        protected override void Process(CSharpGeneratorContext context)
        {
            // this is where you build new code
            // to modify, e.g., an existing class, use context.ClassDeclaration

            CSharpElementFactory elementFactory = CSharpElementFactory.GetInstance(context.Root.GetPsiModule());
            GeneratorDeclaredElement<ITypeOwner>[] typeOwners = context.InputElements.OfType<GeneratorDeclaredElement<ITypeOwner>>().ToArray();
            foreach (var methodGenerator in MethodGenerators.All)
            {
                methodGenerator.Generate(context, typeOwners, elementFactory);
            }
        }
    }
}