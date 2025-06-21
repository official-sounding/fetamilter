using System.ComponentModel.DataAnnotations;

namespace App.Models;

public class CreatePostModel
{
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Body { get; set; }

    public string? MoreInside { get; set; }
    [StringLength(200)]
    public string? TagList { get; set; }
}