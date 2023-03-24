using CsvHelper;
using CsvHelper.Configuration;

using SignatureValidation.Mappers;

using System.Globalization;
using System.IO;

namespace SignatureValidation.Helpers;

/// <summary>
/// This class contains Helper methods to read, write etc methods
/// </summary>
public class FileHelper
{
    /// <summary>
    /// Number to append in saved file in case same name file found
    /// </summary>
    private static string numberPattern = " ({0})";

    #region Tasks & Methods
    /// <summary>
    /// Load CSV File from provided file location 
    /// </summary>
    /// <typeparam name="T">Model which is based on the csv file</typeparam>
    /// <param name="fileName">relative or absolute file path</param>
    /// <returns>Data list of the csv file</returns>
    public Task<List<T>> LoadFullCsv<T>(string fileName)
    {
        Guard.IsNotNullOrEmpty(fileName);
        string fullPath = Path.IsPathFullyQualified(fileName) ? fileName : Path.GetFullPath(fileName);
        Guard.IsTrue(File.Exists(fullPath));
        List<T> result = new List<T>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (var reader = new StreamReader(fullPath))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<T>();
            result.AddRange(records);
        }
        return Task.FromResult(result);
    }

    /// <summary>
    /// Load CSV File from provided file location 
    /// </summary>
    /// <typeparam name="T">Model which is based on the csv file</typeparam>
    /// <param name="fileName">relative or absolute file path</param>
    /// <returns>IAsyncEnumerable of the csv file</returns>
    public IAsyncEnumerable<T> LoadFullCsvAsync<T>(string fileName)
    {
        Guard.IsNotNullOrEmpty(fileName);
        string fullPath = Path.IsPathFullyQualified(fileName) ? fileName : Path.GetFullPath(fileName);
        Guard.IsTrue(File.Exists(fullPath));
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        var reader = new StreamReader(fullPath);
        var csv = new CsvReader(reader, config);
        var records = csv.GetRecordsAsync<T>();
        return records;
    }
    /// <summary>
    /// Load CSV File from provided file absolute path  
    /// </summary>
    /// <typeparam name="T">Model which is based on the csv file</typeparam>
    /// <param name="fullPath">absolute file path</param>
    /// <returns>IAsyncEnumerable of the csv file</returns>
    private static IAsyncEnumerable<T> GetRecords<T>(string fullPath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        using (var reader = new StreamReader(fullPath))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecordsAsync<T>();
            return records;
        }
    }
    /// <summary>
    /// Save CSV File to the provided filepath
    /// </summary>
    /// <typeparam name="T">Model to save csv file</typeparam>
    /// <param name="fileName">absolute file path</param>
    /// <param name="data">Data list to save</param>
    /// <returns>saved file name</returns>
    public async Task<string> SaveCsvFile<T>(string fileName, IEnumerable<T> data)
    {
        Guard.IsNotNullOrEmpty(fileName);
        string fullPath = Path.IsPathFullyQualified(fileName) ? fileName : Path.GetFullPath(fileName);
        fullPath = NextAvailableFilename(fullPath);
        Guard.IsFalse(File.Exists(fullPath));
        using (var writer = new StreamWriter(fullPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<HashResultModelMapper>();
            await csv.WriteRecordsAsync(data);
        }
        return fullPath;
    }

    /// <summary>
    /// Check if file name already exist and provide new filename if required 
    /// </summary>
    /// <param name="path">Absolute filepath</param>
    /// <returns>filename string</returns>
    private string NextAvailableFilename(string path)
    {
        // Short-cut if already available
        if (!File.Exists(path))
            return path;

        // If path has extension then insert the number pattern just before the extension and return next filename
        if (Path.HasExtension(path))
            return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

        // Otherwise just append the pattern to the path and return next filename
        return GetNextFilename(path + numberPattern);
    }

    /// <summary>
    /// Suggest New File name with appending some pattern
    /// </summary>
    /// <param name="pattern">file name pattern</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">In case incorrect pattern</exception>
    private string GetNextFilename(string pattern)
    {
        string tmp = string.Format(pattern, 1);
        if (tmp == pattern)
            throw new ArgumentException("The pattern must include an index place-holder", "pattern");

        if (!File.Exists(tmp))
            return tmp; // short-circuit if no matches

        int min = 1, max = 2; // min is inclusive, max is exclusive/untested

        while (File.Exists(string.Format(pattern, max)))
        {
            min = max;
            max *= 2;
        }

        while (max != min + 1)
        {
            int pivot = (max + min) / 2;
            if (File.Exists(string.Format(pattern, pivot)))
                min = pivot;
            else
                max = pivot;
        }

        return string.Format(pattern, max);
    } 
    #endregion
}