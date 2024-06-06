namespace doc_mapper.nuget.Enums;
/// <summary>
/// Перечисление представляющее шаги процесса "AppProcess"(отражение поля Step_Name в таблице tbd_Processes_Steps)
/// </summary>
public enum Steps
{
    /// <summary>
    /// Загрузка контейнеров без типов контейнеров и товаров и цен
    /// </summary>
    UploadLotContent,
    /// <summary>
    /// Загрузка типов контейнеров
    /// </summary>
    UploadContainerTypes,
    /// <summary>
    /// Загрузка цен товаров
    /// </summary>
    UploadPrice,
    /// <summary>
    /// Экспорт трейсинга
    /// </summary>
    ExportTracing,
    /// <summary>
    /// Экспорт таможенной очистки
    /// </summary>
    ExportCustoms,
    /// <summary>
    /// Загрузка локальных винов
    /// </summary>
    UploadPPP,
    /// <summary>
    /// Обновление трейсинга
    /// </summary>
    UpdateTracing,
    /// <summary>
    /// Обновление таможенной очистки
    /// </summary>
    UpdateCustoms,
    /// <summary>
    /// Экспорт графика
    /// </summary>
    ExportSchedule,
    /// <summary>
    /// Экспорт таможенных деталей
    /// </summary>
    ExportCustomsParts,
    /// <summary>
    /// Экспорт для SFS
    /// </summary>
    ExportForSFS,
}
