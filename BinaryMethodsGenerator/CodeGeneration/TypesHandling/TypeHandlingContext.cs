using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling
{
    internal sealed class TypeHandlingContext
    {
        public IType Type;
        public String TypeName;
        public ITypeOwner TypeOwner;
        public String TypeOwnerName;
        public String SizeVariableKey = VariableKeys.Size;

        public TypeHandlingContext(CSharpGeneratorContext generatorContext)
        {
            GeneratorContext = generatorContext;
            ElementFactory = CSharpElementFactory.GetInstance(generatorContext.Root.GetPsiModule());
            Builder = new StringBuilder();
            Args = new List<Object>();
            Variables = new VariablesTracker();
        }

        public TypeHandlingContext(TypeHandlingContext other)
        {
            ElementFactory = other.ElementFactory;
            GeneratorContext = other.GeneratorContext;
            Builder = other.Builder;
            Args = other.Args;
            Variables = other.Variables;
            TypeOwner = other.TypeOwner;
            TypeOwnerName = other.TypeOwnerName;
            Type = other.Type;
            TypeName = other.TypeName;
            SizeVariableKey = other.SizeVariableKey;
        }

        public CSharpElementFactory ElementFactory { get; private set; }
        public CSharpGeneratorContext GeneratorContext { get; private set; }
        public StringBuilder Builder { get; private set; }
        public IList<Object> Args { get; private set; }
        public VariablesTracker Variables { get; private set; }

        public PsiLanguageType PresentationLanguage
        {
            get { return GeneratorContext.Language; }
        }

        public void Resolve(ITypeOwner typeOwner)
        {
            TypeOwner = typeOwner;
            Type = typeOwner.Type;
            TypeName = typeOwner.Type.GetPresentableName(typeOwner.PresentationLanguage);
            TypeOwnerName = TypeOwner.ShortName;
        }

        public TypeHandlingContext WithType(IType type)
        {
            var context = new TypeHandlingContext(this)
            {
                Type = type,
                TypeName = type.GetPresentableName(GeneratorContext.Language)
            };
            return context;
        }

        public TypeHandlingContext Inherit()
        {
            return new TypeHandlingContext(this);
        }

        public String GetSizeVariableName()
        {
            return Variables.Use(SizeVariableKey);
        }
    }
}