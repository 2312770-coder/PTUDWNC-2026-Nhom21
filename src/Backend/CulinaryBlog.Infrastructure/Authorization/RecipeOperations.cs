using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CulinaryBlog.Infrastructure.Authorization;

// Hằng số cho các thao tác trên Recipe - tránh viết magic string rải rác.
public static class RecipeOperations
{
    public static readonly OperationAuthorizationRequirement Read = new() { Name = "Read" };
    public static readonly OperationAuthorizationRequirement Update = new() { Name = "Update" };
    public static readonly OperationAuthorizationRequirement Delete = new() { Name = "Delete" };
}
