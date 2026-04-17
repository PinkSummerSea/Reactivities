using System;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

public class UpdateAttendance
{
    public class Command:IRequest<Result<Unit>>
    {
        public required string Id {get;set;}
    }

    public class Handler(AppDbContext dbContext, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await dbContext.Activities.Include(x => x.Attendees).ThenInclude(x => x.User)
                            .FirstOrDefaultAsync(x => x.Id == request.Id);
            if(activity==null) return Result<Unit>.Failure("activity not found", 404);
            var user = await userAccessor.GetUserAsync();
            var attendance = activity.Attendees.FirstOrDefault(x => x.UserId == user.Id);
            var isHost = activity.Attendees.Any(x => x.UserId == user.Id && x.IsHost);
            if(attendance != null)
            {
                if (isHost)
                {
                    //toggle cancel state of the activity
                    activity.IsCancelled=!activity.IsCancelled;
                }
                else
                {
                    //remove attendence
                    activity.Attendees.Remove(attendance);
                }
            }
            else
            {
                //add attendence
                var newAttendance = new ActivityAttendee
                {
                    UserId=user.Id,
                    ActivityId=request.Id,
                    IsHost=false
                };
                activity.Attendees.Add(newAttendance);

            }

            var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("problem updating the db", 400);

        }
    }
}
