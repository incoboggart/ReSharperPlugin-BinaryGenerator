using System.Collections.Generic;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Generate.Actions;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.Icons;
using DataConstants = JetBrains.ProjectModel.DataContext.DataConstants;

namespace ReSharperPlugins.BinaryMethodsGenerator
{
    [GenerateProvider]
    public class BinaryMethodsGeneratorItemProvider : IGenerateActionProvider
    {
        public IEnumerable<IGenerateActionWorkflow> CreateWorkflow(IDataContext dataContext)
        {
            ISolution solution = dataContext.GetData(DataConstants.SOLUTION);
            var iconManager = solution.GetComponent<PsiIconManager>();
            IconId icon = iconManager.GetImage(CLRDeclaredElementType.METHOD);
            yield return new BinaryMethodsGeneratorActionWorkflow(icon);
        }
    }
}