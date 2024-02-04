using FluentValidation;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoGatherer.api.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserRegisterDto> _validator;

        public AuthController(IUserRepository userRepository, IValidator<UserRegisterDto> validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }
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
    }
}
