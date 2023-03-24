using System.ComponentModel;

namespace SignatureValidation.Enums;

/// <summary>
/// All possible Validation status
/// </summary>
public enum ValidationResult
{
    [Description("Match")]
    MATCH,

    [Description("No Sign Type Match")]
    NO_SIGN_TYPE_MATCH,

    [Description("No Match")]
    NO_MATCH,

    [Description("No File")]
    NO_FILE
}