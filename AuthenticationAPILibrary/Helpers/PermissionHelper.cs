using System.Collections.Generic;
using System.Security.Claims;

namespace AuthenticationAPILibrary
{
    public static class PermissionHelper
    {
        public static bool HasPermission(ClaimsPrincipal user, string permission)
        {
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

            var rolePermissions = new Dictionary<string, List<string>>
            {
                { "Editor", new List<string> { Permissions.EditArticle, Permissions.DeleteArticle, Permissions.DeleteComment } },
                { "Writer", new List<string> { Permissions.CreateArticle, Permissions.EditArticle } },
                { "Subscriber", new List<string> { Permissions.CommentOnArticle } }
            };

            return rolePermissions.ContainsKey(userRole) && rolePermissions[userRole].Contains(permission);
        }
    }
}

