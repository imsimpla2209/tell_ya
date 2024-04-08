using DiaryManagement.Application.Dtos;
using DiaryManagement.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using DiaryManagement.Infrastructure.Repositories;

namespace DiaryManagement.Application.Services
{
    public interface IUserService
    {
        public Task<Result> CreateAsync(SignUpDto form, string roleName);
        public Task<Result> UpdateUserAsync(UpdateUserDto form);
        public Task<Result> LoginAsync(LoginDto form);
        public Task<User> GetCurrentUserAsync();
        public Task<Result> LogoutAsync();
        public Task<IEnumerable<UserDto>> GetAllUsersAsync();
        public Task<string> GetRoleAsync(User user);
        public Task<bool> ChangeUserStatus(string userId);
    }

    public class UserService : IUserService
    {
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly IUserRepository _userRepository;
        public readonly UserManager<User> _userManager;
        public readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;

        public UserService(RoleManager<IdentityRole> roleManager, IUserRepository userRepository, UserManager<User> userManager,
            SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
        {
            _roleManager = roleManager;
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Result> CreateAsync(SignUpDto form, string roleName)
        {

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist) return new Result(false, " the role = " + roleName + " was not found");
            var mailChecker = await _userManager.Users.AnyAsync(u => u.Email.Equals(form.Email));
            if (mailChecker) return new Result(false, "The email already used");
            User newUser = new User()
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                UserName = form.Email,
                Email = form.Email,
                IsActivated = true
            };
            var result = await _userRepository.CreateWithRoleAsync(newUser, form.Password, roleName);
            if (result == null) return new Result(false, "Failed to create new account");
            return new Result("Create new account successfully");

        }

        public async Task<Result> UpdateUserAsync(UpdateUserDto form)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null) return new Result(false, "UnWriterzed");
            currentUser.FirstName = form.FirstName;
            currentUser.LastName = form.LastName;
            currentUser.PhoneNumber = form.PhoneNumber != null ? form.PhoneNumber : currentUser.PhoneNumber;
            currentUser.Gender = form.Gender;
            currentUser.Dob = form.Dob != null ? form.Dob : currentUser.Dob;
            currentUser.Address = form.Address != null ? form.Address : currentUser.Address;
            var result = await _userRepository.UpdateUserAsync(currentUser);
            if (result == null) return new Result(false, "Failed to update your information");
            return new Result("Your information updated!");
        }
        public async Task<Result> LoginAsync(LoginDto form)
        {
            User currentUser = await _userRepository.CheckUserLoginAsync(form.Email, form.Password);
            if (currentUser == null) return new Result(false, "Invalid email or password");
            await AddUserClaims(currentUser);
            var result = await _signInManager.PasswordSignInAsync(currentUser, form.Password, form.RememberMe, true);
            if (!result.Succeeded) return new Result(false, "Failed to login");
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            return new Result(userRoles.FirstOrDefault());
        }

        public async Task<Result> LogoutAsync()
        {

            await _signInManager.SignOutAsync();
            return new Result("Success");
        }

        public async Task<User> GetCurrentUserAsync()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                return currentUser;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            if (!users.Any()) return new List<UserDto>();
            var data = users.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Gender = u.Gender,
                Dob = u.Dob,
                Avatar = u.Avatar,
                Role = GetRoleAsync(u).Result,
                Address = u.Address,
                IsActivated = u.IsActivated,
            }).ToList();
            return data;

        }
        public async Task<string> GetRoleAsync(User user)
        {
            var roleList = await _userManager.GetRolesAsync(user);
            if (!roleList.Any()) return string.Empty;
            string role = roleList[0];
            return role;
        }
        public async Task<bool> ChangeUserStatus(string userId)
        {
            var result = await _userRepository.ChangeUserStatus(userId);
            return result;
        }



        private async Task AddUserClaims(User user)
        {
            var claims = new List<Claim>
             {
                new Claim(ClaimTypes.Name, user.ToString()),
                new Claim(ClaimTypes.Email, user.Email),

             };
            await _userManager.AddClaimsAsync(user, claims);
        }
    }
}
