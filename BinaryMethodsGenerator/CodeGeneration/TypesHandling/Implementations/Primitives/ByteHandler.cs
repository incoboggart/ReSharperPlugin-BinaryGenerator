using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Primitives
{
    internal sealed class ByteHandler : IEnumBaseTypeHandler
    {
        #region ITypeHandler members

        public bool BinarySizePersistent
        {
            get { return true; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            return context.Type.IsByte();
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{0}++;", context.GetSizeVariableName());
        }

        public void HandleRead(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{0} = bytes[index];", context.TypeOwnerName);
            context.Builder.Append("index += sizeof(Byte);");
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("bytes[index] = {0};", context.TypeOwnerName);
            context.Builder.Append("index += sizeof(Byte);");
        }

        #endregion

        #region IEnumBaseTypeHandler

        public void HandleEnumRead(ITypeOwner element, StringBuilder builder, IList<object> args)
        {
            int fieldIndex = args.Count;
            args.Add(element);
            int typeIndex = args.Count;
            args.Add(element.Type);

            builder.AppendFormat("${0} = (${1})bytes[index];", fieldIndex, typeIndex);
            builder.AppendFormat("index += sizeof(${0});", typeIndex);
        }

        public void HandleEnumWrite(ITypeOwner element, StringBuilder builder, IList<object> args)
        {
            int fieldIndex = args.Count;
            args.Add(element);
            int typeIndex = args.Count;
            args.Add(element.Type);

            builder.AppendFormat("bytes[index] = (${1})${0};", fieldIndex, typeIndex);
            builder.AppendFormat("index += sizeof(${0});", typeIndex);
        }

        #endregion
    }
}