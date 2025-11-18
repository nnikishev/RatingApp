using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RatingApp.Models;
using RatingApp.Services;
using Npgsql;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace RatingApp.ViewModels
{
    public partial class SqlShellViewModel : ObservableObject
    {
        private readonly Database _database;
        private readonly IDatasetService _datasetService;

        [ObservableProperty]
        private string sqlQuery = string.Empty;

        [ObservableProperty]
        private string formattedResults = string.Empty;

        [ObservableProperty]
        private string resultsInfo = string.Empty;

        [ObservableProperty]
        private string executionTime = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isExecuting;

        [ObservableProperty]
        private bool hasResults;

        [ObservableProperty]
        private bool hasError;

        public bool IsNotExecuting => !IsExecuting;
        public string DatabaseName => _database?.Name ?? "Unknown";

        public SqlShellViewModel(Database database)
        {
            _database = database;
        }

        [RelayCommand]
        private async Task ExecuteQueryAsync()
        {
            if (string.IsNullOrWhiteSpace(SqlQuery))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите SQL запрос", "OK");
                return;
            }

            IsExecuting = true;
            HasResults = false;
            HasError = false;
            ErrorMessage = string.Empty;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (_database.Type != DatabaseType.PostgreSQL)
                {
                    throw new NotSupportedException("Поддерживается только PostgreSQL");
                }

                using var connection = new NpgsqlConnection(_database.ConnectionString);
                await connection.OpenAsync();

                using var command = new NpgsqlCommand(SqlQuery, connection);
                
                if (IsSelectQuery(SqlQuery))
                {
                    // Для SELECT запросов
                    using var reader = await command.ExecuteReaderAsync();
                    await ProcessSelectResults(reader);
                }
                else
                {
                    // Для INSERT, UPDATE, DELETE и других запросов
                    var affectedRows = await command.ExecuteNonQueryAsync();
                    FormatNonQueryResults(affectedRows);
                }

                stopwatch.Stop();
                ExecutionTime = $"Выполнено за {stopwatch.Elapsed.TotalSeconds:F2} сек";
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                ExecutionTime = string.Empty;
            }
            finally
            {
                IsExecuting = false;
            }
        }

        [RelayCommand]
        private void ClearQuery()
        {
            SqlQuery = string.Empty;
            FormattedResults = string.Empty;
            ResultsInfo = string.Empty;
            ExecutionTime = string.Empty;
            HasResults = false;
            HasError = false;
        }

        [RelayCommand]
        private async Task CopyResultsAsync()
        {
            if (!string.IsNullOrEmpty(FormattedResults))
            {
                await Clipboard.Default.SetTextAsync(FormattedResults);
                await Application.Current.MainPage.DisplayAlert("Успех", "Результаты скопированы", "OK");
            }
        }

         [RelayCommand]
        private async Task SaveAsDatasetAsync()
        {
            if (string.IsNullOrWhiteSpace(SqlQuery))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Нет SQL запроса для сохранения", "OK");
                return;
            }

            // Запрашиваем название набора
            var datasetName = await Application.Current.MainPage.DisplayPromptAsync(
                "Сохранение набора данных",
                "Введите название для набора данных:",
                "Сохранить",
                "Отмена",
                "Новый набор данных",
                -1,
                Keyboard.Text,
                "Новый набор данных");

            if (string.IsNullOrWhiteSpace(datasetName))
                return;

            // Запрашиваем описание (опционально)
            var datasetDescription = await Application.Current.MainPage.DisplayPromptAsync(
                "Описание набора данных",
                "Введите описание (необязательно):",
                "Сохранить",
                "Пропустить",
                "",
                -1,
                Keyboard.Text,
                "");

            try
            {
                // Создаем новый набор данных
                var dataset = new Dataset
                {
                    Name = datasetName.Trim(),
                    Description = datasetDescription?.Trim() ?? string.Empty,
                    SqlQuery = SqlQuery.Trim(),
                    DatabaseId = _database.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var result = await _datasetService.SaveDatasetAsync(dataset);
                
                if (result > 0)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Успех", 
                        $"Набор данных \"{dataset.Name}\" сохранен", 
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Ошибка", 
                        "Не удалось сохранить набор данных", 
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка", 
                    $"Не удалось сохранить набор данных: {ex.Message}", 
                    "OK");
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private bool IsSelectQuery(string query)
        {
            var trimmedQuery = query.TrimStart();
            return trimmedQuery.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                   trimmedQuery.StartsWith("WITH", StringComparison.OrdinalIgnoreCase) ||
                   trimmedQuery.StartsWith("SHOW", StringComparison.OrdinalIgnoreCase) ||
                   trimmedQuery.StartsWith("DESCRIBE", StringComparison.OrdinalIgnoreCase) ||
                   trimmedQuery.StartsWith("EXPLAIN", StringComparison.OrdinalIgnoreCase);
        }

        private async Task ProcessSelectResults(NpgsqlDataReader reader)
        {
            var results = new StringBuilder();
            var rowCount = 0;

            // Получаем названия колонок
            var columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }

            // Форматируем заголовки
            results.AppendLine(string.Join(" | ", columnNames));
            results.AppendLine(new string('-', columnNames.Sum(n => n.Length) + (columnNames.Count - 1) * 3));

            // Читаем данные
            while (await reader.ReadAsync())
            {
                var rowValues = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i)?.ToString() ?? "NULL";
                    rowValues.Add(value);
                }
                results.AppendLine(string.Join(" | ", rowValues));
                rowCount++;
            }

            FormattedResults = results.ToString();
            ResultsInfo = $"Найдено строк: {rowCount}";
            HasResults = true;
        }

        private void FormatNonQueryResults(int affectedRows)
        {
            FormattedResults = $"Запрос выполнен успешно.\nЗатронуто строк: {affectedRows}";
            ResultsInfo = "Результат выполнения";
            HasResults = true;
        }
    }
}