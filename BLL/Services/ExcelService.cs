using doc_mapper.nuget.BLL.Contracts;
using ClosedXML.Excel;
using doc_mapper.nuget.DAL.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using doc_mapper.nuget.Properties;

namespace doc_mapper.nuget.BLL.Services;

/// <summary>
/// Сервис для работы с файлами Excel через DocumentMapper.
/// </summary>
/// <remarks>
/// Инициализирует новый экземпляр класса <see cref="ExcelService"/>.
/// </remarks>
/// <param name="logger">Интерфейс для записи логов.</param>
public partial class ExcelService(ILogger<ExcelService> logger) : IExcelService
{
    /// <inheritdoc />
    public void CreateNgFileAndFillCells(Dictionary<string, CellInfo> validationErrors, string sheetName, string? ngFilePath, string? destinationFilePath)
    {
        try
        {
            using XLWorkbook workbook = new(destinationFilePath);

            XLWorkbook? nGWorkbook = null;

            IXLWorksheet? nGWorksheet = null;

            if (!File.Exists(ngFilePath))
            {
                workbook.SaveAs(ngFilePath);
            }

            nGWorkbook = new XLWorkbook(ngFilePath);

            nGWorksheet = nGWorkbook.Worksheet(sheetName);

            if (nGWorksheet is null)
            {
                throw new Exception(string.Format(Resources.ErrorExcelSheetNotFound, sheetName));
            }

            foreach (KeyValuePair<string, CellInfo> validationError in validationErrors)
            {
                foreach (DocumentMapperError error in validationError.Value.Errors)
                {
                    if (error.ErrorMessage is not null)
                    {
                        HighlightCell(nGWorksheet, error.Row, error.Column, XLColor.Red);

                        AddCommentToCell(nGWorksheet, error.Row, error.Column, error.ErrorMessage);
                    }
                }
            }

            nGWorkbook.Save();
        } catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogCreateFileAndFillCells}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorCreateFileAndFillCells);
        }
    }

    /// <inheritdoc />
    public string[,] ReadExcelFile(string filePath, string sheetName, string exportFormatDate)
    {
        try
        {
            logger.LogInformation($"{string.Format(Resources.LogReadExcel, filePath, sheetName)}");

            using XLWorkbook workbook = new(filePath);

            IXLWorksheet worksheet = workbook.Worksheet(sheetName) ??
                throw new Exception($"{string.Format(Resources.ErrorExcelSheetNotFound, sheetName)}");

            int rowCount = worksheet.RowsUsed().Count();
            int colCount = worksheet.ColumnsUsed().Count();

            string[,] data = new string[rowCount, colCount];

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    XLCellValue value = worksheet.Cell(row + 1, col + 1).Value;

                    if (value.IsDateTime)
                    {
                        if (value.TryConvert(out DateTime date))
                        {
                            data[row, col] = exportFormatDate is null ? date.ToString() : date.ToString(exportFormatDate);
                        }
                    }
                    else
                    {
                        data[row, col] = value.ToString();
                    }
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {string.Format(Resources.LogReadExcel, filePath, sheetName)}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception($"{string.Format(Resources.ErrorReadExcel, filePath, sheetName)} -> {ex.Message}");
        }
    }

    /// <summary>
    /// Закрашивает ячейку
    /// </summary>
    /// <param name="worksheet">Лист документа</param>
    /// <param name="row">Строка</param>
    /// <param name="column">Колонка</param>
    /// <param name="color">Цвет</param>
    private void HighlightCell(IXLWorksheet worksheet, int row, int column, XLColor color)
    {
        try
        {
            logger.LogInformation($"{string.Format(Resources.LogHighlightCell, row, column)}");

            worksheet.Cell(row, column).Style.Fill.BackgroundColor = color;

            logger.LogInformation($"{string.Format(Resources.LogHighlightCell, row, column)} {Resources.Completed}");
        } catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {string.Format(Resources.LogHighlightCell, row, column)}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(string.Format(Resources.ErrorHighlightCell, row, column));
        }
    }

    /// <summary>
    /// Добавляет выноску к ячейке с текстом
    /// </summary>
    /// <param name="worksheet">Лист документа</param>
    /// <param name="row">Строка</param>
    /// <param name="column">Колонка</param>
    /// <param name="commentText">Комментарий</param>
    private void AddCommentToCell(IXLWorksheet worksheet, int row, int column, string commentText)
    {
        try
        {
            logger.LogInformation($"{string.Format(Resources.LogAddCommentToCell, commentText, row, column)}");

            IXLComment comment = worksheet.Cell(row, column).GetComment();

            _ = comment.AddText(commentText);

            _ = comment.SetVisible();

            comment.FontSize = 10;

            logger.LogInformation($"{string.Format(Resources.LogAddCommentToCell, commentText, row, column)} {Resources.Completed}");
        } catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogAddCommentToCell}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(string.Format(Resources.ErrorAddCommentToCell, commentText, row, column));
        }
    }
}
