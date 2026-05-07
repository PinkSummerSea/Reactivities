using System;
using Application.Activities.Commands;
using Application.Profiles.Commands;
using Application.Profiles.DTOs;
using Application.Profiles.Queries;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProfilesController:BaseApiController
{
    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto(IFormFile file)
    {
        return HandleResult(await Mediator.Send(new AddPhoto.Command{File=file}));
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<List<Photo>>> GetPhotos(string id)
    {
        return HandleResult(await Mediator.Send(new GetProfilePhotos.Query{UserId=id}));
    }

    [HttpDelete("{photoId}/photos")]
    public async Task<ActionResult> DeletePhoto(string photoId)
    {
        return HandleResult(await Mediator.Send(new DeletePhoto.Command{PhotoId = photoId}));
    }

    [HttpPut("{photoId}/setMain")]
    public async Task<ActionResult> SetMainPhoto(string photoId)
    {
        return HandleResult(await Mediator.Send(new SetMainPhoto.Command{PhotoId=photoId}));
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserProfile>> GetProfile(string userId)
    {
        return HandleResult(await Mediator.Send(new GetProfile.Query{UserId= userId}));
    }

    [HttpPost("{userId}/follow")]
    public async Task<ActionResult> FollowToggle(string userId)
    {
        return HandleResult(await Mediator.Send(new FollowToggle.Command {TargetId=userId}));
    } 

    [HttpGet("{userId}/follow-list")]
    //get predicate from query string
    public async Task<ActionResult<List<UserProfile>>> GetFollowings(string userId, string predicate)
    {
        return HandleResult(await Mediator.Send(new GetFollowings.Query {Predicate=predicate, UserId=userId}));
    }
}
