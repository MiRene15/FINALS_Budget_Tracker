using Microsoft.AspNetCore.Identity;
using FINALS_Budget_Tracker.Infrastructure.Entities;

namespace FINALS_Budget_Tracker.Infrastructure
{
    public class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string? password)
        {
            var errors = new List<IdentityError>();

            if (string.IsNullOrEmpty(password))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequired",
                    Description = "Password is required."
                });
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            // Check for at least 2 uppercase letters
            var upperCaseCount = password.Count(char.IsUpper);
            if (upperCaseCount < 2)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordUppercase",
                    Description = "Password must contain at least 2 uppercase letters."
                });
            }

            // Check for at least 3 numbers
            var digitCount = password.Count(char.IsDigit);
            if (digitCount < 3)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordDigit",
                    Description = "Password must contain at least 3 numbers."
                });
            }

            // Check for at least 3 symbols
            var symbolCount = password.Count(c => !char.IsLetterOrDigit(c));
            if (symbolCount < 3)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordSymbol",
                    Description = "Password must contain at least 3 symbols."
                });
            }

            // Check minimum length
            if (password.Length < 8)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordLength",
                    Description = "Password must be at least 8 characters long."
                });
            }

            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
