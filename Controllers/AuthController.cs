using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TaskCircle.AuthentcationApi.DTOs;
using TaskCircle.AuthentcationApi.Infrastructure.Services.Interfaces;
using TaskCircle.AuthentcationApi.Models;

namespace TaskCircle.AuthentcationApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IUserService? _userService;
    private readonly IConfiguration? _configuration;

    public AuthController(IUserService? userService, IConfiguration? configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register([FromBody] UserDTO userDto)
    {
        if (userDto is null) return BadRequest("Invalid Data");

        CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        userDto.PasswordHash = passwordHash;
        userDto.PasswordSalt = passwordSalt;

        await _userService.Register(userDto);

        userDto.Password = null;

        return Ok(userDto);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login([FromBody] UserDTO userDto)
    {
        if (userDto is null) return BadRequest("Invalid Data");

        var user = await _userService.GetUserByEmail(userDto?.Email);

        if (user == null) return NotFound("User not found!");

        if (userDto.Password == null || !VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
            return BadRequest("Wrong Email or Password!");

        // Create Token
        string token = CreateToken(user);

        // Create Refresh token
        var refreshToken = GenerateRefreshToken();
        var newRefreshToken = GetRefreshToken(refreshToken);

        userDto.IdUser = user.IdUser;
        userDto.RefreshToken = newRefreshToken.Token;
        userDto.TokenCreated = newRefreshToken.Created;
        userDto.TokenExpires = newRefreshToken.Expires;

        await _userService.RefreshToken(userDto);

        return Ok(token);
    }

    [HttpPost("logout"), Authorize]
    public async Task<ActionResult> Logout()
    {
        // Remove o refresh token do usuário
        var loggedUser = await WhoAmI();

        await _userService.RemoveRefreshToken(loggedUser);

        // Remove o cookie de refresh token
        Response.Cookies.Delete("refreshToken");

        return Ok("Logout successful");
    }

    [HttpPost("refresh-token"), Authorize]
    public async Task<ActionResult<string>> refreshToken()
    {
        var refreshTokenCookie = Request.Cookies["refreshToken"];

        //Receber usuario logado
        var loggedUser = await WhoAmI();

        //Receber usuario com informações sobre o refresh token
        var user = await _userService.GetUserById(loggedUser.IdUser);

        if (!user.RefreshToken.Equals(refreshTokenCookie))
        {
            return Unauthorized("Invalid Refresh token.");
        }
        else if(user.TokenExpires < DateTime.Now)
        {
            return Unauthorized("Token expired.");
        }

       // Create Token
        string token = CreateToken(user);
        var refreshToken = GenerateRefreshToken();
        var newRefreshToken = GetRefreshToken(refreshToken);

        user.RefreshToken = newRefreshToken.Token;
        user.TokenCreated = newRefreshToken.Created;
        user.TokenExpires = newRefreshToken.Expires;

        await _userService.RefreshToken(user);

        return Ok(token);
    }

    [HttpGet("WhoAmI"), Authorize]
    public async Task<WhoAmIDTO> WhoAmI()
    {
        //Obter token de acesso atual do usuario
        var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Split(' ')[1];

        // Decodificar o token de acesso
        var handler = new JwtSecurityTokenHandler();
        var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;

        // Obter o ID do usuário do token de acesso
        var userId = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

        // Receber utilizador atual
        var user = await _userService.WhoAmI(int.Parse(userId));

        return user;
    }

    [HttpPut("UpdateUser"), Authorize]
    public async Task<ActionResult> Put([FromBody] UpdateUserDTO updateUserDto)
    {
        if (updateUserDto is null) return BadRequest();

        //Receber usuario logado
        var loggedUser = await WhoAmI();

        updateUserDto.IdUser = loggedUser.IdUser;

        await _userService.Update(updateUserDto);

        return Ok(updateUserDto);
    }

    [HttpPut("ChangePassword"), Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
    {
        if (changePasswordDto is null) return BadRequest();

        // Receber usuario logado
        var loggedUser = await WhoAmI();

        // Receber informações sobre a password
        var user = await _userService.GetUserById(loggedUser.IdUser);

        //Verificar password atual
        if (!VerifyPasswordHash(changePasswordDto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            return BadRequest("The atual password is wrong");

        // Verificar se password é a mesma
        if (VerifyPasswordHash(changePasswordDto.NewPassword, user.PasswordHash, user.PasswordSalt))
            return BadRequest("The new password cannot be the same as the current password.");

        CreatePasswordHash(changePasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        await _userService.ChangePassword(user);

        return Ok(user);

    }

    [HttpDelete("DeleteUser"), Authorize]
    public async Task<ActionResult> Delete()
    {
        // Receber usuario logado
        var loggedUser = await WhoAmI();

        await _userService.DeleteUser(loggedUser.IdUser);



        return Ok(loggedUser);
    }

    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };
        return refreshToken;
    }

    private RefreshToken GetRefreshToken(RefreshToken newRefreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires
        };
        Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

        return newRefreshToken;
    }

    private string CreateToken(UserDTO user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString())
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:key").Value));

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var issuer = _configuration.GetSection("Jwt:Issuer").Value;
        var audience = _configuration.GetSection("Jwt:Issuer").Value;

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: cred,
            issuer: issuer,
            audience: audience);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using(var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }  
}