using CsvHelper.Configuration.Attributes;

namespace SignatureValidation.Models
{
    public class HashRepoModel
    {
        [Name("imagename")]
        public string? ImageName { get; set; }

        [Name("sigtype1")]
        public string? SigType1 { get; set; }

        [Name("sig1")]
        public string? Sig1 { get; set; }

        [Name("sigtype2")]
        public string? SigType2 { get; set; }

        [Name("sig2")]
        public string? Sig2 { get; set; }

        [Name("sigtype3")]
        public string? SigType3 { get; set; }

        [Name("sig3")]
        public string? Sig3 { get; set; }

        [Name("sigtype4")]
        public string? SigType4 { get; set; }

        [Name("sig4")]
        public string? Sig4 { get; set; }

        [Name("sigtype5")]
        public string? SigType5 { get; set; }

        [Name("sig5")]
        public string? Sig5 { get; set; }
    }
}