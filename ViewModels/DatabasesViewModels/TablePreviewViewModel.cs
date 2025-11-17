// ViewModels/TablePreviewViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;
using Npgsql;
using System.Data;
using System.Globalization;


namespace RatingApp.ViewModels
{
    public partial class TablePreviewViewModel : ObservableObject
    {
        private readonly IRatingService _ratingService;
        private readonly Database _database;
        private readonly string _tableName;

        [ObservableProperty]
        private TableData tableData = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string currentFilterInfo;

        public bool IsNotLoading => !IsLoading;
        public bool HasData => TableData?.Rows?.Count > 0;
        public bool HasNoData => !HasData && IsNotLoading;

        public event EventHandler DataLoaded;

        
        public TablePreviewViewModel(IRatingService ratingService, Database database, string tableName)
        {
            _ratingService = ratingService;
            _database = database;
            _tableName = tableName;
            
            TableData.TableName = tableName;
            TableData.PreviewRowsCount = 15;
            CurrentFilterInfo = "Без фильтров";
        }

        public async void InitializeData()
        {
            await LoadTableDataAsync();
        }

        [RelayCommand]
        private async Task LoadTableDataAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                TableData.IsLoading = true;

                if (_database.Type != DatabaseType.PostgreSQL)
                {
                    await LoadDemoData();
                    OnDataLoaded();
                    return;
                }

                await LoadDataFromDatabase();
                OnDataLoaded();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    $"Не удалось загрузить данные таблицы: {ex.Message}", "OK");
                await LoadDemoData();
                OnDataLoaded();
            }
            finally
            {
                IsLoading = false;
                TableData.IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            await LoadTableDataAsync();
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }


        [RelayCommand]
        private async Task ShowFilterDialogAsync(TableColumn column)
        {
            if (column == null) return;

            // Запрашиваем значение для фильтра
            var filterValue = await Application.Current.MainPage.DisplayPromptAsync(
                $"Фильтр по колонке: {column.Name}",
                $"Введите значение для фильтрации ({column.DataType}):",
                "Применить",
                "Отмена",
                initialValue: column.FilterValue,
                maxLength: 100,
                keyboard: Keyboard.Text);

            if (filterValue == null) return; // Пользователь нажал "Отмена"

            // Если поле пустое - сбрасываем фильтр
            if (string.IsNullOrWhiteSpace(filterValue))
            {
                column.FilterValue = null;
                column.IsFiltered = false;
                await ClearFiltersAsync();
                return;
            }

            // Проверяем и преобразуем значение в соответствии с типом данных
            var validationResult = ValidateAndConvertFilterValue(filterValue.Trim(), column.DataType);
            if (!validationResult.IsValid)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка ввода", 
                    validationResult.ErrorMessage, 
                    "OK");
                return;
            }

            // Устанавливаем значение фильтра
            column.FilterValue = validationResult.ConvertedValue;
            column.IsFiltered = true;

            // Применяем фильтр
            await ApplyAllFiltersAsync();
        }

        public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ConvertedValue { get; set; }
        public string ErrorMessage { get; set; }

        public static ValidationResult Valid(string convertedValue)
        {
            return new ValidationResult 
            { 
                IsValid = true, 
                ConvertedValue = convertedValue 
            };
        }

        public static ValidationResult Invalid(string errorMessage)
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = errorMessage 
            };
        }
    }

        private ValidationResult ValidateAndConvertFilterValue(string value, string dataType)
        {
            if (string.IsNullOrEmpty(value))
                return ValidationResult.Invalid("Значение не может быть пустым");

            try
            {
                // Приводим тип данных к нижнему регистру для сравнения
                var lowerDataType = dataType.ToLowerInvariant();

                // INTEGER, INT, BIGINT, SMALLINT
                if (lowerDataType.Contains("int") || lowerDataType.Contains("serial"))
                {
                    if (long.TryParse(value, out long intValue))
                    {
                        return ValidationResult.Valid(intValue.ToString());
                    }
                    return ValidationResult.Invalid($"Некорректное целое число: {value}");
                }

                // DECIMAL, NUMERIC, REAL, DOUBLE PRECISION, FLOAT
                else if (lowerDataType.Contains("decimal") || 
                         lowerDataType.Contains("numeric") || 
                         lowerDataType.Contains("real") || 
                         lowerDataType.Contains("double") || 
                         lowerDataType.Contains("float"))
                {
                    // Заменяем запятую на точку для корректного парсинга
                    var normalizedValue = value.Replace(',', '.');
                    if (double.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double decimalValue))
                    {
                        return ValidationResult.Valid(decimalValue.ToString(CultureInfo.InvariantCulture));
                    }
                    return ValidationResult.Invalid($"Некорректное число: {value}");
                }

                // BOOLEAN, BOOL
                else if (lowerDataType.Contains("bool"))
                {
                    var lowerValue = value.ToLowerInvariant();
                    if (lowerValue == "true" || lowerValue == "1" || lowerValue == "t" || lowerValue == "yes" || lowerValue == "y")
                    {
                        return ValidationResult.Valid("true");
                    }
                    else if (lowerValue == "false" || lowerValue == "0" || lowerValue == "f" || lowerValue == "no" || lowerValue == "n")
                    {
                        return ValidationResult.Valid("false");
                    }
                    return ValidationResult.Invalid($"Некорректное логическое значение: {value}. Используйте true/false, 1/0, yes/no");
                }

                // DATE, TIME, TIMESTAMP
                else if (lowerDataType.Contains("date") || lowerDataType.Contains("time"))
                {
                    if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dateValue))
                    {
                        // Для дат используем формат ISO
                        return ValidationResult.Valid($"'{dateValue:yyyy-MM-dd HH:mm:ss}'");
                    }
                    return ValidationResult.Invalid($"Некорректная дата/время: {value}");
                }

                // TEXT, VARCHAR, CHAR, STRING (строковые типы)
                else if (lowerDataType.Contains("char") || 
                         lowerDataType.Contains("text") || 
                         lowerDataType.Contains("string") ||
                         lowerDataType.Contains("varchar"))
                {
                    // Экранируем одинарные кавычки для SQL
                    var escapedValue = value.Replace("'", "''");
                    return ValidationResult.Valid($"'{escapedValue}'");
                }

                // UUID
                else if (lowerDataType.Contains("uuid"))
                {
                    if (Guid.TryParse(value, out Guid uuidValue))
                    {
                        return ValidationResult.Valid($"'{uuidValue}'");
                    }
                    return ValidationResult.Invalid($"Некорректный UUID: {value}");
                }

                // JSON, JSONB
                else if (lowerDataType.Contains("json"))
                {
                    // Базовая проверка JSON (проверяем фигурные скобки)
                    var trimmed = value.Trim();
                    if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) || 
                        (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
                    {
                        var escapedValue = value.Replace("'", "''");
                        return ValidationResult.Valid($"'{escapedValue}'");
                    }
                    return ValidationResult.Invalid($"Некорректный JSON: {value}");
                }

                // По умолчанию считаем строковым типом
                else
                {
                    var escapedValue = value.Replace("'", "''");
                    return ValidationResult.Valid($"'{escapedValue}'");
                }
            }
            catch (Exception ex)
            {
                return ValidationResult.Invalid($"Ошибка преобразования: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ClearFiltersAsync()
        {
            try
            {
                IsLoading = true;
                TableData.IsLoading = true;

                // Сбрасываем все фильтры
                foreach (var column in TableData.Columns)
                {
                    column.FilterValue = null;
                    column.IsFiltered = false;
                }

                CurrentFilterInfo = "Без фильтров";

                // Загружаем данные без фильтров (с LIMIT 15)
                if (_database.Type == DatabaseType.PostgreSQL)
                {
                    await LoadDataFromDatabase();
                }
                else
                {
                    await LoadDemoData();
                }

                OnDataLoaded();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    $"Не удалось сбросить фильтры: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
                TableData.IsLoading = false;
            }
        }

        private async Task ApplyAllFiltersAsync()
        {
            try
            {
                IsLoading = true;
                TableData.IsLoading = true;

                if (_database.Type != DatabaseType.PostgreSQL)
                {
                    await Application.Current.MainPage.DisplayAlert("Информация", 
                        "Фильтрация доступна только для PostgreSQL баз данных", "OK");
                    return;
                }

                await ApplyFiltersToDatabase();
                ForceTableRefresh();
                
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", 
                    $"Не удалось применить фильтр: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
                TableData.IsLoading = false;
            }
        }

        private async Task ApplyFiltersToDatabase()
        {
            NpgsqlConnection connection = null;
            try
            {
                connection = new NpgsqlConnection(_database.ConnectionString);
                await connection.OpenAsync();

                // Получаем активные фильтры
                var activeFilters = TableData.Columns
                    .Where(c => c.IsFiltered && !string.IsNullOrEmpty(c.FilterValue))
                    .ToList();

                if (activeFilters.Count > 0)
                {
                    // Строим WHERE условие
                    var whereConditions = new List<string>();
                    var parameters = new Dictionary<string, object>();

                    foreach (var col in activeFilters)
                    {
                        // Используем уже преобразованные значения
                        whereConditions.Add($"\"{col.Name}\" = {col.FilterValue}");
                    }

                    var whereClause = $"WHERE {string.Join(" AND ", whereConditions)}";
                    
                    // Загружаем данные с фильтрами БЕЗ LIMIT
                    await LoadTableRowsWithFilter(connection, whereClause);
                    
                    // Получаем общее количество строк с фильтром
                    await LoadTotalRowCountWithFilter(connection, whereClause);

                    System.Diagnostics.Debug.WriteLine($"Applied filters: {whereClause}");
                }
                else
                {
                    // Если фильтров нет, загружаем с LIMIT 15
                    await LoadTableRows(connection);
                    await LoadTotalRowCount(connection);
                }

                // Обновляем информацию о фильтрах
                UpdateFilterInfo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FILTER_APPLY_ERROR: {ex.Message}");
                
                // Показываем ошибку пользователю
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка фильтрации", 
                    $"Не удалось применить фильтр: {ex.Message}", 
                    "OK");
                
                throw;
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }
        private async Task LoadTableRowsWithFilter(NpgsqlConnection connection, string whereClause)
        {
            var newRows = new List<Dictionary<string, object>>();
            var query = $"SELECT * FROM \"{_tableName}\" {whereClause}";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[columnName] = value;
                }
                
                newRows.Add(row);
            }

            TableData.Rows = newRows;
            
            System.Diagnostics.Debug.WriteLine($"Loaded {TableData.Rows.Count} rows with filter");
        }

        private async Task LoadTotalRowCountWithFilter(NpgsqlConnection connection, string whereClause)
        {
            var query = $"SELECT COUNT(*) FROM \"{_tableName}\" {whereClause}";
            
            using var command = new NpgsqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();
            TableData.TotalRows = Convert.ToInt32(result);
        }

        private void UpdateFilterInfo()
        {
            var activeFilters = TableData.Columns
                .Where(c => c.IsFiltered && !string.IsNullOrEmpty(c.FilterValue))
                .Select(c => $"{c.Name} = '{c.FilterValue}'")
                .ToList();

            if (activeFilters.Count > 0)
            {
                CurrentFilterInfo = $"Фильтры: {string.Join(", ", activeFilters)}";
            }
            else
            {
                CurrentFilterInfo = "Без фильтров";
            }
        }

        [RelayCommand]
        public async Task CopyToClipboardAsync(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                await Clipboard.Default.SetTextAsync(text);
                
                // Показываем краткое уведомление вместо алерта
                // Это менее навязчиво для пользователя
                await Application.Current.MainPage.DisplayAlert("Успех", "Текст скопирован в буфер обмена", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось скопировать текст: {ex.Message}", "OK");
            }
        }

        private void OnDataLoaded()
        {
            DataLoaded?.Invoke(this, EventArgs.Empty);
            System.Diagnostics.Debug.WriteLine("DataLoaded event fired");
        }

        private async Task LoadDataFromDatabase()
        {
            NpgsqlConnection connection = null;
            try
            {
                connection = new NpgsqlConnection(_database.ConnectionString);
                await connection.OpenAsync();

                // Получаем информацию о колонках (только если еще не загружены)
                if (TableData.Columns == null || TableData.Columns.Count == 0)
                {
                    await LoadTableColumns(connection);
                }

                // Получаем данные таблицы С LIMIT 15
                await LoadTableRows(connection);

                // Получаем общее количество строк
                await LoadTotalRowCount(connection);

                System.Diagnostics.Debug.WriteLine($"Data loaded: {TableData.Rows.Count} rows, {TableData.Columns.Count} columns");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TABLE_DATA_LOAD_ERROR: {ex.Message}");
                throw;
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }

        public void ForceTableRefresh()
        {
            OnDataLoaded();
            
            // Принудительно обновляем свойства
            OnPropertyChanged(nameof(TableData));
            OnPropertyChanged(nameof(TableData.Rows));
            OnPropertyChanged(nameof(TableData.Columns));
            OnPropertyChanged(nameof(TableData.TotalRows));
            
            System.Diagnostics.Debug.WriteLine($"ForceTableRefresh: {TableData.Rows?.Count} rows");
        }


        private async Task LoadTableColumns(NpgsqlConnection connection)
        {
            var columns = new List<TableColumn>();
            
            using var command = new NpgsqlCommand(@"
                SELECT column_name, data_type, ordinal_position
                FROM information_schema.columns 
                WHERE table_schema = 'public' 
                AND table_name = @tableName
                ORDER BY ordinal_position", connection);
                
            command.Parameters.AddWithValue("tableName", _tableName);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                columns.Add(new TableColumn
                {
                    Name = reader.GetString(0),
                    DataType = reader.GetString(1),
                    OrdinalPosition = reader.GetInt32(2)
                });
            }

            TableData.Columns = columns;
        }

        private async Task LoadTableRows(NpgsqlConnection connection)
        {
            var rows = new List<Dictionary<string, object>>();
            // ЗАПРОС С LIMIT 15 для обычного превью
            var query = $"SELECT * FROM \"{_tableName}\" LIMIT {TableData.PreviewRowsCount}";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[columnName] = value;
                }
                
                rows.Add(row);
            }

            TableData.Rows = rows;
        }

        private async Task LoadTotalRowCount(NpgsqlConnection connection)
        {
            using var command = new NpgsqlCommand($"SELECT COUNT(*) FROM \"{_tableName}\"", connection);
            var result = await command.ExecuteScalarAsync();
            TableData.TotalRows = Convert.ToInt32(result);
        }

        private Task LoadDemoData()
        {
            // Демо-данные для примера
            if (_tableName.ToLower().Contains("auth") || _tableName.ToLower().Contains("permission"))
            {
                TableData.Columns = new List<TableColumn>
                {
                    new TableColumn { Name = "id", DataType = "integer", OrdinalPosition = 1 },
                    new TableColumn { Name = "name", DataType = "character varying", OrdinalPosition = 2 },
                    new TableColumn { Name = "content_type_id", DataType = "integer", OrdinalPosition = 3 },
                    new TableColumn { Name = "codename", DataType = "character varying", OrdinalPosition = 4 }
                };

                TableData.Rows = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { {"id", 1}, {"name", "Can add log entry"}, {"content_type_id", 1}, {"codename", "add_logentry"} },
                    new Dictionary<string, object> { {"id", 2}, {"name", "Can change log entry"}, {"content_type_id", 1}, {"codename", "change_logentry"} },
                    new Dictionary<string, object> { {"id", 3}, {"name", "Can delete log entry"}, {"content_type_id", 1}, {"codename", "delete_logentry"} }
                };

                TableData.TotalRows = 15;
            }
            else
            {
                TableData.Columns = new List<TableColumn>
                {
                    new TableColumn { Name = "id", DataType = "INTEGER", OrdinalPosition = 1 },
                    new TableColumn { Name = "well_name", DataType = "VARCHAR", OrdinalPosition = 2 },
                    new TableColumn { Name = "depth", DataType = "DECIMAL", OrdinalPosition = 3 },
                    new TableColumn { Name = "pressure", DataType = "DECIMAL", OrdinalPosition = 4 }
                };

                TableData.Rows = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { {"id", 1}, {"well_name", "Скважина-001"}, {"depth", 2450.5}, {"pressure", 145.2} },
                    new Dictionary<string, object> { {"id", 2}, {"well_name", "Скважина-002"}, {"depth", 3120.8}, {"pressure", 167.8} }
                };

                TableData.TotalRows = 15420;
            }

            return Task.CompletedTask;
        }
    }
}