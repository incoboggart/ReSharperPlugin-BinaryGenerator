using System;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Primitives
{
    internal sealed class DateTimeHandler : ITypeHandler
    {
        private readonly Predicate<TypeHandlingContext> _predicate;
        private readonly String _typeName;

        public DateTimeHandler(String typeName, Predicate<TypeHandlingContext> predicate)
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
            context.Builder.AppendFormat("{0} = new {1}(BitConverter.ToInt64(bytes, index));", context.TypeOwnerName,
                _typeName);
            context.Builder.Append("index += sizeof(Int64);");
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("tmp = BitConverter.GetBytes({0}.Ticks);", context.TypeOwnerName);
            context.Builder.Append("Array.Copy(tmp, 0, bytes, index, tmp.Length);");
            context.Builder.Append("index += tmp.Length;");
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{0} += sizeof(Int64);", context.GetSizeVariableName());
        }

        #endregion
    }
}