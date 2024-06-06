using doc_mapper.nuget.BLL.Contracts;
using doc_mapper.nuget.DAL.Context;
using doc_mapper.nuget.DAL.Models;
using doc_mapper.nuget.Properties;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace doc_mapper.nuget.BLL.Services;

/// <summary>
/// Сервис, отвечающий за операции, связанные с пользователями.
/// </summary>
/// <remarks>
/// Инициализирует новый экземпляр класса <see cref="UserService"/>.
/// </remarks>
/// <param name="userRepository">Репозиторий для доступа к информации о пользователях.</param>
public class UserService(DocumentMapperContext context, ILogger<UserService> logger) : IUserService
{
    /// <inheritdoc />
    public async Task<User> GetCurrentUserAsync(string userAccount)
    {
        try
        {
            logger.LogInformation($"{string.Format(Resources.LogUserGetByAccount, userAccount)}");

            User user = await context.Users
                .Include(u => u.Section)
                .FirstOrDefaultAsync(u => u.Account == userAccount) ??
                throw new Exception( string.Format(Resources.ErrorUserWithAccountNotFound, userAccount));

            logger.LogInformation($"{string.Format(Resources.LogUserGetByAccount, userAccount)} {Resources.Completed}");

            return user;
        }
        catch (Exception ex)
        {
            throw CustomException(ex, Resources.LogUserGetByAccount, Resources.ErrorUserGetByAccount);
        }
    }

    private Exception CustomException(Exception ex, string logResource, string serviceResource)
    {
        logger.LogError($"{Resources.LogError} {logResource}: {JsonConvert.SerializeObject(ex)}");

        return new Exception(serviceResource);
    }
}
