using CsvHelper.Configuration;

using SignatureValidation.Models;

namespace SignatureValidation.Mappers;

public class HashResultModelMapper : ClassMap<HashResultModel>
{
    /// <summary>
    /// Mapping HashResultModel for CSV Helper
    /// </summary>
    public HashResultModelMapper()
    {
        Map(m => m.Location).Name("Location");
        Map(m => m.FileFolderName).Name("FileFolderName");
        Map(m => m.SignatureType).Name("SignatureType");
        Map(m => m.Signature).Name("Signature");
        Map(m => m.Validation).Name("Validation");
    }
}