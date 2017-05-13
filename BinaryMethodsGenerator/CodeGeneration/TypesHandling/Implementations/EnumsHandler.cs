using System;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations
{
    internal sealed class EnumsHandler : ITypeHandler
    {
        #region ITypeHandler members
        public bool BinarySizePersistent
        {
            get { return true; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            return context.Type.IsEnumType();
        }

        public void HandleRead(TypeHandlingContext context)
        {
            IEnum type = context.Type.GetEnumType();

            if (type == null)
            {
                throw new NotSupportedException();
            }

            IType baseType = type.GetUnderlyingType();
            IEnumBaseTypeHandler handler =
                TypeHandlers.EnumBaseTypeHandlers.SingleOrDefault(h => h.CanHandle(context.WithType(baseType)));

            if (handler == null)
            {
                throw new NotSupportedException();
            }

            handler.HandleEnumRead(context.TypeOwner, context.Builder, context.Args);
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            IEnum enumType = context.Type.GetEnumType();

            if (enumType == null)
            {
                throw new NotSupportedException();
            }

            IType baseType = enumType.GetUnderlyingType();
            IEnumBaseTypeHandler handler =
                TypeHandlers.EnumBaseTypeHandlers.SingleOrDefault(h => h.CanHandle(context.WithType(baseType)));

            if (handler == null)
            {
                throw new NotImplementedException();
            }

            handler.HandleEnumWrite(context.TypeOwner, context.Builder, context.Args);
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            IEnum enumType = context.Type.GetEnumType();

            if (enumType == null)
            {
                throw new NotSupportedException();
            }

            IType baseType = enumType.GetUnderlyingType();

            context.Builder.AppendFormat("{1} += sizeof({0});", baseType.GetPresentableName(context.PresentationLanguage), context.GetSizeVariableName());
        } 
        #endregion
    }
}