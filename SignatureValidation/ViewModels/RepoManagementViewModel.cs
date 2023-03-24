using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using SignatureValidation.Constants;
using SignatureValidation.Helpers;
using SignatureValidation.Models;
using SignatureValidation.Services;

using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace SignatureValidation.ViewModels;

public partial class RepoManagementViewModel : BaseViewModel
{
    #region Fields & Properties

    private readonly OpenFileDialog fileDialog;
    private readonly DataAccessService dataAccessService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHashListVisible))]
    private ObservableCollection<HashRepoModel> hashList = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileNameOnly))]
    [NotifyPropertyChangedFor(nameof(FileFolderPath))]
    private string filePath = string.Empty;

    public string? FileNameOnly => Path.GetFileName(FilePath);

    public string? FileFolderPath => FilePath?.Replace(FileNameOnly ?? " ", string.Empty);

    [ObservableProperty]
    private Visibility isHashListVisible = Visibility.Hidden;

    [ObservableProperty]
    private Visibility isLoaderVisible = Visibility.Hidden;

    [ObservableProperty]
    private Visibility isButtonVisible = Visibility.Hidden;

    public RepoManagementViewModel(OpenFileDialog fileDialog, DataAccessService dataAccessService)
    {
        Title = AppResources.RepoPageTitle;
        this.fileDialog = fileDialog;
        this.dataAccessService = dataAccessService;
        FilePath = Path.GetFullPath(Path.Combine(AppConstants.DataFolderName, AppConstants.HashRepoFileName));
    }

    #endregion Fields & Properties

    #region Tasks & Methods

    /// <summary>
    /// Load Repo CSV to the model list
    /// </summary>
    /// <returns></returns>

    /// <summary>
    /// Load CSV from FilePath and load it to model list
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task LoadList()
    {
        try
        {
            SetButtonVisibility();
            IsLoaderVisible = Visibility.Visible;
            if (ValidateFilePath())
            {
                HashList.Clear();
                var list = await dataAccessService.GetRepo(FilePath);
                IsHashListVisible = Visibility.Visible;
                foreach (var item in list)
                {
                    HashList.Add(item);
                }
            }
            else
            {
                ShowAlert("File not found. Please provide correct file path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            SetButtonVisibility();
            IsLoaderVisible = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Browser and file from system
    /// </summary>
    /// <returns>void</returns>
    [RelayCommand]
    private Task PickFile()
    {
        try
        {
            IsLoaderVisible = Visibility.Visible;
            // Configure open file dialog box
            var dialog = fileDialog;
            dialog.FileName = AppConstants.HashRepoFileName; // Default file name
            dialog.DefaultExt = Path.GetExtension(AppConstants.HashRepoFileName); // Default file extension
            dialog.Filter = AppConstants.HashResultFilter; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                Guard.IsNotNullOrWhiteSpace(dialog.FileName);
                // Open document
                FilePath = dialog.FileName;
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            IsLoaderVisible = Visibility.Hidden;
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Clear list data
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private Task ClearList()
    {
        HashList?.Clear();
        IsHashListVisible = Visibility.Hidden;
        SetButtonVisibility();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Validation Provided path is Valid
    /// </summary>
    /// <returns></returns>
    private bool ValidateFilePath()
    {
        return File.Exists(FilePath);
    }

    /// <summary>
    /// Set Clear Button Visibility
    /// </summary>
    private void SetButtonVisibility()
    {
        IsButtonVisible = HashList is not null && HashList.Any() ? Visibility.Visible : Visibility.Hidden;
    }

    #endregion Tasks & Methods
}