using JetBrains.ReSharper.Psi;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.BinarySerializeable;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Collections;
using ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling.Implementations.Primitives;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling
{
    internal static class TypeHandlers
    {
        public static readonly BooleanHandler Boolean = new BooleanHandler();
        public static readonly IntegerHandler Int32 = new IntegerHandler("Int32", ctx => ctx.Type.IsInt());

        public static readonly ITypeHandler[] PrimitiveHandlers =
        {
            new BooleanHandler(),
            new ByteHandler(),
            new IntegerHandler("Int16", ctx => ctx.Type.IsShort()),
            new IntegerHandler("UInt16", ctx => ctx.Type.IsUshort()),
            new IntegerHandler("Int32", ctx => ctx.Type.IsInt()),
            new IntegerHandler("UInt32", ctx => ctx.Type.IsUint()),
            new IntegerHandler("Int64", ctx => ctx.Type.IsLong()),
            new IntegerHandler("UInt64", ctx => ctx.Type.IsUlong()),
            new FloatingHandler("Single", ctx => ctx.Type.IsFloat()),
            new FloatingHandler("Double", ctx => ctx.Type.IsDouble())
        };

        public static readonly IEnumBaseTypeHandler[] EnumBaseTypeHandlers =
        {
            new ByteHandler(),
            new IntegerHandler("Int16", ctx => ctx.Type.IsShort()),
            new IntegerHandler("UInt16", ctx => ctx.Type.IsUshort()),
            new IntegerHandler("Int32", ctx => ctx.Type.IsInt()),
            new IntegerHandler("UInt32", ctx => ctx.Type.IsUint()),
            new IntegerHandler("Int64", ctx => ctx.Type.IsLong()),
            new IntegerHandler("UInt64", ctx => ctx.Type.IsUlong())
        };

        public static readonly ITypeHandler[] All =
        {
            new BooleanHandler(),
            new ByteHandler(),
            new IntegerHandler("Int16", ctx => ctx.Type.IsShort()),
            new IntegerHandler("UInt16", ctx => ctx.Type.IsUshort()),
            new IntegerHandler("Int32", ctx => ctx.Type.IsInt()),
            new IntegerHandler("UInt32", ctx => ctx.Type.IsUint()),
            new IntegerHandler("Int64", ctx => ctx.Type.IsLong()),
            new IntegerHandler("UInt64", ctx => ctx.Type.IsUlong()),
            new FloatingHandler("Single", ctx => ctx.Type.IsFloat()),
            new FloatingHandler("Double", ctx => ctx.Type.IsDouble()),
            new DateTimeHandler("DateTime", ctx => ctx.Type.IsDateTime()),
            new DateTimeHandler("TimeSpan", ctx => ctx.Type.IsTimeSpan()),
            new GuidHandler(),
            new EnumsHandler(),
            new StringHandler(),
            new NullableHandler(),
            new ArrayHandler(),
            new ListHandler(), 
            new BinarySerializableClassHandler(), 
            new BinarySerializableStructHandler(), 
        };
    }
}