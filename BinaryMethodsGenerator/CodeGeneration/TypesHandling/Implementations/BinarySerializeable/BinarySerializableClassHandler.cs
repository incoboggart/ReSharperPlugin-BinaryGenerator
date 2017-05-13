using System;
using System.Linq;
using JetBrains.ReSharper.Psi.Util;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration.Implementations;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.BinarySerializeable
{
    internal sealed class BinarySerializableClassHandler : ITypeHandler
    {
        #region ITypeHandler members

        public bool BinarySizePersistent
        {
            get { return false; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            var classType = context.Type.GetClassType();
            if (classType != null)
            {
                if (classType.Constructors.Any(c => c.Parameters.Count == 0))
                {
                    var generators = MethodGenerators.All;
                    var found = new Boolean[generators.Length];

                    foreach (var method in classType.Methods)
                    {
                        for (Int32 i = 0, c = generators.Length; i < c; i++)
                        {
                            if (!found[i])
                            {
                                found[i] = generators[i].IsGenerationTarget(method);
                            }
                        }
                    }

                    if (found.All(any => any))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void HandleRead(TypeHandlingContext context)
        {
            var haveValue = context.Variables.Declare("haveValue");

            context.Builder.Append("{");
            TypeHandlers.Boolean.HandleRead(new TypeHandlingContext(context)
            {
                TypeOwnerName = String.Format("var {0}", haveValue)
            });
            context.Builder.AppendFormat("if({0})", haveValue);
            context.Builder.Append("{");
            context.Builder.AppendFormat("{0} = new {1}();", context.TypeOwnerName, context.TypeName);
            context.Builder.AppendFormat("index += {0}.{1}(bytes, index);", context.TypeOwnerName, ReadMethodGenerator.Instance.MethodName);
            context.Builder.Append("}}");

            context.Variables.Dispose("haveValue");
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            var haveValue = context.Variables.Declare("haveValue");

            context.Builder.Append("{");
            context.Builder.AppendFormat("var {0} = {1} != null;", haveValue, context.TypeOwnerName);
            TypeHandlers.Boolean.HandleWrite(new TypeHandlingContext(context)
            {
                TypeOwnerName = haveValue
            });
            context.Builder.AppendFormat("if({0} != null)", context.TypeOwnerName);
            context.Builder.Append("{");
            context.Builder.AppendFormat("index += {0}.{1}(bytes, index);", context.TypeOwnerName, WriteMethodGenerator.Instance.MethodName);
            context.Builder.Append("}}");

            context.Variables.Dispose("haveValue");
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            TypeHandlers.Boolean.HandleGetSize(context);
            context.Builder.AppendFormat("if({0} != null)", context.TypeOwnerName);
            context.Builder.Append("{");
            context.Builder.AppendFormat("{2} += {0}.{1}();", context.TypeOwnerName, GetBinarySizeMethodGenerator.MethodName, context.GetSizeVariableName());
            context.Builder.Append("}");
        }

        #endregion
    }
}