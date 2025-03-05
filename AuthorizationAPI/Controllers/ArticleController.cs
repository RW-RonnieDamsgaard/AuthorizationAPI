using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthenticationAPILibrary.Models;
using AuthenticationAPILibrary;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private static List<Article> Articles = new List<Article>();
        private static List<Comment> Comments = new List<Comment>();

        [Authorize]
        [HttpPut("edit-article/{id}")]
        public IActionResult EditArticle(int id, Article article)
        {
            var existingArticle = Articles.FirstOrDefault(a => a.Id == id);
            if (existingArticle == null)
            {
                return NotFound();
            }
            if (int.TryParse(User.FindAll(ClaimTypes.NameIdentifier).Last()?.Value, out int userId))
            {
                if (PermissionHelper.HasPermission(User, Permissions.EditArticle) &&
                    (PermissionHelper.HasPermission(User, Permissions.DeleteArticle) || existingArticle.AuthorId == userId))
                {
                    existingArticle.Title = article.Title;
                    existingArticle.Content = article.Content;
                    return Ok();
                }
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpDelete("delete-article/{id}")]
        public IActionResult DeleteArticle(int id)
        {
            var article = Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            if (PermissionHelper.HasPermission(User, Permissions.DeleteArticle))
            {
                Articles.Remove(article);
                return Ok();
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpPost("create-article")]
        public IActionResult CreateArticle(Article article)
        {
            if (int.TryParse(User.FindAll(ClaimTypes.NameIdentifier).Last()?.Value, out int userId))
            {
                if (PermissionHelper.HasPermission(User, Permissions.CreateArticle))
                {
                    article.AuthorId = userId;
                    article.Id = Articles.Count + 1;
                    Articles.Add(article);
                    return Ok();
                }
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpPost("comment/{articleId}")]
        public IActionResult CommentOnArticle(int articleId, Comment comment)
        {
            var article = Articles.FirstOrDefault(a => a.Id == articleId);
            if (article == null)
            {
                return NotFound();
            }

            if (PermissionHelper.HasPermission(User, Permissions.CommentOnArticle))
            {
                comment.Id = Comments.Count + 1;
                comment.ArticleId = articleId;
                Comments.Add(comment);
                return Ok();
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpDelete("delete-comment/{id}")]
        public IActionResult DeleteComment(int id)
        {
            var comment = Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            if (PermissionHelper.HasPermission(User, Permissions.DeleteComment))
            {
                Comments.Remove(comment);
                return Ok();
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpGet("articles")]
        public IActionResult GetArticles()
        {
            return Ok(Articles);
        }

        [AllowAnonymous]
        [HttpGet("comments/{articleId}")]
        public IActionResult GetComments(int articleId)
        {
            var article = Articles.FirstOrDefault(a => a.Id == articleId);
            if (article == null)
            {
                return NotFound();
            }

            var articleComments = Comments.Where(c => c.ArticleId == articleId).ToList();
            return Ok(articleComments);
        }
    }

   

  
}
