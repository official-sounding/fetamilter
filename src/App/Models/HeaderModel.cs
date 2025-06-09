using Data.Models;

namespace App.Models;

public record HeaderModel(Site Site, bool IsLoggedIn, string? Username);