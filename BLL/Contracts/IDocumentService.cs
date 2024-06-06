using doc_mapper.nuget.DAL.Models;

namespace doc_mapper.nuget.BLL.Contracts;

/// <summary>
/// Интерфейс сервиса для операций, связанных с картами сопоставления данных в экселе.
/// </summary>
public interface IDocumentService
{
    /// <summary>
    /// Асинхронно создает документ.
    /// </summary>
    /// <param name="document"></param>
    /// <returns>Задача, представляющая асинхронную операцию, добавляющую документ и возвращающую документ с текущим Id.</returns>
    Task CreateDocumentAsync(DocumentMapper document);

    /// <summary>
    /// Асинхронно получает список карт документов.
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию, возвращающую список документов.</returns>
    Task<List<DocumentMapper>> GetAllDocumentsAsync();

    /// <summary>
    /// Асинхронно получает список карт документов.
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию, возвращающую список документов.</returns>
    Task<List<DocumentMapper>> GetFilteredDocumentsAsync(string docmapperName);

    /// <summary>
    /// Асинхронно получает список колонок документа.
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию, возвращающую список колонок документов.</returns>
    Task<IEnumerable<DocumentMapperColumn>> GetAllColumnsAsync();

    /// <summary>
    /// Асинхронно обновляет документ.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="documentContent"></param>
    /// <returns>Задача, представляющая асинхронную операцию, обновляющую документ.</returns>
    Task UpdateDocumentAsync(DocumentMapper document);

    /// <summary>
    /// Асинхронно добавляет элемент используемый в элементе контента документа
    /// </summary>
    /// <param name="documentColumn">Информация о элементе используемом в элементе контента документа</param>
    /// <returns>Задача, представляющая асинхронную операцию, добавляющую элемент используемый в элементе контента документа.</returns>
    Task AddDocumentColumnAsync(DocumentMapperColumn documentColumn);

    /// <summary>
    /// Удаляет из локального состояния
    /// </summary>
    /// <param name="docmapperContent"></param>
    /// <returns>Задача, представляющая асинхронную операцию, удаления из локального состояния.</returns>
    Task MarkToDeleteDocumentContentItemAsync(DocumentMapperContent docmapperContent);

    /// <summary>
    /// Добавляет в локальное состояние без сохранения контекста
    /// </summary>
    /// <param name="docmapperContent"></param>
    void MarkToAddNewDocumentContentItem(DocumentMapperContent docmapperContent);

    /// <summary>
    /// Обновляет строку контента
    /// </summary>
    /// <param name="docmapperContent"></param>
    /// <returns>Задача, представляющая асинхронную операцию, обновляющую строку контента документа.</returns>
    Task UpdateRowNumberInDocumentContentItemAsync(DocumentMapperContent docmapperContent);

    /// <summary>
    /// Обновляет колонку контента
    /// </summary>
    /// <param name="docmapperContent"></param>
    /// <returns>Задача, представляющая асинхронную операцию, обновляющую колонку контента документа.</returns>
    Task UpdateColumnNumberInDocumentContentItemAsync(DocumentMapperContent docmapperContent);
}
