using SignatureValidation.ViewModels;

using System.Windows.Controls;

namespace SignatureValidation.Pages
{
    /// <summary>
    /// Interaction logic for ResultManagementControl.xaml
    /// </summary>
    public partial class ResultManagementControl : UserControl
    {
        public ResultManagementControl(ResultManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}