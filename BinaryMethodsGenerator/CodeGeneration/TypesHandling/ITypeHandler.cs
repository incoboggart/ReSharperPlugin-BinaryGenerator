namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling
{
    internal interface ITypeHandler
    {
        bool BinarySizePersistent { get; }
        bool CanHandle(TypeHandlingContext context);
        void HandleRead(TypeHandlingContext context);
        void HandleWrite(TypeHandlingContext context);
        void HandleGetSize(TypeHandlingContext context);
    }
}