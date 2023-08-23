using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SignatureValidation.Constants;
using SignatureValidation.Pages;

using System.Windows.Controls;

namespace SignatureValidation.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    #region Fields & Properties

    /// <summary>
    /// Content View of the Main Window
    /// </summary>
    [ObservableProperty]
    private UserControl? control;

    private readonly RepoManagementControl? repoManagementControl;
    private readonly ResultManagementControl? resultManagementControl;

    public MainWindowViewModel(RepoManagementControl repoManagementControl, ResultManagementControl resultManagementControl)
    {
        Title = AppResources.MainWindowsTitle;
        this.resultManagementControl = resultManagementControl;
        this.repoManagementControl = repoManagementControl;
        Control = this.repoManagementControl;
    }

    #endregion Fields & Properties

    #region Tasks & Methods

    /// <summary>
    /// Switch to Repo Page
    /// </summary>
    [RelayCommand]
    private void SwitchToRepoPage()
    {
        Control = this.repoManagementControl;
    }

    /// <summary>
    /// Switch to Resutl Page
    /// </summary>
    [RelayCommand]
    private void SwitchToResultPage()
    {
        Control = this.resultManagementControl;
    }

    #endregion Tasks & Methods
}