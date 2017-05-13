﻿using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace ReSharperPlugins.BinaryMethodsGenerator
{
    [ActionHandler("BinaryMethodsGenerator.About")]
    public class AboutAction : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // return true or false to enable/disable this action
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            MessageBox.Show(
              "Custom serialization generator\nIncoBoggart.\n\n",
              "Generates custom binary serialization methods.",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information);
        }
    }
}