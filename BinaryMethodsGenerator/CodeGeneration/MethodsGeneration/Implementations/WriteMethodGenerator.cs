using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration.Implementations
{
    internal sealed class WriteMethodGenerator : MethodGeneratorBase
    {
        public static readonly MethodGeneratorBase Instance = new WriteMethodGenerator();

        private WriteMethodGenerator()
        {
        }

        public override string MethodName
        {
            get
            {
                const String name = "BinaryWrite";
                return name;
            }
        }

        protected override void Generate(CSharpGeneratorContext context,
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
            ctx.Builder.Append("{");
            ctx.Builder.AppendFormat("var index = startIndex;");
            ctx.Builder.AppendFormat("Byte[] tmp;");
            if (elements.Count > 0)
            {
                foreach (var element in elements)
                {
                    ctx.Resolve(element.DeclaredElement);

                    ITypeHandler handler = TypeHandlers.All.FirstOrDefault(h => h.CanHandle(ctx));
                    if (handler != null)
                    {
                        handler.HandleWrite(ctx);
                    }
                }
            }
            ctx.Builder.Append("return index;").Append("}");

            IBlock body = factory.CreateBlock(ctx.Builder.ToString(), ctx.Args.ToArray());

            declaration.SetBody(body);
        }
    }
}