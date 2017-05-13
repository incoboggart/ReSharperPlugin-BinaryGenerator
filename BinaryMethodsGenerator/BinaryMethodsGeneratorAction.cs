using System;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.Feature.Services.Generate.Actions;
using JetBrains.UI.RichText;

namespace ReSharperPlugins.BinaryMethodsGenerator
{
    [ActionHandler("ReSharper.BinaryMethods.Generator")]
    public class BinaryMethodsGeneratorAction : GenerateActionBase<BinaryMethodsGeneratorItemProvider>
    {
        protected override Boolean ShowMenuWithOneItem
        {
            get { return true; }
        }

        protected override RichText Caption
        {
            get { return "Binary serialization"; }
        }
    }
}