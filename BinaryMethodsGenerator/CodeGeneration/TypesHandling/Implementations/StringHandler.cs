using System;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations
{
    internal sealed class StringHandler : ITypeHandler
    {
        #region ITypeHandler members
        public bool BinarySizePersistent
        {
            get { return false; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            return context.Type.IsString();
        }

        public void HandleRead(TypeHandlingContext context)
        {
            var length = context.Variables.Declare("length");
            var exists = context.Variables.Declare(VariableKeys.NotNull);

            TypeHandlers.Boolean.HandleRead(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("var {0}", exists)
            });

            context.Builder.AppendFormat("if({0})", exists);
            context.Builder.Append("{");
            TypeHandlers.Int32.HandleRead(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("var {0}", length)
            });
            context.Builder.AppendFormat("if({0} > 0)", length);
            context.Builder.Append("{");
            context.Builder.AppendFormat("{0} = System.Text.Encoding.UTF8.GetString(bytes, index, {1});", context.TypeOwnerName, length);
            context.Builder.AppendFormat("index += System.Text.Encoding.UTF8.GetByteCount({0});", context.TypeOwnerName);
            context.Builder.Append("}");
            context.Builder.Append("else{");
            context.Builder.AppendFormat("{0} = String.Empty;", context.TypeOwnerName);
            context.Builder.Append("}");
            context.Builder.Append("}");

            context.Variables.Dispose("length");
        }

        public void HandleWrite(TypeHandlingContext context)
        {
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
                TypeOwnerName = String.Format("{0}.Length", context.TypeOwnerName)
            });
            context.Builder.AppendFormat("if({0}.Length > 0)", context.TypeOwnerName);
            context.Builder.Append("{");
            context.Builder.AppendFormat("tmp = System.Text.Encoding.UTF8.GetBytes({0});", context.TypeOwnerName);
            context.Builder.Append("Array.Copy(tmp, 0, bytes, index, tmp.Length);");
            context.Builder.Append("index += tmp.Length;");
            context.Builder.Append("}");
            context.Builder.Append("}");
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            TypeHandlers.Boolean.HandleGetSize(context);
            context.Builder.AppendFormat("if({0} != null)", context.TypeOwnerName);
            context.Builder.Append("{");
            TypeHandlers.Int32.HandleGetSize(context);
            context.Builder.AppendFormat("if(!String.IsNullOrEmpty({0}))", context.TypeOwnerName);
            context.Builder.Append("{");
            context.Builder.AppendFormat("{1} += System.Text.Encoding.UTF8.GetByteCount({0});", context.TypeOwnerName, context.GetSizeVariableName());
            context.Builder.Append("}");
            context.Builder.Append("}");
        } 
        #endregion
    }
}