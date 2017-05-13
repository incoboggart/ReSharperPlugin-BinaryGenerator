using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration.Implementations
{
    internal sealed class GetBinarySizeMethodGenerator : IMethodGenerator
    {
        public const String MethodName = "GetBinarySize";
        public static readonly GetBinarySizeMethodGenerator Instance = new GetBinarySizeMethodGenerator();

        private GetBinarySizeMethodGenerator()
        {
        }

        public bool IsGenerationTarget(IMethod method)
        {
            return method.ShortName == MethodName &&
                   method.ReturnType.IsInt() &&
                   method.Parameters.Count == 0;
        }

        public void Generate(CSharpGeneratorContext context,
            IList<GeneratorDeclaredElement<ITypeOwner>> elements,
            CSharpElementFactory factory)
        {
            IMethodDeclaration methodDeclaration = null;

            ITypeElement declaredElement = context.ClassDeclaration.DeclaredElement;

            if (declaredElement != null)
            {
                IOverridableMember member = declaredElement.Methods.FirstOrDefault(ValidateMethod);
                if (member != null)
                {
                    methodDeclaration = (IMethodDeclaration) member.GetDeclarations().FirstOrDefault();
                    Generate(context, methodDeclaration, elements, factory);
                    return;
                }
            }
            string method = String.Format("public Int32 {0}()", MethodName);
            methodDeclaration = (IMethodDeclaration) factory.CreateTypeMemberDeclaration(method);
            Generate(context, methodDeclaration, elements, factory);
            context.PutMemberDeclaration(methodDeclaration, null,
                newDeclaration => new GeneratorDeclarationElement(newDeclaration));
        }

        private static Boolean ValidateMethod(IMethod method)
        {
            return method != null &&
                   method.ShortName == MethodName &&
                   method.ReturnType.IsInt() &&
                   method.Parameters.Count == 0;
        }

        private static void Generate(CSharpGeneratorContext context,
            IMethodDeclaration declaration,
            IList<GeneratorDeclaredElement<ITypeOwner>> elements,
            CSharpElementFactory factory)
        {
            var owner = (IParametersOwner) declaration.DeclaredElement;
            if (owner == null)
            {
                return;
            }

            var ctx = new TypeHandlingContext(context);

            var size = ctx.GetSizeVariableName();

            ctx.Builder.Append("{");
            ctx.Builder.AppendFormat("var {0} = 0;", size);
            if (elements.Count > 0)
            {
                foreach (var element in elements)
                {
                    ctx.Resolve(element.DeclaredElement);

                    ITypeHandler handler = TypeHandlers.All.SingleOrDefault(h => h.CanHandle(ctx));
                    if (handler != null)
                    {
                        handler.HandleGetSize(ctx);
                    }
                }
            }
            ctx.Builder.AppendFormat("return {0};", size);
            ctx.Builder.Append("}");

            IBlock body = factory.CreateBlock(ctx.Builder.ToString(), ctx.Args.ToArray());

            declaration.SetBody(body);
        }
    }
}