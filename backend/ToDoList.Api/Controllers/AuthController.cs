using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Api.Data;
using ToDoList.Api.Domain.Entities;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    /// <summary>Exchanges a Google ID token for an application JWT.</summary>
    [HttpPost("google")]
    public async Task<IActionResult> Google([FromBody] GoogleAuthRequest request, CancellationToken ct)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(
            request.IdToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [config["Google:ClientId"]]
            });

        var user = await db.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject, ct);
        if (user is null)
        {
            user = new User(payload.Subject, payload.Email, payload.Name, payload.Picture);
            db.Users.Add(user);
        }
        else
        {
            user.Name = payload.Name;
            user.Picture = payload.Picture;
        }

        await db.SaveChangesAsync(ct);

        var token = GenerateJwt(user);
        return Ok(new AuthResponse(token, new AuthUserDto(user.Id, user.Email, user.Name, user.Picture)));
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(int.Parse(config["Jwt:ExpiryHours"] ?? "24"));

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
            ],
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
