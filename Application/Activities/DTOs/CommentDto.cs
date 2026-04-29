using System;

namespace Application.Activities.DTOs;

public class CommentDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Body { get; set; }
    public DateTime CreateDate {get;set;}
    // nav properties
    public required string UserId {get;set;}
    public required string DisplayName {get;set;}
    public string? ImageUrl {get;set;}
}
