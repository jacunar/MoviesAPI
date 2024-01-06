using Microsoft.AspNetCore.Authorization;

namespace MoviesAPI.Tests;
public class AllowAnonymousHandler : IAuthorizationHandler {
    public Task HandleAsync(AuthorizationHandlerContext context) {
        foreach(var requiered in context.PendingRequirements.ToList()) {
            context.Succeed(requiered);
        }

        return Task.CompletedTask;
    }
}