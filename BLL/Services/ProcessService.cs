using doc_mapper.nuget.BLL.Contracts;
using doc_mapper.nuget.DAL.Context;
using doc_mapper.nuget.DAL.Models;
using doc_mapper.nuget.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace doc_mapper.nuget.BLL.Services;

public class ProcessService(IUserService userService, DocumentMapperContext context) : IProcessService
{
    /// <inheritdoc />
    public async Task<List<ProcessesStep>> GetProcessStepsByUserSectionAsync(AppProcess appProcess, string userAccount)
    {
        User user = await userService.GetCurrentUserAsync(userAccount);

        IQueryable<ProcessesStep> processesQuery = context.ProcessesSteps
            .AsNoTracking()
            .Include(p => p.DocumentMapper)
            .ThenInclude(s => s.DocumentMapperContents)
            .ThenInclude(s => s.DocumentMapperColumn)
            .Include(p => p.Process)
            .Include(p => p.Section);

        if (user.Section.SectionName == Enums.Section.IS.ToString())
        {
            processesQuery = processesQuery.Where(c => c.Process.ProcessName == appProcess.ToString());
        } else
        {
            processesQuery = processesQuery.Where(c => c.Process.ProcessName == appProcess.ToString() && user.SectionId == c.SectionId);
        }

        return await processesQuery.ToListAsync();
    }
}
