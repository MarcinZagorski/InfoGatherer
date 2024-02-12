using FluentValidation;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.DTOs.Users;
using InfoGatherer.api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InfoGatherer.api.Controllers
{
    public class AuthController(IUserRepository userRepository, IValidator<UserRegisterDto> validator, ITokenService tokenService, IValidator<ChangePasswordDto> validatorChangePassword) : BaseController
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IValidator<UserRegisterDto> _validator = validator;
        private readonly IValidator<ChangePasswordDto> _validatorChangePassword = validatorChangePassword;
        private readonly ITokenService _tokenService = tokenService;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var validationResult = await _validator.ValidateAsync(userRegisterDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var userExists = await _userRepository.UserExistsAsync(userRegisterDto.Email);
            if (userExists)
            {
                return BadRequest(new { message = "User with given email already exists." });
            }
            var result = await _userRepository.RegisterUserAsync(userRegisterDto);
            if (result)
            {
                return Ok(new { message = "Registration successfully completed." });
            }
            else
            {
                return BadRequest(new { message = "User registration failed." });
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.LoginAsync(loginDto.Email, loginDto.Password);
            if (user != null)
            {
                var token = await _tokenService.GenerateJwtToken(user);
                return Ok(new { token, message = "Login successful" });
            }

            _logger.Warn($"Login failed for user {loginDto.Email}.");
            return Unauthorized(new { message = "Login failed. Invalid email or password." });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDto refreshRequest)
        {
            if (string.IsNullOrEmpty(refreshRequest.Token) || string.IsNullOrEmpty(refreshRequest.RefreshToken))
            {
                return BadRequest(new { message = "Invalid request, missing tokens." });
            }

            var authResponse = await _tokenService.RefreshTokenAsync(refreshRequest.Token, refreshRequest.RefreshToken);

            if (authResponse == null)
            {
                return BadRequest(new { message = "Invalid tokens or tokens expired." });
            }

            return Ok(authResponse);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var validationResult = await _validatorChangePassword.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var success = await _userRepository.ChangePasswordAsync(model.OldPassword, model.NewPassword);

            if (!success)
            {
                return BadRequest("Failed to change password.");
            }

            return Ok("Password changed successfully.");
        }
    }
}
