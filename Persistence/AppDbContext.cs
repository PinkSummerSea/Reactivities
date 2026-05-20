using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence;

public class AppDbContext(DbContextOptions options):IdentityDbContext<User>(options)
{
    public required DbSet<Activity> Activities { get; set; }
    public required DbSet<ActivityAttendee> ActivityAttendees {get;set;}
    public required DbSet<Photo> Photos {get;set;}
    public required DbSet<Comment> Comments {get;set;}
    public required DbSet<UserFollowing> UserFollowings {get;set;}
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ActivityAttendee>()
            .HasKey(a => new {a.ActivityId, a.UserId});

        builder.Entity<ActivityAttendee>()
            .HasOne(x => x.User)
            .WithMany(x => x.Activities)
            .HasForeignKey(x => x.UserId);
        
        builder.Entity<ActivityAttendee>()
            .HasOne(x => x.Activity)
            .WithMany(x => x.Attendees)
            .HasForeignKey(x => x.ActivityId);
        builder.Entity<UserFollowing>()
            .HasKey(x => new {x.ObserverId, x.TargetId});
        builder.Entity<UserFollowing>()
            .HasOne(x => x.Observer)
            .WithMany(x => x.Followings)
            .HasForeignKey(x => x.ObserverId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<UserFollowing>()
            .HasOne(x => x.Target)
            .WithMany(x => x.Followers)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.NoAction);
        
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        foreach(var entityType in builder.Model.GetEntityTypes())
        {
            foreach(var property in entityType.GetProperties())
            {
                if(property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }
    }
}
