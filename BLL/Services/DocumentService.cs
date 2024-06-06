using doc_mapper.nuget.BLL.Contracts;
using doc_mapper.nuget.DAL.Context;
using doc_mapper.nuget.DAL.Models;
using doc_mapper.nuget.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace doc_mapper.nuget.BLL.Services;

/// <summary>
/// Интерфейс сервиса для операций, связанных с картами сопоставления данных для эксель.
/// </summary>
/// <remarks>
/// Инициализирует новый экземпляр класса <see cref="DocumentService"/>.
/// </remarks>
/// <param name="documentMapperRepository">Репозиторий для доступа к информации о картах сопоставления данных для эксель.</param>
/// <param name="logger">Регистратор для отслеживания информации и ошибок.</param>
public class DocumentService(DocumentMapperContext db, ILogger<DocumentService> logger) : IDocumentService
{
    /// <inheritdoc />
    public async Task<List<DocumentMapper>> GetFilteredDocumentsAsync(string docmapperName)
    {
        try
        {
            logger.LogInformation($"{Resources.LogDocmapperGetAllFiltered}");

            List<DocumentMapper> docmappers = await db.DocumentMappers
                .Where(document => EF.Functions.Like(document.DocumentMapperName, $"%{docmapperName}%"))
                .ToListAsync();

            logger.LogInformation($"{Resources.LogDocmapperGetAllFiltered} {Resources.Completed}");

            return docmappers;
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogDocmapperGetAllFiltered}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorDocmapperGetAllFiltered);
        }
    }

    /// <inheritdoc />
    public async Task<List<DocumentMapper>> GetAllDocumentsAsync()
    {
        try
        {
            logger.LogInformation($"{Resources.LogDocmapperGetAll}");

            List<DocumentMapper> docmappers = await db.DocumentMappers
                .Include(d => d.DocumentMapperContents)
                .ThenInclude(d => d.DocumentMapperColumn)
                .ToListAsync();

            logger.LogInformation($"{Resources.LogDocmapperGetAll} {Resources.Completed}");

            return docmappers;
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogDocmapperGetAll}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorDocmapperGetAll);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DocumentMapperColumn>> GetAllColumnsAsync()
    {
        try
        {
            logger.LogInformation($"{Resources.LogDocmapperColumnGetAll}");

            List<DocumentMapperColumn> columns = await db.DocumentMapperColumns
                .OrderBy(dc => dc.ElementName)
                .ToListAsync();

            logger.LogInformation($"{Resources.LogDocmapperColumnGetAll} {Resources.Completed}");

            return columns;
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogDocmapperColumnGetAll}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorDocmapperColumnGetAll);
        }
    }

    /// <inheritdoc />
    public async Task AddDocumentColumnAsync(DocumentMapperColumn newDocumentColumn)
    {
        try
        {
            logger.LogInformation($"{Resources.LogDocmapperColumnAdd}");

            db.Entry(newDocumentColumn).State = EntityState.Added;

            await SaveContextAsync();

            logger.LogInformation($"{Resources.LogDocmapperColumnAdd} {Resources.Completed}");
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogDocmapperColumnGetAll}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorDocmapperColumnAdd);
        }
    }

    /// <inheritdoc />
    public async Task CreateDocumentAsync(DocumentMapper document)
    {
        try
        {
            logger.LogInformation($"{Resources.LogDocmapperAdd}: '{JsonConvert.SerializeObject(document)}'");

            db.Entry(document).State = EntityState.Added;

            await SaveContextAsync();

            logger.LogInformation($"{Resources.LogDocmapperAdd} {Resources.Completed}");
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogDocmapperAdd}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorDocmapperColumnAdd);
        }
    }

    /// <inheritdoc />
    public async Task UpdateDocumentAsync(DocumentMapper document)
    {
        try
        {
            DocumentMapper? existingDocument = await db.DocumentMappers.FindAsync(document.Id) ?? 
                throw new Exception(string.Format(Resources.ErrorDocumentWithIdNotFound, document.Id));

            logger.LogInformation($"{Resources.LogDocmapperUpdate}");

            existingDocument.DocumentMapperName = document.DocumentMapperName;
            existingDocument.SheetName = document.SheetName;
            existingDocument.FirstDataRow = document.FirstDataRow;
            existingDocument.DefaultFolder = document.DefaultFolder;
            existingDocument.IsActive = document.IsActive;

            await SaveContextAsync();

            logger.LogInformation($"{Resources.LogDocmapperUpdate} {Resources.Completed}");
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogDocmapperUpdate}: {JsonConvert.SerializeObject(document)}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorDocmapperUpdate);
        }
    }
 
    /// <inheritdoc />
    public async Task MarkToDeleteDocumentContentItemAsync(DocumentMapperContent docmapperContent)
    {
        if (docmapperContent.Id != 0)
        {
            DocumentMapperContent existingContent = await GetExistingContentById(docmapperContent.Id);

            _ = db.Entry(existingContent).State = EntityState.Deleted;
        } else
        {
            DocumentMapperContent existingContent = GetExistingContentByDocumentMapperColumnIdAndDocumentMapperId(docmapperContent.Id, docmapperContent.DocumentMapperColumnId, docmapperContent.DocumentMapper.Id);

            _ = db.DocumentMapperContents.Local.Remove(existingContent);
        }
    }

    /// <inheritdoc />
    public void MarkToAddNewDocumentContentItem(DocumentMapperContent docmapperContent)
    {
        db.Entry(docmapperContent).State = EntityState.Added;
    }

    /// <inheritdoc />
    public async Task UpdateRowNumberInDocumentContentItemAsync(DocumentMapperContent docmapperContent)
    {         
        try
        {
            logger.LogInformation($"{Resources.LogDocmapperContentUpdate}");

            if (docmapperContent.Id != 0)
            {
                DocumentMapperContent existingContent = await GetExistingContentById(docmapperContent.Id);

                existingContent.RowNr = docmapperContent.RowNr;
            }
            else
            {
                DocumentMapperContent existingContent = GetExistingContentByDocumentMapperColumnIdAndDocumentMapperId(docmapperContent.Id, docmapperContent.DocumentMapperColumnId, docmapperContent.DocumentMapper.Id);

                existingContent.RowNr = docmapperContent.RowNr;
            }

            logger.LogInformation($"{Resources.LogDocmapperContentUpdate} {Resources.Completed}");
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogDocmapperContentUpdate}: {JsonConvert.SerializeObject(docmapperContent)}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorDocmapperContentUpdate);
        }
    }

    /// <inheritdoc />
    public async Task UpdateColumnNumberInDocumentContentItemAsync(DocumentMapperContent docmapperContent)
    {
        try
        {
            logger.LogInformation($"{Resources.LogDocmapperContentUpdate}");

            if (docmapperContent.Id != 0)
            {
                DocumentMapperContent existingContent = await GetExistingContentById(docmapperContent.Id);

                existingContent.ColumnNr = docmapperContent.ColumnNr;
            }
            else
            {
                DocumentMapperContent existingContent = GetExistingContentByDocumentMapperColumnIdAndDocumentMapperId(docmapperContent.Id, docmapperContent.DocumentMapperColumnId, docmapperContent.DocumentMapper.Id);

                existingContent.ColumnNr = docmapperContent.ColumnNr;
            }

            logger.LogInformation($"{Resources.LogDocmapperContentUpdate} {Resources.Completed}");
        }
        catch (Exception ex)
        {
            logger.LogError($"{Resources.LogError} {Resources.LogDocmapperContentUpdate}: {JsonConvert.SerializeObject(docmapperContent)}: {JsonConvert.SerializeObject(ex)}");

            throw new Exception(Resources.ErrorDocmapperContentUpdate);
        }
    }

    private async Task<DocumentMapperContent> GetExistingContentById(int docmapperContentId)
    {
        return await db.DocumentMapperContents.FindAsync(docmapperContentId) ??
            throw new Exception(string.Format(Resources.ErrorDocumentContentWithIdNotFound, docmapperContentId));
    }

    private DocumentMapperContent GetExistingContentByDocumentMapperColumnIdAndDocumentMapperId(int docmapperContentId, int documentMapperColumnId, int documentMapperId)
    {
        return db.DocumentMapperContents.Local.FirstOrDefault(dc => dc.DocumentMapperColumnId == documentMapperColumnId && dc.DocumentMapperId == documentMapperId) ??
                    throw new Exception(string.Format(Resources.ErrorDocumentContentWithDocmapperColumnIdNotFound, docmapperContentId));
    }

    private async Task SaveContextAsync()
    {
        _ = await db.SaveChangesAsync();
    }
}
