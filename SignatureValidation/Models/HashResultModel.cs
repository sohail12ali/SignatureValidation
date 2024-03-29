﻿using CommunityToolkit.Mvvm.ComponentModel;

using CsvHelper.Configuration.Attributes;

namespace SignatureValidation.Models
{
    public partial class HashResultModel : ObservableObject
    {
        [Name("Location")]
        public string? Location { get; set; }

        [Name("FileFolderName")]
        public string? FileFolderName { get; set; }

        [Name("SignatureType")]
        public string? SignatureType { get; set; }

        [Name("Signature")]
        public string? Signature { get; set; }

        [Ignore]
        private string? validation;

        [Ignore]
        public string? Validation
        {
            get => validation;
            set => SetProperty(ref validation, value);
        }
    }
}