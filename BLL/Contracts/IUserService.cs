using doc_mapper.nuget.DAL.Models;

namespace doc_mapper.nuget.BLL.Contracts;

/// <summary>
/// Интерфейс сервиса для операций, связанных с пользователями.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Асинхронно получает информацию о пользователе.
    /// </summary>
    /// <param name="userAccount">Учетная запись пользователя для получения информации.</param>
    /// <returns>Задача, представляющая асинхронную операцию, возвращающую информацию о пользователе.</returns>
    Task<User> GetCurrentUserAsync(string userAccount);
}
