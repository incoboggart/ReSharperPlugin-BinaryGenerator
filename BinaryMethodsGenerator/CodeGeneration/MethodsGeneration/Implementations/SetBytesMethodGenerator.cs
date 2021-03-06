﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration.Implementations
{
    internal sealed class SetBytesMethodGenerator : IMethodGenerator
    {
        private static IType _byteArrayType;
        private static ITypeConversionRule _conversionRule;

        public const String MethodName = "SetBytes";

        public static void Initialize(CSharpGeneratorContext context)
        {
            IDeclaredType byteType = TypeFactory.CreateTypeByCLRName(PredefinedType.BYTE_FQN, context.PsiModule, context.Anchor.GetResolveContext());
            _byteArrayType = TypeFactory.CreateArrayType(byteType, 1);
            _conversionRule = new CSharpTypeConversionRule(context.PsiModule);
        }

        public bool IsGenerationTarget(IMethod method)
        {
            return method.ShortName == MethodName &&
                   method.ReturnType.IsVoid() &&
                   method.Parameters.Count == 1 &&
                   method.Parameters[0].Type.IsImplicitlyConvertibleTo(_byteArrayType, _conversionRule);
        }

        public void Generate(CSharpGeneratorContext context, IList<GeneratorDeclaredElement<ITypeOwner>> elements, CSharpElementFactory factory)
        {
            IMethodDeclaration methodDeclaration = null;

            ITypeElement declaredElement = context.ClassDeclaration.DeclaredElement;

            if (declaredElement != null)
            {
                IOverridableMember member = declaredElement.Methods.FirstOrDefault(IsGenerationTarget);
                if (member != null)
                {
                    methodDeclaration = (IMethodDeclaration)member.GetDeclarations().FirstOrDefault();
                    Generate(context, methodDeclaration, elements, factory);
                    return;
                }
            }
            string method = String.Format("public void {0}(Byte[] bytes)", MethodName);
            methodDeclaration = (IMethodDeclaration)factory.CreateTypeMemberDeclaration(method);
            Generate(context, methodDeclaration, elements, factory);
            context.PutMemberDeclaration(methodDeclaration, null, newDeclaration => new GeneratorDeclarationElement(newDeclaration));
        }

        private void Generate(CSharpGeneratorContext context,
            IMethodDeclaration declaration,
            IList<GeneratorDeclaredElement<ITypeOwner>> elements,
            CSharpElementFactory factory)
        {
            var builder = new StringBuilder();
            builder.Append("{").AppendFormat("{0}(bytes, 0);", ReadMethodGenerator.Instance.MethodName).Append("}");

            IBlock body = factory.CreateBlock(builder.ToString());
            declaration.SetBody(body);
        }
    }
}
