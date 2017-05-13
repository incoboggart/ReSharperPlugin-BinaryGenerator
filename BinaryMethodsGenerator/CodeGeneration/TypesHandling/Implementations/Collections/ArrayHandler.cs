using System;
using System.Linq;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Collections
{
    internal sealed class ArrayHandler : ITypeHandler
    {
        private const String Index = "index";
        private const String Length = "length";

        #region ITypeHandler members
        public bool BinarySizePersistent
        {
            get { return false; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            var arrayType = context.Type as IArrayType;

            if (arrayType != null && arrayType.Rank == 1)
            {
                var scalarType = context.Type.GetScalarType();
                if (scalarType != null)
                {
                    return TypeHandlers.All.Any(h => h.CanHandle(context.WithType(scalarType)));
                }
            }

            return false;
        }

        public void HandleRead(TypeHandlingContext context)
        {
            IDeclaredType valueType = context.Type.GetScalarType();

            if (valueType == null)
            {
                throw new NotSupportedException();
            }

            ITypeHandler valueHandler = TypeHandlers.All.SingleOrDefault(h => h.CanHandle(context.WithType(valueType)));

            if (valueHandler == null)
            {
                throw new NotSupportedException();
            }

            var indexName = context.Variables.Declare(Index);
            var lengthName = context.Variables.Declare(Length);
            var exists = context.Variables.Declare(VariableKeys.NotNull);

            TypeHandlers.Boolean.HandleRead(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("var {0}", exists)
            });
            context.Builder.AppendFormat("if({0})", exists);
            context.Builder.Append("{");
            context.Builder.AppendFormat("var {0} = BitConverter.ToInt32(bytes, index);", lengthName);
            context.Builder.Append("index += sizeof(Int32);");
            context.Builder.AppendFormat("{0} = new {1}[{2}];", context.TypeOwnerName, valueType.GetPresentableName(context.PresentationLanguage), lengthName);
            context.Builder.AppendFormat("if({0} > 0)", lengthName);
            context.Builder.Append("{");
            context.Builder.AppendFormat("for(Int32 {0} = 0; {0} < {1}; {0}++)", indexName, lengthName);
            context.Builder.Append("{");
            valueHandler.HandleRead(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("{0}[{1}]", context.TypeOwnerName, indexName),
                Type = valueType,
                TypeName = valueType.GetPresentableName(context.PresentationLanguage),
                TypeOwner = context.TypeOwner
            });
            context.Builder.Append("}}}");

            context.Variables.Dispose(Index);
            context.Variables.Dispose(Length);
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            IDeclaredType valueType = context.Type.GetScalarType();

            if (valueType == null)
            {
                throw new NotSupportedException();
            }

            ITypeHandler valueHandler = TypeHandlers.All.SingleOrDefault(h => h.CanHandle(context.WithType(valueType)));

            if (valueHandler == null)
            {
                throw new NotSupportedException();
            }

            String indexName = context.Variables.Declare(Index);

            var exists = context.Variables.Declare(VariableKeys.NotNull);

            context.Builder.AppendFormat("var {0} = {1} != null;", exists, context.TypeOwnerName);
            TypeHandlers.Boolean.HandleWrite(new TypeHandlingContext(context)
            {
                TypeOwnerName = exists
            });
            context.Builder.AppendFormat("if({0})", exists);
            context.Builder.Append("{");
            TypeHandlers.Int32.HandleWrite(new TypeHandlingContext(context)
            {
                TypeName = TypeHandlers.Int32.TypeName,
                TypeOwnerName = String.Format("{0}.Length", context.TypeOwnerName)
            });
            context.Builder.AppendFormat("if({0}.Length > 0)", context.TypeOwnerName);
            context.Builder.Append("{");
            context.Builder.AppendFormat("for(Int32 {0} = 0; {0} < {1}.Length; {0}++)", indexName, context.TypeOwnerName);
            context.Builder.Append("{");
            valueHandler.HandleWrite(new TypeHandlingContext(context)
            {
                TypeName = valueType.GetPresentableName(context.PresentationLanguage),
                TypeOwnerName = String.Format("{0}[{1}]", context.TypeOwnerName, indexName)
            });
            context.Builder.Append("}}}");
            context.Variables.Dispose(Index);
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            IDeclaredType valueType = context.Type.GetScalarType();

            if (valueType == null)
            {
                throw new NotSupportedException();
            }

            ITypeHandler valueHandler = TypeHandlers.All.SingleOrDefault(h => h.CanHandle(context.WithType(valueType)));

            if (valueHandler == null)
            {
                throw new NotSupportedException();
            }

            var exists = context.Variables.Declare(VariableKeys.NotNull);

            context.Builder.AppendFormat("var {0} = {1} != null;", exists, context.TypeOwnerName);
            TypeHandlers.Boolean.HandleGetSize(new TypeHandlingContext(context)
            {
                TypeOwnerName = exists
            });

            context.Builder.AppendFormat("if({0})", exists).Append("{");
            TypeHandlers.Int32.HandleGetSize(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("{0}.Length", context.TypeOwnerName)
            });
            context.Builder.AppendFormat("if({0}.Length > 0)", context.TypeOwnerName);
            context.Builder.Append("{");
            if (valueHandler.BinarySizePersistent)
            {
                var size = context.Variables.Declare(VariableKeys.ValueSize);

                context.Builder.AppendFormat("var {0} = 0;", size);

                valueHandler.HandleGetSize(new TypeHandlingContext(context)
                {
                    SizeVariableKey = VariableKeys.ValueSize
                });

                context.Builder.AppendFormat("{0} += {1}*{2}.Length;", context.GetSizeVariableName(), size, context.TypeOwnerName);
            }
            else
            {
                var indexName = context.Variables.Declare(Index);
                context.Builder.AppendFormat("for(Int32 {0} = 0; {0} < {1}.Length; {0}++)", indexName, context.TypeOwnerName);
                context.Builder.Append("{");
                valueHandler.HandleGetSize(new TypeHandlingContext(context)
                {
                    TypeOwnerName = String.Format("{0}[{1}]", context.TypeOwnerName, indexName)
                });
                context.Builder.Append("}");
            }
            context.Builder.Append("}}");
        } 
        #endregion
    }
}