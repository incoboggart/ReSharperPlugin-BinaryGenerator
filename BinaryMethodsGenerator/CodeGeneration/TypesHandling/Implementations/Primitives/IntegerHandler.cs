using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Primitives
{
    internal sealed class IntegerHandler : ITypeHandler, IEnumBaseTypeHandler
    {
        private readonly Predicate<TypeHandlingContext> _predicate;
        private readonly String _typeName;

        public IntegerHandler(String typeName, Predicate<TypeHandlingContext> predicate)
        {
            _typeName = typeName;
            _predicate = predicate;
        }

        public String TypeName
        {
            get { return _typeName; }
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

        #region IEnumBaseTypeHandler members

        public void HandleEnumRead(ITypeOwner element, StringBuilder builder, IList<object> args)
        {
            int fieldCount = args.Count;
            args.Add(element);
            int typeCount = args.Count;
            args.Add(element.Type);

            builder.AppendFormat("${0} = (${1})BitConverter.To{2}(bytes, index);", fieldCount, typeCount, _typeName);
            builder.AppendFormat("index += sizeof({0});", _typeName);
        }

        public void HandleEnumWrite(ITypeOwner element, StringBuilder builder, IList<object> args)
        {
            builder.AppendFormat("tmp = BitConverter.GetBytes(({1})${0});", args.Count, _typeName);
            args.Add(element);
            builder.Append("Array.Copy(tmp, 0, bytes, index, tmp.Length);");
            builder.Append("index += tmp.Length;");
        }

        #endregion
    }
}