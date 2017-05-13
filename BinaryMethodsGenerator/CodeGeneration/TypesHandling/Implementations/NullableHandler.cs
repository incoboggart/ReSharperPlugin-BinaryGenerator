using System;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations
{
    internal sealed class NullableHandler : ITypeHandler
    {
        #region ITypeHandler members
        public bool BinarySizePersistent
        {
            get { return false; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            return context.Type.IsNullable();
        }

        public void HandleRead(TypeHandlingContext context)
        {
            IType type = context.Type;

            IType valueType = type.GetNullableUnderlyingType();

            if (valueType == null)
            {
                throw new NotSupportedException();
            }

            var valueHandlingContext = new TypeHandlingContext(context)
            {
                Type = valueType,
                TypeName = valueType.GetPresentableName(context.PresentationLanguage),
                TypeOwnerName = String.Format("{0}", context.TypeOwnerName)
            };

            ITypeHandler valueHandler = TypeHandlers.All.FirstOrDefault(h => h.CanHandle(valueHandlingContext));

            if (valueHandler == null)
            {
                throw new NotSupportedException();
            }

            context.Builder.Append("if(BitConverter.ToBoolean(bytes, index)){");
            context.Builder.Append("index += sizeof(Boolean);");
            valueHandler.HandleRead(valueHandlingContext);
            context.Builder.Append("}");
            context.Builder.Append("else{");
            context.Builder.Append("index += sizeof(Boolean);");
            context.Builder.Append("}");
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            IType valueType = context.Type.GetNullableUnderlyingType();

            if (valueType == null)
            {
                throw new NotSupportedException();
            }

            var valueHandlingContext = new TypeHandlingContext(context)
            {
                Type = valueType,
                TypeName = valueType.GetPresentableName(context.PresentationLanguage),
                TypeOwnerName = String.Format("{0}.Value", context.TypeOwnerName)
            };

            ITypeHandler valueHandler = TypeHandlers.All.FirstOrDefault(h => h.CanHandle(valueHandlingContext));

            if (valueHandler == null)
            {
                throw new NotSupportedException();
            }

            context.Builder.AppendFormat("tmp = BitConverter.GetBytes({0}.HasValue);", context.TypeOwnerName);
            context.Builder.Append("Array.Copy(tmp, 0, bytes, index, tmp.Length);");
            context.Builder.Append("index += tmp.Length;");
            context.Builder.AppendFormat("if({0}.HasValue)", context.TypeOwnerName).Append("{");
            valueHandler.HandleWrite(valueHandlingContext);
            context.Builder.Append("}");
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            IType valueType = context.Type.GetNullableUnderlyingType();

            if (valueType == null)
            {
                throw new NotSupportedException();
            }

            var valueHandlingContext = new TypeHandlingContext(context)
            {
                Type = valueType,
                TypeName = valueType.GetPresentableName(context.PresentationLanguage),
                TypeOwnerName = String.Format("{0}.Value", context.TypeOwnerName),
            };
            ITypeHandler valueHandler = TypeHandlers.All.FirstOrDefault(h => h.CanHandle(valueHandlingContext));

            if (valueHandler == null)
            {
                throw new NotSupportedException();
            }

            context.Builder.AppendFormat("{0} += sizeof(Boolean);", context.GetSizeVariableName());
            valueHandler.HandleGetSize(context);
        } 
        #endregion
    }
}