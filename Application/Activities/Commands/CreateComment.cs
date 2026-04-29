using System;
using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class CreateComment
{
    public class Command : IRequest<Result<CommentDto>>
    {
        public required string ActivityId {get;set;}
        public required string CommentBody {get;set;}
    }

    public class Handler(AppDbContext context, IUserAccessor userAccessor, IMapper mapper) : IRequestHandler<Command, Result<CommentDto>>
    {
        public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserAsync();
            if(user == null) return Result<CommentDto>.Failure("no user found", 404);
            var activity = await context.Activities.FindAsync([request.ActivityId], cancellationToken);
            if(activity == null) return Result<CommentDto>.Failure("no activity found", 404);
            var comment = new Comment
            {
                UserId = user.Id,
                ActivityId = request.ActivityId,
                Body = request.CommentBody
            };
            context.Comments.Add(comment);
            var result = await context.SaveChangesAsync(cancellationToken) > 0;
            var commentDto = mapper.Map<CommentDto>(comment);
            return result ? Result<CommentDto>.Success(commentDto) : Result<CommentDto>.Failure("problem creating the comment", 400);
        }
    }
}
