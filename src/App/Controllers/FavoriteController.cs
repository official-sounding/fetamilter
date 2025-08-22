using Microsoft.AspNetCore.Mvc;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using App.Authorization;
using App.Services;

namespace App.Controllers;

public class FavoriteController(ISiteService siteService, IPostService postService, IFavoriteService favoriteService) : ControllerBase(siteService, postService)
{

    [HttpGet("{postNum:int}/favorites")]
    public async Task<IActionResult> Details(int postNum) => await WithPost(postNum, async (post) => View(await favoriteService.GetFavorites(FavoriteType.Post, post.ID)));

    [HttpGet("{postNum:int}/{commentId:int}/favorites")]
    public async Task<IActionResult> Details(int postNum, int commentId) => await WithPost(postNum, async (post) => View(await favoriteService.GetFavorites(FavoriteType.Comment, commentId)));

    [HttpPost("{postNum:int}/favorite")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> AddFavorite(int postNum) => await WithPost(postNum, async (post) => Json(await favoriteService.AddFavorite(FavoriteType.Post, post.ID, User.GetUserId())));

    [HttpDelete("{postNum:int}/favorite")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> DeleteFavorite(int postNum) => await WithPost(postNum, async (post) => Json(await favoriteService.RemoveFavorite(FavoriteType.Post, post.ID, User.GetUserId())));

    [HttpPost("{postNum:int}/{commentId:int}/favorite")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> AddFavorite(int postNum, int commentId) => await WithPost(postNum, async (post) => Json(await favoriteService.AddFavorite(FavoriteType.Comment, commentId, User.GetUserId())));

    [HttpDelete("{postNum:int}/{commentId:int}/favorite")]
    [Authorize(Policy = Policy.MakePost)]
    public async Task<IActionResult> DeleteFavorite(int postNum, int commentId) => await WithPost(postNum, async (post) => Json(await favoriteService.RemoveFavorite(FavoriteType.Comment, commentId, User.GetUserId())));
}