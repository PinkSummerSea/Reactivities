using System;
using Application.Core;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetUserActivities
{
    public class Query : IRequest<Result<List<UserActivityDto>>>{
        public required string Filter { get; set; }
        public required string UserId {get;set;}
    }

    public class Handler(IMapper mapper, AppDbContext context) : IRequestHandler<Query, Result<List<UserActivityDto>>>
    {
        public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // get the user's all activities
            var query = context.Activities.Where(x => x.Attendees.Any(a => a.UserId==request.UserId)).OrderBy(x => x.Date).AsQueryable();

            query = request.Filter switch
            {
                "future" => query.Where(x => x.Date >= DateTime.UtcNow),
                "past" => query.Where(x => x.Date < DateTime.UtcNow),
                "hosting"=> query.Where(x => x.Attendees.Any(a => a.IsHost && a.UserId==request.UserId)),
                _ => query
            
            };

            return Result<List<UserActivityDto>>.Success(await query.ProjectTo<UserActivityDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken));
        }
    }
}
