using doc_mapper.nuget.DAL.Models;
using doc_mapper.nuget.Enums;

namespace doc_mapper.nuget.BLL.Contracts;

/// <summary>
/// Интерфейс сервиса для операций, связанных с информацией о процессе приложения.
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// Получает шаги для процесса приложения и по уникальному индентификатору секции пользователя
    /// </summary>
    /// <param name="appProcess">индентификатор процесса прилодения</param>
    /// /// <param name="userAccount">аккаунт пользователя</param>
    /// <returns>Задача, представляющая асинхронную операцию, возвращающая шаги процесса по уникальному секции и процессу приложения</returns>
    Task<List<ProcessesStep>> GetProcessStepsByUserSectionAsync(AppProcess appProcess, string userAccount);
}
