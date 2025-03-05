using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AuthenticationAPILibrary.Models;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private static List<Article> Articles = new List<Article>();
        private static List<Comment> Comments = new List<Comment>();

       
        [Authorize(Policy = "EditorOrWriterPolicy")]
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
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole == "Editor" || (userRole == "Writer" && existingArticle.AuthorId == userId))
                {
                    existingArticle.Title = article.Title;
                    existingArticle.Content = article.Content;
                    return Ok();
                }
            }

            return Unauthorized();
        }

        [Authorize(Policy = "EditorPolicy")]
        [HttpDelete("delete-article/{id}")]
        public IActionResult DeleteArticle(int id)
        {
            var article = Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            Articles.Remove(article);

            return Ok();
        }

        [Authorize(Policy = "WriterPolicy")]
        [HttpPost("create-article")]
        public IActionResult CreateArticle(Article article)
        {
            //var userIdTest = User.FindAll(ClaimTypes.NameIdentifier).Last()?.Value;
            if (int.TryParse(User.FindAll(ClaimTypes.NameIdentifier).Last()?.Value, out int userId))
            {
                article.AuthorId = userId;
                article.Id = Articles.Count + 1;
                Articles.Add(article);
                return Ok();
            }

            return Unauthorized();
        }

        [Authorize(Policy = "SubscriberPolicy")]
        [HttpPost("comment/{articleId}")]
        public IActionResult CommentOnArticle(int articleId, Comment comment)
        {
            var article = Articles.FirstOrDefault(a => a.Id == articleId);
            if (article == null)
            {
                return NotFound();
            }

            comment.Id = Comments.Count + 1;
            comment.ArticleId = articleId;
            Comments.Add(comment);

            return Ok();
        }

        [Authorize(Policy = "EditorPolicy")]
        [HttpDelete("delete-comment/{id}")]
        public IActionResult DeleteComment(int id)
        {
            var comment = Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            Comments.Remove(comment);

            return Ok();
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
