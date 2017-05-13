using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling
{
    internal interface IEnumBaseTypeHandler : ITypeHandler
    {
        void HandleEnumRead(ITypeOwner element, StringBuilder builder,
            IList<object> args);

        void HandleEnumWrite(ITypeOwner element, StringBuilder builder,
            IList<object> args);
    }
}