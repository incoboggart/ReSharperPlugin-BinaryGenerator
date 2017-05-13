using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Feature.Services.Generate.Actions;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.Icons;
using DataConstants = JetBrains.ProjectModel.DataContext.DataConstants;

namespace ReSharperPlugins.BinaryMethodsGenerator
{
    public class BinaryMethodsGeneratorActionWorkflow : StandardGenerateActionWorkflow
    {
        public BinaryMethodsGeneratorActionWorkflow(IconId icon) :
            base("BinaryMethods", icon, "Binary serialization", GenerateActionGroup.CLR_LANGUAGE,
                "Binary serialization", "Select fields or properties, that will affect binary image.",
                "ReSharper.BinaryMethods.Generator.BinaryMethodsGeneratorAction")
        {
        }

        public override double Order
        {
            get { return 100; }
        }

        /// <summary>
        ///     This method is redefined in order to get rid of the IsKindAllowed() check at the end.
        /// </summary>
        public override bool IsAvailable(IDataContext dataContext)
        {
            ISolution solution = dataContext.GetData(DataConstants.SOLUTION);
            if (solution == null)
                return false;

            GeneratorManager generatorManager = GeneratorManager.GetInstance(solution);
            if (generatorManager == null)
                return false;

            PsiLanguageType languageType = generatorManager.GetPsiLanguageFromContext(dataContext);
            if (languageType == null)
                return false;

            var generatorContextFactory = LanguageManager.Instance.TryGetService<IGeneratorContextFactory>(languageType);
            return generatorContextFactory != null;
        }
    }
}