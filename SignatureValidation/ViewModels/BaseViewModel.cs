using CommunityToolkit.Mvvm.ComponentModel;

using System.Windows;

namespace SignatureValidation.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    #region Properties & Fields

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string? title;

    public bool IsNotBusy => !IsBusy;

    #endregion Properties & Fields

    #region Tasks & Methods
    protected MessageBoxResult ShowAlert(string message, string title, MessageBoxButton button, MessageBoxImage icon)
    {
        MessageBoxResult result = MessageBox.Show(message, title, button, icon, MessageBoxResult.Yes);
        return result;
    }

    protected MessageBoxResult ShowError(Exception ex)
    {
        MessageBoxResult result = MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
        Debug.WriteLine(ex);
        return result;
    }
    #endregion
}