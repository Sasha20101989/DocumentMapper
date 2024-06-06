using doc_mapper.nuget.DAL.Models;

namespace doc_mapper.nuget.BLL.Contracts;

/// <summary>
/// Интерфейс сервиса для операций, связанных с excel.
/// </summary>
public interface IExcelService
{
    /// <summary>
    /// Читает файл эксель
    /// </summary>
    /// <param name="filePath">Путь к документу</param>
    /// <param name="sheetName">Лист документа</param>
    /// <param name="exportFormatDate">Формат даты необходимый в экспортном файле</param>
    /// <returns></returns>
    string[,] ReadExcelFile(string filePath, string sheetName, string exportFormatDate);

    /// <summary>
    /// Красит ячейки в документе
    /// </summary>
    /// <param name="validationErrors">Список ошибок</param>
    /// <param name="sheetName">Имя листа</param>
    /// <param name="ngFilePath">Путь к файлу с ошибками</param>
    /// <param name="destinationFilePath">Путь к оригинальному файлу</param>
    void CreateNgFileAndFillCells(Dictionary<string, CellInfo> validationErrors, string sheetName, string? ngFilePath, string? destinationFilePath);
}
