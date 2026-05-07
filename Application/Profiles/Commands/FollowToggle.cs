using System;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class FollowToggle
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string TargetId {get;set;}
    }

    public class Handler(AppDbContext context, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var observer = await userAccessor.GetUserAsync();
            var target = await context.Users.FindAsync([request.TargetId], cancellationToken);
            if(target==null) return Result<Unit>.Failure("target user not found", 400);
            var following = await context.UserFollowings.FindAsync([observer.Id, target.Id], cancellationToken);
            if (following==null) context.UserFollowings.Add(new UserFollowing
            {
                ObserverId=observer.Id,
                TargetId=target.Id
            }); else context.UserFollowings.Remove(following);
            
            var result = await context.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("problem updating user following", 400);
        }
    }
}
