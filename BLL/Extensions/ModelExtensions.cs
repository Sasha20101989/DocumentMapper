using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;

using ClosedXML.Excel;

using doc_mapper.nuget.DAL.Models;
using doc_mapper.nuget.Properties;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace doc_mapper.nuget.BLL.Extensions
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Возвращает системное имя атрибута ColumnAttribute для указанного свойства в заданной модели.
        /// </summary>
        /// <param name="modelType">Модель, в которой будет производиться поиск свойства с атрибутом.</param>
        /// <param name="nameOfProperty">Имя свойства, для которого осуществляется поиск атрибута с системным именем.</param>
        /// <returns>Системное имя атрибута ColumnAttribute.</returns>
        /// <exception cref="ArgumentNullException">Вызывается, если свойство modelType является null</exception>
        /// <exception cref="InvalidOperationException">
        /// Вызывается, если свойство nameOfProperty не найдено, отсутствует атрибут ColumnAttribute или тип свойства не является string.
        /// Также вызывается, если атрибут ColumnAttribute или его свойство Name равны null.
        /// </exception>
        public static string GetSystemColumnName(this Type modelType, string nameOfProperty)
        {
            ArgumentNullException.ThrowIfNull(modelType);

            PropertyInfo? propertyModelInfo = modelType?.GetProperty(nameOfProperty);

            return propertyModelInfo != null && Attribute.IsDefined(propertyModelInfo, typeof(ColumnAttribute))
                ? propertyModelInfo?.GetCustomAttribute(typeof(ColumnAttribute)) is ColumnAttribute systemColumnNameAttribute && systemColumnNameAttribute.Name != null
                    ? systemColumnNameAttribute.Name
                    : throw new InvalidOperationException($"ColumnAttribute in {nameOfProperty} or its {nameOfProperty} property is null.")
                : throw new InvalidOperationException($"Property {nameOfProperty} not found, ColumnAttribute is missing, or the property type is not string.");
        }

        /// <summary>
        /// Возвращает значение из указанной строки в документе, приведенное к строковому типу.
        /// </summary>
        /// <param name="document">Маппер документа.</param>
        /// <param name="row">Индекс строки.</param>
        /// <param name="model">Тип модели данных.</param>
        /// <param name="nameOfProperty">Имя свойства.</param>
        /// <returns>Значение из ячейки строки в виде строки или null, если значение не найдено.</returns>
        public static string? GraftValueFromRow(this DocumentMapper document, int? row, Type model, string nameOfProperty)
        {
            object? cellValue = document.GetValue(model, nameOfProperty, row);

            return cellValue is null ? null : (cellValue?.ToString());
        }

        /// <summary>
        /// Заполняет значение свойства модели данных из указанной строки в документе.
        /// </summary>
        /// <typeparam name="T">Тип модели данных.</typeparam>
        /// <param name="document">Маппер документа.</param>
        /// <param name="row">Индекс строки.</param>
        /// <param name="model">Модель данных.</param>
        /// <param name="nameOfProperty">Имя свойства.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если свойство model является null.</exception>
        public static void GraftAndSetValueFromRow<T>(this DocumentMapper document, int row, T model, string nameOfProperty)
        {
            object? cellValue = document.GetValue(typeof(T), nameOfProperty, row);

            ArgumentNullException.ThrowIfNull(model);

            model.SetProperty(nameOfProperty, cellValue);
        }

        /// <summary>
        /// Валидация указанного свойства модели.
        /// </summary>
        /// <example>
        /// Пример использования:
        /// <code>
        /// Lot lot = new();
        /// Dictionary&lt;string, CellInfo&gt; validationResult;
        /// if (lot.TryValidateProperty(nameof(Lot.LotNumber), out validationResult))
        /// {
        ///     // Свойство прошло валидацию успешно
        /// }
        /// else
        /// {
        ///     // Обработка ошибок валидации
        ///     foreach (var error in validationResult)
        ///     {
        ///         Console.WriteLine($"Свойство: {error.Key}, Значение: {error.Value.Value}, Ошибки: {string.Join(", ", error.Value.Errors)}");
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <typeparam name="T">Тип модели.</typeparam>
        /// <param name="instance">Экземпляр модели.</param>
        /// <param name="propertyName">Имя свойства для валидации.</param>
        /// <param name="outResult">Результат валидации.</param>
        /// <returns>Возвращает true, если валидация прошла успешно, и false в противном случае.</returns>
        public static bool TryValidateProperty(this object instance, string propertyName, int row, int col, out Dictionary<string, CellInfo> outResult)
        {
            ArgumentNullException.ThrowIfNull(instance);

            outResult = [];

            PropertyInfo? propertyInfo = instance.GetType().GetProperty(propertyName);

            if (propertyInfo != null)
            {
                object? value = propertyInfo.GetValue(instance);

                List<ValidationResult> results = [];

                ValidationContext context = new(instance) { MemberName = propertyName };

                if (!Validator.TryValidateProperty(value, context, results))
                {
                    if (results is not null)
                    {
                        CellInfo cellInfo = new()
                        {
                            Errors = [],
                            Value = instance.GetPropertyValue(propertyName)
                        };

                        foreach (ValidationResult validationResult in results)
                        {
                            DocumentMapperError customError = new()
                            {
                                ErrorMessage = validationResult.ErrorMessage,
                                Row = row,
                                Column = col
                            };

                            cellInfo.Errors.Add(customError);

                            if (outResult.TryGetValue(propertyName, out CellInfo? cellValue))
                            {
                                cellValue.Errors.AddRange(cellInfo.Errors);
                            } else
                            {
                                outResult.Add(propertyName, cellInfo);
                            }
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Попытка установки значения и валидации свойства модели.
        /// </summary>
        /// <example>
        /// Пример использования:
        /// <code>
        /// Dictionary&lt;string, CellInfo&gt; validationResult;
        /// if (model.TryCreateAndValidateProperty(nameof(MyModel.MyProperty), "NewValue", out validationResult))
        /// {
        ///     // Свойство успешно установлено и прошло валидацию
        /// }
        /// else
        /// {
        ///     // Обработка ошибок валидации
        ///     foreach (var error in validationResult)
        ///     {
        ///         Console.WriteLine($"Свойство: {error.Key}, Значение: {error.Value.Value}, Ошибки: {string.Join(", ", error.Value.Errors)}");
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="model">Экземпляр модели.</param>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="propertyValue">Значение для установки.</param>
        /// <param name="outResult">Результат валидации.</param>
        /// <returns>Возвращает true, если валидация прошла успешно, и false в противном случае.</returns>
        public static bool TrySetAndValidateProperty(this object model, string propertyName, object propertyValue, int? row, int col, out Dictionary<string, CellInfo> outResult)
        {
            outResult = [];

            model.SetProperty(propertyName, propertyValue);

            if (row is not null)
            {
                if (!model.TryValidateProperty(propertyName, (int)row, col, out Dictionary<string, CellInfo> validationResult))
                {
                    outResult = validationResult;
                }
            }

            return outResult.Count == 0;
        }

        /// <summary>
        /// Заполняет заголовочную строку в рабочем листе Excel.
        /// </summary>
        /// <param name="content">Коллекция данных для заполнения.</param>
        /// <param name="worksheet">Рабочий лист Excel.</param>
        /// <param name="rowIndex">Индекс строки, в которую происходит запись.</param>
        public static void WriteInToWorksheetHeaderRow(this IEnumerable<DocumentMapperContent> content, IXLWorksheet worksheet, ref int rowIndex)
        {
            int columnIndex = 1;

            foreach (DocumentMapperContent item in content.OrderBy(c => c.ColumnNr))
            {
                worksheet.Cell(rowIndex, columnIndex).Value = item.DocumentMapperColumn.ElementName;

                columnIndex++;
            }
        }

        /// <summary>
        /// Заполняет строки данных в рабочем листе Excel.
        /// </summary>
        /// <typeparam name="T">Тип данных в коллекции экспортируемых данных.</typeparam>
        /// <param name="content">Коллекция данных для заполнения.</param>
        /// <param name="worksheet">Рабочий лист Excel.</param>
        /// <param name="exportData">Коллекция экспортируемых данных.</param>
        /// <param name="rowIndex">Индекс строки, в которую происходит запись.</param>
        /// <param name="dateIndexes">Список индексов столбцов, содержащих даты.</param>
        public static void WriteInToWorksheetDataRows<T>(this IEnumerable<DocumentMapperContent> content, IXLWorksheet worksheet, IEnumerable<T> exportData, ref int rowIndex, List<int> dateIndexes)
        {
            foreach (T item in exportData)
            {
                int columnIndex = 1;

                foreach (DocumentMapperContent contentItem in content.OrderBy(c => c.ColumnNr))
                {
                    object? propValue = item.GetPropertyByColumnAttribute(contentItem.DocumentMapperColumn.SystemColumnName);

                    if (DateTime.TryParse(propValue?.ToString(), out DateTime date))
                    {
                        worksheet.Cell(rowIndex, columnIndex).Value = date;

                        if (!dateIndexes.Contains(columnIndex))
                        {
                            dateIndexes.Add(columnIndex);
                        }
                    } else
                    {
                        worksheet.Cell(rowIndex, columnIndex).Value = propValue?.ToString();
                    }

                    columnIndex++;
                }

                rowIndex++;
            }
        }

        /// <summary>
        /// Валидирует заголовки табличной части документа, что они совпадают с картой документов
        /// </summary>
        /// <param name="excelData"></param>
        /// <param name="firstRow">Строка с заголовками</param>
        /// <returns>Метод возвращающий результат валидации, если заголовки валидны то возвращает массив с данными, иначе null</returns>
        /// <exception cref="Exception">Вызывается если ожидаемая колонка не совпадет с текущей.</exception>
        public static string[,]? ValidateHeaders(this IEnumerable<DocumentMapperContent> content, string[,] excelData, int firstRow)
        {
            bool hasError = false;
            int row = firstRow - 1;

            foreach (DocumentMapperContent? contentItem in content.OrderBy(item => item.ColumnNr))
            {
                if (contentItem.RowNr is not null)
                {
                    continue;
                }

                string cell = excelData[row, contentItem.ColumnNr - 1];

                if (!string.IsNullOrEmpty(cell) &&
                    !string.Equals(cell, contentItem?.DocumentMapperColumn?.ElementName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(string.Format(Resources.ErrorExcelHeaderValidation, firstRow, contentItem?.DocumentMapperColumn?.ElementName, cell));
                }
            }

            return hasError ? null : excelData;
        }

        private static object? GetPropertyByColumnAttribute<T>(this T model, string attributeName)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            PropertyInfo? prop = properties?.FirstOrDefault(property =>
            {
                if (Attribute.IsDefined(property, typeof(ColumnAttribute)))
                {
                    ColumnAttribute? columnAttribute = property?.GetCustomAttribute<ColumnAttribute>();

                    return columnAttribute != null && columnAttribute.Name == attributeName;
                } else
                {
                    throw new InvalidOperationException($"More than one model found with the specified ColumnAttribute '{attributeName}'.");
                }
            });

            return prop is null
                ? throw new InvalidOperationException($"A property with the system name '{attributeName}' was not found.")
                : prop.GetValue(model);
        }

        private static object? GetPropertyValue(this object model, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(model);

            PropertyInfo? propertyInfo = model.GetType().GetProperty(propertyName);

            return propertyInfo == null
                ? throw new ArgumentException($"Property {propertyName} not found in type {model.GetType().Name}")
                : propertyInfo.GetValue(model);
        }

        private static void SetProperty(this object model, string propertyName, object? value)
        {
            ArgumentNullException.ThrowIfNull(model);

            PropertyInfo? propertyInfo = model.GetType().GetProperty(propertyName) ?? throw new ArgumentException($"Поле '{propertyName}' не найдено в модели данных '{model.GetType().Name}'");

            Type? propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(decimal) || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(propertyType) == typeof(decimal)))
            {
                if (value is null)
                {
                    propertyInfo.SetValue(model, value);
                } else if (value.ToString() == "")
                {
                    propertyInfo.SetValue(model, null);
                } else if (string.IsNullOrWhiteSpace(value.ToString()))
                {
                    propertyInfo.SetValue(model, null);
                } else
                {
                    if (decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out decimal decimalValue))
                    {
                        propertyInfo.SetValue(model, decimalValue);
                    } else
                    {
                        throw new ArgumentException($"Ошибка в поле '{propertyName}', не верный формат данных.");
                    }
                }
            } else if (propertyType == typeof(int) || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(propertyType) == typeof(int)))
            {
                if (int.TryParse(value?.ToString(), out int intValue))
                {
                    propertyInfo.SetValue(model, intValue);
                } else
                {
                    throw new ArgumentException($"Ошибка в поле '{propertyName}', не верный формат данных.");
                }
            } else if (propertyType == typeof(string))
            {
                propertyInfo.SetValue(model, value?.ToString());
            } else if (propertyType == typeof(DateTime) || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(propertyType) == typeof(DateTime)))
            {
                string[] formats = { "dd.MM.yyyy", "yyyy-MM-dd", "MM/dd/yyyy" };

                if (DateTime.TryParseExact(value?.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeValue))
                {
                    propertyInfo.SetValue(model, dateTimeValue);
                } else
                {
                    throw new ArgumentException($"Ошибка в поле '{propertyName}', не верный формат данных.");
                }
            } else
            {
                throw new NotImplementedException($"Convert value to '{propertyType}' for property '{propertyName}' not implemented.");
            }
        }

        private static object? GetValue(this DocumentMapper document, Type modelType, string nameOfProperty, int? row = null)
        {
            string systemName = modelType.GetSystemColumnName(nameOfProperty);

            DocumentMapperContent? content = document.DocumentMapperContents?.FirstOrDefault(dc => dc.DocumentMapperColumn.SystemColumnName == systemName);

            if (content is not null && content.RowNr is not null)
            {
                return document.Data.GetValue((int)content.RowNr - 1, content.ColumnNr - 1);
            } else if (content is not null && row is not null)
            {
                return document.Data?.GetValue((int)row, content.ColumnNr - 1);
            }

            return null;
        }
    }
}
