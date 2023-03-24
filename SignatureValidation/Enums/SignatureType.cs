using System.ComponentModel;

namespace SignatureValidation.Enums;

/// <summary>
/// All Signature Type available in the repo files
/// </summary>
public enum SignatureType
{
    [Description("SHA1")]
    SHA1,

    [Description("MD5")]
    MD5,

    [Description("CRC16")]
    CRC16,

    [Description("CRC32")]
    CRC32,

    [Description("HMACSHA1")]
    HMACSHA1
}