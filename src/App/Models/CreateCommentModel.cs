using System.ComponentModel.DataAnnotations;

namespace App.Models;

public class CreateCommentModel
{
    [Required]
    public string? Body { get; set; }
}