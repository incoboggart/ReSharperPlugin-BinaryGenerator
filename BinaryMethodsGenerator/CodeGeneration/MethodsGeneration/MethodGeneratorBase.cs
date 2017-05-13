using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration
{
    internal abstract class MethodGeneratorBase : IMethodGenerator
    {
        private static IType _byteArrayType;
        private static ITypeConversionRule _typeConversionRule;

        public static void Initialize(CSharpGeneratorContext context)
        {
            IDeclaredType byteType = TypeFactory.CreateTypeByCLRName(PredefinedType.BYTE_FQN, context.PsiModule, context.Anchor.GetResolveContext());
            _byteArrayType = TypeFactory.CreateArrayType(byteType, 1);
            _typeConversionRule = new CSharpTypeConversionRule(context.PsiModule);
        }

        public abstract String MethodName { get; }

        public Boolean IsGenerationTarget(IMethod method)
        {
            return ValidateMethod(method);
        }

        public void Generate(CSharpGeneratorContext context,
            IList<GeneratorDeclaredElement<ITypeOwner>> elements,
            CSharpElementFactory factory)
        {
            IMethodDeclaration methodDeclaration = null;

            ITypeElement declaredElement = context.ClassDeclaration.DeclaredElement;

            if (declaredElement != null)
            {
                IDeclaredType byteType = TypeFactory.CreateTypeByCLRName(PredefinedType.BYTE_FQN, context.PsiModule,
                    context.Anchor.GetResolveContext());
                _byteArrayType = TypeFactory.CreateArrayType(byteType, 1);
                _typeConversionRule = new CSharpTypeConversionRule(context.PsiModule);
                IOverridableMember member = declaredElement.Methods.FirstOrDefault(ValidateMethod);
                if (member != null)
                {
                    methodDeclaration = (IMethodDeclaration) member.GetDeclarations().FirstOrDefault();
                    Generate(context, methodDeclaration, elements, factory);
                    return;
                }
            }
            string method = String.Format("public Int32 {0}(Byte[] bytes, Int32 startIndex)", MethodName);
            methodDeclaration = (IMethodDeclaration) factory.CreateTypeMemberDeclaration(method);
            Generate(context, methodDeclaration, elements, factory);
            context.PutMemberDeclaration(methodDeclaration, null,
                newDeclaration => new GeneratorDeclarationElement(newDeclaration));
        }

        private Boolean ValidateMethod(IMethod method)
        {
            bool found = method != null;
            found = found && method.ShortName == MethodName;
            found = found && method.ReturnType.IsInt();
            found = found && method.Parameters.Count == 2;
            found = found && method.Parameters[0].Type.IsImplicitlyConvertibleTo(_byteArrayType, _typeConversionRule);
            found = found && method.Parameters[1].Type.IsInt();
            return found;
        }

        protected abstract void Generate(CSharpGeneratorContext context, IMethodDeclaration declaration,
            IList<GeneratorDeclaredElement<ITypeOwner>> elements, CSharpElementFactory factory);
    }
}