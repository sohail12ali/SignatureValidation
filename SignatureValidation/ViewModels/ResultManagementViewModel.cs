using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;

using SignatureValidation.Constants;
using SignatureValidation.Enums;
using SignatureValidation.Extensions;
using SignatureValidation.Helpers;
using SignatureValidation.Models;
using SignatureValidation.Services;

using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace SignatureValidation.ViewModels;

public partial class ResultManagementViewModel : BaseViewModel
{
    #region Fields & Properties

    private readonly FileHelper fileHelper;
    private readonly OpenFileDialog fileDialog;
    private readonly DataAccessService dataAccessService;
    private readonly SaveFileDialog saveFileDialog;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHashListVisible))]
    private ObservableCollection<HashResultModel> hashList = new();

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

    public ResultManagementViewModel(FileHelper fileHelper, OpenFileDialog fileDialog, DataAccessService dataAccessService, SaveFileDialog saveFileDialog)
    {
        Title = AppResources.ResultPageTitle;
        this.fileHelper = fileHelper;
        this.fileDialog = fileDialog;
        this.dataAccessService = dataAccessService;
        this.saveFileDialog = saveFileDialog;
        FilePath = Path.GetFullPath(Path.Combine(AppConstants.DataFolderName, AppConstants.HashResultFileName));
    }

    #endregion Fields & Properties

    #region Tasks & Methods

    /// <summary>
    /// Load CSV from FilePath and load it to model list
    /// </summary>
    /// <returns>void</returns>
    [RelayCommand]
    private async Task LoadList()
    {
        try
        {
            IsLoaderVisible = Visibility.Visible;
            if (ValidateFilePath())
            {
                HashList.Clear();
                var list = await fileHelper.LoadFullCsv<HashResultModel>(FilePath);
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
            dialog.FileName = AppConstants.HashResultFileName; // Default file name
            dialog.DefaultExt = Path.GetExtension(AppConstants.HashResultFileName); // Default file extension
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
    /// Clear Grid Data
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
    /// Validate result data with repo data
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ValidateData()
    {
        try
        {
            IsLoaderVisible = Visibility.Hidden;
            var repoData = await dataAccessService.GetRepo();
            Guard.IsNotNull(repoData);
            Guard.IsTrue(repoData.Any());
            Guard.IsTrue(HashList.Any());

            foreach (HashResultModel item in HashList)
            {
                ValidateData(repoData, item);
            }

            ShowAlert(AppResources.ValidationCompleteMessage, AppResources.ValidationCompleteTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            IsLoaderVisible = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Save/Export Grid data as CSV file
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task SaveCsv()
    {
        try
        {
            Guard.IsNotNull(HashList);
            Guard.IsTrue(HashList.Any());

            saveFileDialog.FileName = AppConstants.ExportedResultFileName;
            saveFileDialog.Filter = AppConstants.HashResultFilter;
            saveFileDialog.OverwritePrompt = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == true)
            {
                string path = saveFileDialog.FileName;

                var fileName = await fileHelper.SaveCsvFile(path, HashList);
                Guard.IsTrue(File.Exists(fileName));
                ShowAlert(fileName, "File Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    #region Validate Data
    /// <summary>
    /// Validatation process of result data with repo data
    /// </summary>
    /// <param name="repoData"></param>
    /// <param name="item"></param>
    private static void ValidateData(IEnumerable<HashRepoModel> repoData, HashResultModel item)
    {
        var t1 = repoData.Where(x => x.ImageName!.Tm().Equals(item?.FileFolderName?.Tm()));
        if (t1 is not null && t1.Any())
        {
            item.Validation = ValidationResult.NO_SIGN_TYPE_MATCH.GetDesc();

            if (Enum.TryParse(item.SignatureType?.Tm()!, out SignatureType type))
            {
                switch (type)
                {
                    case SignatureType.SHA1:
                        SHA1(item, t1);
                        break;

                    case SignatureType.MD5:
                        MD5(item, t1);
                        break;

                    case SignatureType.CRC16:
                        CRC16(item, t1);
                        break;

                    case SignatureType.CRC32:
                        CRC32(item, t1);
                        break;

                    case SignatureType.HMACSHA1:
                        HMACSHA1(item, t1);
                        break;

                    default:
                        break;
                }
            }
        }
        else
        {
            item.Validation = ValidationResult.NO_FILE.GetDesc();
        }
    }

    /// <summary>
    /// Match Sign of HMACSHA1
    /// </summary>
    /// <param name="item">Result Row</param>
    /// <param name="t1">Repo Data</param>
    private static void HMACSHA1(HashResultModel item, IEnumerable<HashRepoModel>? t1)
    {
        Guard.IsNotNull(t1);
        var s5 = t1.Where(x => x.SigType5?.Tm() == item.SignatureType?.Tm());
        if (s5 is not null && s5.Any())
        {
            item.Validation = ValidationResult.NO_MATCH.GetDesc();
            if (s5.Any(x => x.Sig5?.UpTmW() == item.Signature?.UpTmW()))
            {
                item.Validation = ValidationResult.MATCH.GetDesc();
            }
        }
    }

    /// <summary>
    /// Match Sign of CRC32
    /// </summary>
    /// <param name="item">Result Row</param>
    /// <param name="t1">Repo Data</param>
    private static void CRC32(HashResultModel item, IEnumerable<HashRepoModel>? t1)
    {
        Guard.IsNotNull(t1);
        var s4 = t1.Where(x => x.SigType4?.Tm() == item.SignatureType?.Tm());
        if (s4 is not null && s4.Any())
        {
            item.Validation = ValidationResult.NO_MATCH.ToString();
            if (s4.Any(x => x.Sig4?.UpTmW() == item.Signature?.UpTmW()))
            {
                item.Validation = ValidationResult.MATCH.GetDesc();
            }
        }
    }

    /// <summary>
    /// Match Sign of CRC16
    /// </summary>
    /// <param name="item">Result Row</param>
    /// <param name="t1">Repo Data</param>
    private static void CRC16(HashResultModel item, IEnumerable<HashRepoModel>? t1)
    {
        Guard.IsNotNull(t1);
        var s3 = t1.Where(x => x.SigType3?.Tm() == item.SignatureType?.Tm());
        if (s3 is not null && s3.Any())
        {
            item.Validation = ValidationResult.NO_MATCH.ToString();
            if (s3.Any(x => x.Sig3?.UpTmW() == item.Signature?.UpTmW()))
            {
                item.Validation = ValidationResult.MATCH.GetDesc();
            }
        }
    }

    /// <summary>
    /// Match Sign of MD5
    /// </summary>
    /// <param name="item">Result Row</param>
    /// <param name="t1">Repo Data</param>
    private static void MD5(HashResultModel item, IEnumerable<HashRepoModel>? t1)
    {
        Guard.IsNotNull(t1);
        var s2 = t1.Where(x => x.SigType2?.Tm() == item.SignatureType?.Tm());
        if (s2 is not null && s2.Any())
        {
            item.Validation = ValidationResult.NO_MATCH.ToString();

            if (s2.Any(x => x.Sig2?.UpTmW() == item.Signature?.UpTmW()))
            {
                item.Validation = ValidationResult.MATCH.GetDesc();
            }
        }
    }

    /// <summary>
    /// Match Sign of SHA1
    /// </summary>
    /// <param name="item">Result Row</param>
    /// <param name="t1">Repo Data</param>
    private static void SHA1(HashResultModel item, IEnumerable<HashRepoModel>? t1)
    {
        Guard.IsNotNull(t1);
        var s1 = t1.Where(x => x.SigType1?.Tm() == item.SignatureType?.Tm());
        if (s1 is not null && s1.Any())
        {
            item.Validation = ValidationResult.NO_MATCH.ToString();
            if (s1.Any(x => x.Sig1?.UpTmW() == item.Signature?.UpTmW()))
            {
                item.Validation = ValidationResult.MATCH.GetDesc();
            }
        }
    }

    #endregion Validate Data

    /// <summary>
    /// Validation if selected file path is correct
    /// </summary>
    /// <returns>bool</returns>
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