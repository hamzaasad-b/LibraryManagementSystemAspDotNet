using Common.Roles;
using Data.Entities;
using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Domain.Services;

public class AuthenticationService
{
    public AuthenticationService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    private readonly UserManager<User> _userManager;

    private List<string> IdentityErrorsToList(IEnumerable<IdentityError> identityErrors)
    {
        var errors = new List<string>();
        foreach (IdentityError identityError in identityErrors)
        {
            errors.Add($"{identityError.Code}: {identityError.Description}");
        }

        return errors;
    }

    public async Task<ServiceResult<User?>> RegisterUser(string email, string? password = null, string? fullName = null)
    {
        // Check if a user with the same username or email already exists
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return ServiceResult<User?>.FailedFactory("Email already registered");
        }

        // Create a new ApplicationUser
        var newUser = new User
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };


        // Use UserManager to create the user with the provided password
        var result = password == null
            ? await _userManager.CreateAsync(newUser)
            : await _userManager.CreateAsync(newUser,
                password);

        return result.Succeeded
            ? ServiceResult<User?>.SuccessfulFactory()
            : ServiceResult<User?>.FailedFactory(IdentityErrorsToList(result.Errors));
    }

    public async Task<ServiceResult<User?>> VerifyUser(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return ServiceResult<User?>.FailedFactory("User Not Found");
        }

        var hasPassword = !string.IsNullOrEmpty(user.PasswordHash);

        if (!hasPassword)
        {
            return ServiceResult<User?>.FailedFactory("Ask Admin for password creation");
        }

        if (await _userManager.CheckPasswordAsync(user, password))
        {
            return ServiceResult<User?>.SuccessfulFactory(user);
        }

        return ServiceResult<User?>.FailedFactory("Invalid email or password");
    }

    public async Task<ServiceResult<User?>> ResetAdminPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            // return if user not exist
            return ServiceResult<User?>.SuccessfulFactory();
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        if (!isAdmin)
        {
            return ServiceResult<User?>.SuccessfulFactory();
        }
        //send email to admin

        return ServiceResult<User?>.SuccessfulFactory();
    }

    public async Task<ServiceResult<User?>> UpdatePassword(string email, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return ServiceResult<User?>.FailedFactory("Invalid user");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded
            ? ServiceResult<User?>.SuccessfulFactory()
            : ServiceResult<User?>.FailedFactory(IdentityErrorsToList(result.Errors));
    }

    public async Task<ServiceResult<User?>> DeleteUser(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return ServiceResult<User?>.FailedFactory("User Not Found");
        }

        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded
            ? ServiceResult<User?>.SuccessfulFactory()
            : ServiceResult<User?>.FailedFactory(IdentityErrorsToList(result.Errors));
    }
}