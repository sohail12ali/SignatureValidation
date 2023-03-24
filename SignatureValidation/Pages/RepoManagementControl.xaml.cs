using SignatureValidation.ViewModels;

using System.Windows.Controls;

namespace SignatureValidation.Pages
{
    /// <summary>
    /// Interaction logic for RepoManagmentControl.xaml
    /// </summary>
    public partial class RepoManagementControl : UserControl
    {
        public RepoManagementControl(RepoManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}