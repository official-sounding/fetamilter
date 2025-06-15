using System.ComponentModel.DataAnnotations;

namespace App.Models;

public class CreateUserModel()
{
    [Required]
    [Length(1, 100, ErrorMessage = "Length must be between 1 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\-_ ]+$", ErrorMessage = "Only alphanumeric characters, -, _ and ' ' are allowed")]
    public string? Username { get; set; }

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string? PasswordConfirm { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
}