using System;
using System.Linq;
using JetBrains.ReSharper.Psi.Util;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.MethodsGeneration.Implementations;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.BinarySerializeable
{
    internal sealed class BinarySerializableStructHandler : ITypeHandler
    {
        #region ITypeHandler members

        public bool BinarySizePersistent
        {
            get { return false; }
        }

        public bool CanHandle(TypeHandlingContext context)
        {
            var structType = context.Type.GetStructType();
            if (structType != null)
            {
                var generators = MethodGenerators.All;
                var found = new Boolean[generators.Length];

                foreach (var method in structType.Methods)
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

            return false;
        }

        public void HandleRead(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("index += {0}.{1}(bytes, index);", context.TypeOwnerName, ReadMethodGenerator.Instance.MethodName);
        }

        public void HandleWrite(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("index += {0}.{1}(bytes, index);", context.TypeOwnerName, WriteMethodGenerator.Instance.MethodName);
        }

        public void HandleGetSize(TypeHandlingContext context)
        {
            context.Builder.AppendFormat("{2} += {0}.{1}();", context.TypeOwnerName, GetBinarySizeMethodGenerator.MethodName, context.GetSizeVariableName());
        }

        #endregion
    }
}
