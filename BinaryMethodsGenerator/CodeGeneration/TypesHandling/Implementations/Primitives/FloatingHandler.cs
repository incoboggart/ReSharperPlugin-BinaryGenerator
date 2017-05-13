using System;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Primitives
{
    internal sealed class FloatingHandler : ITypeHandler
    {
        private readonly Predicate<TypeHandlingContext> _predicate;
        private readonly String _typeName;

        public FloatingHandler(String typeName, Predicate<TypeHandlingContext> predicate)
        {
            _typeName = typeName;
            _predicate = predicate;
        }

        #region ITypeHandler members

        public bool BinarySizePersistent
        {
            get { return true; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            return _predicate(context);
        }

        public void HandleRead(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{0} = BitConverter.To{1}(bytes, index);", context.TypeOwnerName, _typeName);
            context.Builder.AppendFormat("index += sizeof({0});", _typeName);
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("tmp = BitConverter.GetBytes({0});", context.TypeOwnerName);
            context.Builder.Append("Array.Copy(tmp, 0, bytes, index, tmp.Length);");
            context.Builder.Append("index += tmp.Length;");
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{1} += sizeof({0});", _typeName, context.GetSizeVariableName());
        } 
        #endregion
    }
}