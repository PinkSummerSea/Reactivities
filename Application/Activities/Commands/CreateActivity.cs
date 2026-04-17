using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class CreateActivity
{
    public class Command : IRequest<Result<string>>
    {
        public required CreateActivityDto ActivityDto {get;set;}
    }

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = mapper.Map<Activity>(request.ActivityDto);
            var user = await userAccessor.GetUserAsync();
            var attendee = new ActivityAttendee {UserId=user.Id, ActivityId=activity.Id, IsHost=true};
            activity.Attendees.Add(attendee);
            context.Activities.Add(activity);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;
            if(!result) return Result<string>.Failure("Failed to create the activity", 400);
            
            await context.SaveChangesAsync(cancellationToken);
            return Result<string>.Success(activity.Id);
        }
    }
}
