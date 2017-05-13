using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration.Implementations;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Collections;

namespace ReSharperPlugins.BinaryMethodsGenerator
{
    [GeneratorElementProvider("BinaryMethods", typeof (CSharpLanguage))]
    public class BinaryMethodsGeneratorProvider : GeneratorProviderBase<CSharpGeneratorContext>
    {
        public override double Priority
        {
            get { return 0; }
        }

        public override void Populate(CSharpGeneratorContext context)
        {
            // use context.ProvidedElements.AddRange to add new
            // generator elements (e.g., GeneratorDeclaredElement<T>)
            IClassLikeDeclaration typeDeclaration = context.ClassDeclaration;
            ITypeElement typeElement = typeDeclaration.DeclaredElement;
            ListHandler.Initialize(context);
            MethodGeneratorBase.Initialize(context);
            GetBytesMethodGenerator.Initialize(context);
            SetBytesMethodGenerator.Initialize(context);

            if (typeElement is IClass || typeElement is IStruct)
            {
                var ctx = new TypeHandlingContext(context);

                foreach (ITypeMember member in typeElement.GetMembers())
                {
                    ITypeOwner owner = null;

                    var field = member as IField;
                    if (field != null)
                    {
                        if (field.GetAccessRights() != AccessRights.PRIVATE &&
                            !field.IsConstant &&
                            !field.IsReadonly &&
                            !field.IsStatic)
                        {
                            owner = field;
                        }
                    }

                    var property = member as IProperty;
                    if (property != null)
                    {
                        if (property.IsReadable &&
                            property.IsWritable && 
                            !property.IsStatic)
                        {
                            owner = property;
                        }
                    }

                    if (owner != null)
                    {
                        ctx.Resolve(owner);

                        if (TypeHandlers.All.Any(h => h.CanHandle(ctx)))
                        {
                            context.ProvidedElements.Add(new GeneratorDeclaredElement<ITypeOwner>(owner));
                        }
                    }
                }
            }
        }
    }
}