using JetBrains.ReSharper.Psi;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Primitives
{
    internal sealed class BooleanHandler : ITypeHandler
    {
        #region ITypeHandler members
        public bool BinarySizePersistent
        {
            get { return true; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            return context.Type.IsBool();
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("tmp = BitConverter.GetBytes({0});", context.TypeOwnerName);
            context.Builder.Append("Array.Copy(tmp, 0, bytes, index, tmp.Length);");
            context.Builder.Append("index += tmp.Length;");
        }

        public void HandleRead(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{0} = BitConverter.ToBoolean(bytes, index);", context.TypeOwnerName);
            context.Builder.AppendFormat("index += sizeof(Boolean);");
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{0} += sizeof(Boolean);", context.GetSizeVariableName());
        } 
        #endregion
    }
}