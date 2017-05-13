using JetBrains.ReSharper.Psi;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Primitives
{
    internal sealed class GuidHandler : ITypeHandler
    {
        #region ITypeHandler members

        public bool BinarySizePersistent
        {
            get { return true; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            return context.Type.IsGuid();
        }

        public void HandleRead(TypeHandlingContext context)
        {
            context.Builder.Append("tmp = new Byte[16];");
            context.Builder.Append("Array.Copy(bytes, index, tmp, 0, tmp.Length);");
            context.Builder.AppendFormat("{0} = new Guid(tmp);", context.TypeOwnerName);
            context.Builder.Append("index += 16;");
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("tmp = {0}.ToByteArray();", context.TypeOwnerName);
            context.Builder.Append("Array.Copy(tmp, 0, bytes, index, tmp.Length);");
            context.Builder.Append("index += tmp.Length;");
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{0} += 16;", context.GetSizeVariableName());
        } 
        #endregion
    }
}