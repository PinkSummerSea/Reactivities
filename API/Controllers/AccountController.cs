using System;
using API.Dtos;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks;

namespace API.Controllers;

public class AccountController(SignInManager<User> signInManager):BaseApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
    {
        var user = new User
        {
            DisplayName=registerDto.DisplayName,
            Email=registerDto.Email,
            UserName=registerDto.Email
        };

        var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);
        if (result.Succeeded)return Ok();

        foreach(var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }

        return ValidationProblem();
    }

    [AllowAnonymous]
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if(User.Identity?.IsAuthenticated == false) return NoContent();
        var user = await signInManager.UserManager.GetUserAsync(User);
        if(user==null) return Unauthorized();
        return Ok(new
        {
            user.DisplayName,
            user.Email,
            user.Id,
            user.ImageUrl
        });
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }
}
