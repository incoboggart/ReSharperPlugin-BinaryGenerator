using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Collections
{
    internal sealed class ListHandler : ITypeHandler
    {
        private static IDeclaredType _listDeclaredType;
        private static ITypeElement _listTypeElement;
        
        public static void Initialize(CSharpGeneratorContext context)
        {
            _listDeclaredType = TypeFactory.CreateTypeByCLRName(typeof(List<>).FullName, context.PsiModule,
                context.Anchor.GetResolveContext());
            _listTypeElement = _listDeclaredType.GetTypeElement();
        }

        #region ITypeHandler members

        public bool BinarySizePersistent
        {
            get { return false; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            var valueType = context.Type.GetGenericUnderlyingType(_listTypeElement);
            return valueType != null && TypeHandlers.All.Any(h => h.CanHandle(context.WithType(valueType)));
        }

        public void HandleRead(TypeHandlingContext context)
        {
            var valueType = context.Type.GetGenericUnderlyingType(_listTypeElement);
            var valueHandler = TypeHandlers.All.First(h => h.CanHandle(context.WithType(valueType)));
            var count = context.Variables.Declare("count");
            var existed = context.Variables.Declare("existed");
            context.Builder.Append("{");
            TypeHandlers.Boolean.HandleRead(new TypeHandlingContext(context)
            {
                TypeOwnerName = "var " + existed
            });
            context.Builder.AppendFormat("if({0})", existed);
            context.Builder.Append("{");
            TypeHandlers.Int32.HandleRead(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("var {0} ", count)
            });
            context.Builder.AppendFormat("if({0} > 0)", count);
            context.Builder.Append("{");
            context.Builder.AppendFormat("{0} = new List<{2}>({1});", context.TypeOwnerName, count, valueType);
            context.Builder.Append("} else {");
            context.Builder.AppendFormat("{0} = new List<{1}>();", context.TypeOwnerName, valueType);
            context.Builder.Append("}");
            context.Builder.AppendFormat("for(Int32 i = 0; i < {0}; i++)", count);
            context.Builder.Append("{");
            context.Builder.AppendFormat("var {0} = default({1});", VariableKeys.Value, valueType.GetPresentableName(context.PresentationLanguage));
            valueHandler.HandleRead(new TypeHandlingContext(context)
            {
                TypeOwnerName = VariableKeys.Value,
                TypeName = valueType.GetPresentableName(context.PresentationLanguage)
            });
            context.Builder.AppendFormat("{0}.Add(value);", context.TypeOwnerName);
            context.Builder.Append("}}}");
            context.Variables.Dispose("count");
            context.Variables.Dispose("existed");
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            var valueType = context.Type.GetGenericUnderlyingType(_listTypeElement);
            var valueHandler = TypeHandlers.All.First(h => h.CanHandle(context.WithType(valueType)));

            var exists = context.Variables.Declare("exists");
            var index = context.Variables.Declare("index");

            context.Builder.AppendFormat("var {0} = {1} != null;", exists, context.TypeOwnerName);
            TypeHandlers.Boolean.HandleWrite(new TypeHandlingContext(context)
            {
                TypeOwnerName = exists,
            });
            context.Builder.AppendFormat("if({0})", exists);
            context.Builder.Append("{");
            TypeHandlers.Int32.HandleWrite(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("{0}.Count", context.TypeOwnerName)
            });
            context.Builder.AppendFormat("for(Int32 {0} = 0; {0} < {1}.Count; {0}++)", index, context.TypeOwnerName);
            context.Builder.Append("{");
            valueHandler.HandleWrite(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("{0}[{1}]", context.TypeOwnerName, index)
            });
            context.Builder.Append("}");
            context.Builder.Append("}");

            context.Variables.Dispose("index");
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            var valueType = context.Type.GetGenericUnderlyingType(_listTypeElement);
            var valueHandler = TypeHandlers.All.First(h => h.CanHandle(context.WithType(valueType)));

            TypeHandlers.Boolean.HandleGetSize(context);
            context.Builder.AppendFormat("if({0} != null)", context.TypeOwnerName);
            context.Builder.Append("{");
            TypeHandlers.Int32.HandleGetSize(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("{0}.Length", context.TypeOwnerName)
            });

            if (valueHandler.BinarySizePersistent)
            {
                var valueSize = context.Variables.Declare(VariableKeys.ValueSize);

                context.Builder.AppendFormat("var {0} = 0;", valueSize);
                valueHandler.HandleGetSize(new TypeHandlingContext(context)
                {
                    SizeVariableKey = VariableKeys.ValueSize
                });

                context.Builder.AppendFormat("{0} += {1}*{2}.Count;", context.GetSizeVariableName(), valueSize, context.TypeOwnerName);
            }
            else
            {
                var i = context.Variables.Declare("i");
                var v = context.Variables.Declare("v");
                context.Builder.AppendFormat("for(Int32 {0} = 0; {0} < {1}.Count; {0}++)", i, context.TypeOwnerName);
                context.Builder.Append("{");
                context.Builder.AppendFormat("var {0} = {1}[{2}];", v, context.TypeOwnerName, i);
                valueHandler.HandleGetSize(new TypeHandlingContext(context)
                {
                    TypeOwnerName = v
                });
                context.Builder.Append("}");
                context.Variables.Dispose("v");
                context.Variables.Dispose("i");
            }
            context.Builder.Append("}");
        }

        #endregion
    }
}
