using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Web;

namespace DigitalMarketplace.Infrastructure.Services;
public class UserService(
    ApplicationDbContext dbContext,
    UserManager<User> userManager,
    IEmailService emailService) : IUserService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;

    public async Task<ServiceResponse<Guid?>> ConfirmEmail(string tokenEncoded)
    {
        var serviceResponse = new ServiceResponse<Guid?>();

        var id = Guid.Parse(tokenEncoded.Split(';')[0]);
        var token = string.Join("", tokenEncoded.Split(';')[1..]);

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return serviceResponse.Failed(null, "User with given id could not be found");

        //byte[] tokenBytes = WebEncoders.Base64UrlDecode(tokenEncoded);
        //var token = Encoding.UTF8.GetString(tokenBytes);
        var validationResult = await _userManager.ConfirmEmailAsync(user, token);
        if (validationResult.Succeeded)
            return serviceResponse.Succeed(id);

        //var newToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        ////byte[] newTokenBytes = Encoding.UTF8.GetBytes(newToken);
        ////var newTokenEncoded = WebEncoders.Base64UrlEncode(newTokenBytes);

        //var emailResult = await _emailService.SendEmailConfirmationLetter(user.Email!, HttpUtility.UrlEncode($"{user.Id};{newToken}"));
        return serviceResponse.Failed(null, string.Join(", ", validationResult.Errors.Select(e => e.Description)) ?? "");
    }

    public async Task<ServiceResponse<Guid?>> DeleteUser(Guid id, CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<Guid?>();

        var user = await _dbContext.Users.FindAsync([ id ], cancellationToken: ct);
        if (user is null)
            return serviceResponse.Failed(null, "");

        _dbContext.Remove(user);
        await _dbContext.SaveChangesAsync(ct);
        return serviceResponse.Succeed(user.Id);
    }

    public async Task<ServiceResponse<GetUserFullDto>> GetUser(Guid? id = null, string? username = null, string? email = null, CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<GetUserFullDto>();

        var users = _dbContext.Users
                .Include(u => u.Products)
                .Include(u => u.Currency);

        User? user = null;

        if (id is not null && id != Guid.Empty)
            user ??= await users.FirstOrDefaultAsync(u => u.Id == id, ct);

        if (string.IsNullOrWhiteSpace(username))
            user ??= await users.FirstOrDefaultAsync(u => u.UserName!.Equals(username, StringComparison.CurrentCultureIgnoreCase), ct);

        if (string.IsNullOrWhiteSpace(email))
            user ??= await users.FirstOrDefaultAsync(u => u.Email!.Equals(email, StringComparison.CurrentCultureIgnoreCase), ct);
        
        if (user is null)
            return serviceResponse.Failed(null, "User could not be found");

        return serviceResponse.Succeed(new GetUserFullDto(user.Id,
            user.ImageUri,
            user.FirstName,
            user.LastName,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            user.Location,
            user.Balance,
            user.Currency,
            user.Gender,
            user.BirthDate,
            user.Languages,
            user.Tags,
            user.Categories,
            user.ExternalResources,
            user.Bio
            ));
    }

    public async Task<ServiceResponse<IEnumerable<GetUserDto>>> GetUsers(CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<IEnumerable<GetUserDto>>();
        var users = await _dbContext.Users.Take(10).ToListAsync(ct);
        return serviceResponse.Succeed(users.Select(User.GetUserDto));
    }

    public async Task<ServiceResponse<Guid?>> ResendConfirmationLetter(Guid id)
    {
        var response = new ServiceResponse<Guid?>();

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return response.Failed(null, "User with given id does not exist");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailResult = await _emailService.SendEmailConfirmationLetter(user.Email!, HttpUtility.UrlEncode($"{user.Id};{token}"));
        if (!emailResult.Success)
            return response.Failed(null, emailResult.Error ?? "");

        return response.Succeed(id);
    }

    public async Task<ServiceResponse<Guid?>> UpdateEmail(Guid id, string email)
    {
        var serviceResponse = new ServiceResponse<Guid?>();

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return serviceResponse.Failed(null, "User with given id does not exist");

        await _userManager.GenerateChangeEmailTokenAsync(user, email);


        return serviceResponse.Succeed(null);
    }

    public Task<ServiceResponse<Guid?>> UpdatePassword(Guid id, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<Guid?>> UpdateUser(Guid id, UpdateUserDto updateUser, CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<Guid?>();

        if (Guid.Empty == id)
            return serviceResponse.Failed(null, "");

        var user = await _dbContext.Users
            .Include(u => u.Categories.Where(t => updateUser.CategoryIds != null))
            .Include(u => u.Tags.Where(t => updateUser.Tags != null))
            .FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null)
            return serviceResponse.Failed(null, "");

        user.Update(updateUser);
        if (updateUser.Tags != null)
        {
            var userTags = _dbContext.Tags.Where(updateUser.Tags.Contains).ToList();
            if (userTags.Count != updateUser.Tags.Count)
            {
                var newUserTags = updateUser.Tags.Where(t => !userTags.Contains(t)).ToList();
                foreach ( var t in newUserTags )
                {
                    _dbContext.Tags.Add(t);
                }
            }
        }


        await _dbContext.SaveChangesAsync(ct);
        return serviceResponse.Succeed(user.Id);
    }

    public Task<ServiceResponse<Guid?>> UpdateUsername(Guid id, string username)
    {
        throw new NotImplementedException();
    }
}
