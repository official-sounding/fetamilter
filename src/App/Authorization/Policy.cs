using System.Security.Claims;
using System.Security.Principal;

namespace App.Authorization;

public class Policy
{
    public const string MakePost = "make_post";
    public const string MakeComment = "make_comment";
    public const string DeletePost = "delete_post";
    public const string DeleteComment = "delete_comment";
    public const string DisableUser = "disable_user";
    public const string ViewFlags = "view_flags";
    public const string PostOfficially = "post_officially";

    public static readonly string[] AllPolicies = [
        MakePost,
        MakeComment,
        DeletePost,
        DeleteComment,
        DisableUser,
        ViewFlags,
        PostOfficially
    ];
}

public static class PolicyExtensions
{
    public static bool HasRoleClaim(this ClaimsPrincipal user, string policy)
    {
        return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == policy) != null;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {

        return int.Parse(user.Claims.First(c => c.Type == ClaimTypes.Sid).Value);
    }
}