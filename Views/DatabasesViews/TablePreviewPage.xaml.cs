// Views/TablePreviewPage.xaml.cs
using RatingApp.ViewModels;
using System.Text;

namespace RatingApp.Views
{
    public partial class TablePreviewPage : ContentPage
    {
        private const int CELL_PADDING = 8;
        private const int CELL_MIN_WIDTH = 120;
        private const int HEADER_HEIGHT = 60;
        private const int ROW_HEIGHT = 40;

        public TablePreviewPage(TablePreviewViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            
            viewModel.DataLoaded += OnDataLoaded;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            if (BindingContext is TablePreviewViewModel viewModel)
            {
                viewModel.InitializeData();
            }
        }

        private void OnDataLoaded(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"OnDataLoaded: Event received from ViewModel");
            
            if (BindingContext is TablePreviewViewModel viewModel)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"OnDataLoaded: Regenerating table with {viewModel.TableData?.Rows?.Count} rows");
                    
                    // Полностью очищаем и пересоздаем таблицу
                    TableContainer.Children.Clear();
                    
                    // Добавляем небольшую задержку для гарантии обновления UI
                    Task.Delay(1).ContinueWith(t =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            GenerateTableLayout(viewModel.TableData);
                            System.Diagnostics.Debug.WriteLine("OnDataLoaded: Table regeneration completed");
                        });
                    });
                });
            }
        }


        private void GenerateTableLayout(Models.TableData tableData)
        {
            System.Diagnostics.Debug.WriteLine($"GenerateTableLayout called with {tableData?.Rows?.Count} rows");
            
            TableContainer.Children.Clear();

            if (tableData?.Rows == null || tableData.Rows.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("GenerateTableLayout: No data to display");
                TableContainer.Children.Add(new Label
                {
                    Text = "Нет данных для отображения",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 16,
                    TextColor = Colors.Gray,
                    Margin = new Thickness(20)
                });
                return;
            }

            System.Diagnostics.Debug.WriteLine($"GenerateTableLayout: Creating table with {tableData.Rows.Count} rows and {tableData.Columns.Count} columns");
            CreateScrollableTable(tableData);
        }
        private void CreateScrollableTable(Models.TableData tableData)
        {
            var horizontalContainer = new HorizontalStackLayout
            {
                Spacing = 0
            };

            for (int colIndex = 0; colIndex < tableData.Columns.Count; colIndex++)
            {
                var columnStack = new VerticalStackLayout
                {
                    Spacing = 0,
                    WidthRequest = CELL_MIN_WIDTH
                };

                // Добавляем заголовок колонки
                var headerFrame = CreateHeaderCell(tableData.Columns[colIndex]);
                columnStack.Children.Add(headerFrame);

                // Добавляем ячейки данных
                for (int rowIndex = 0; rowIndex < tableData.Rows.Count; rowIndex++)
                {
                    var row = tableData.Rows[rowIndex];
                    var columnName = tableData.Columns[colIndex].Name;
                    var value = row.ContainsKey(columnName) ? row[columnName]?.ToString() : null;
                    
                    var cellFrame = CreateDataCell(value, rowIndex, colIndex, rowIndex, columnName);
                    columnStack.Children.Add(cellFrame);
                }

                horizontalContainer.Children.Add(columnStack);
            }

            TableContainer.Children.Clear();
            
            var scrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Always,
                VerticalScrollBarVisibility = ScrollBarVisibility.Always,
                Content = horizontalContainer
            };

            TableContainer.Children.Add(scrollView);
        }

        private Frame CreateHeaderCell(Models.TableColumn column)
        {
            var headerFrame = new Frame
            {
                BackgroundColor = column.IsFiltered ? 
                    (Application.Current.RequestedTheme == AppTheme.Dark ? 
                     Color.FromArgb("#2D5A7A") : Color.FromArgb("#B3D9FF")) :
                    (Application.Current.RequestedTheme == AppTheme.Dark ? 
                     Color.FromArgb("#404040") : Color.FromArgb("#D0D0D0")),
                BorderColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
                    Color.FromArgb("#606060") : Color.FromArgb("#B0B0B0"),
                CornerRadius = 0,
                HasShadow = false,
                Padding = new Thickness(CELL_PADDING, 4),
                WidthRequest = CELL_MIN_WIDTH,
                HeightRequest = HEADER_HEIGHT,
                Margin = 0
            };

            var mainLayout = new Grid
            {
                ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnSpacing = 5
            };

            // Иконка фильтра слева
            var filterButton = new Button
            {
                Text = column.IsFiltered ? "🔍✅" : "🔍",
                FontSize = 14,
                BackgroundColor = Colors.Transparent,
                TextColor = column.IsFiltered ? 
                    Color.FromArgb("#007ACC") : 
                    (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.LightGray : Colors.Gray),
                HeightRequest = 30,
                WidthRequest = 30,
                CornerRadius = 15,
                Padding = 0,
                Command = ((TablePreviewViewModel)BindingContext).ShowFilterDialogCommand,
                CommandParameter = column
            };
            Grid.SetColumn(filterButton, 1);
            Grid.SetRow(filterButton, 0);
            Grid.SetRowSpan(filterButton, 2);

            // Текстовая информация справа
            var textLayout = new VerticalStackLayout
            {
                Spacing = 2,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var nameLabel = new Label
            {
                Text = column.Name,
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
                    Colors.White : Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            var typeLabel = new Label
            {
                Text = $"({column.DataType})",
                FontSize = 9,
                TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
                    Colors.LightGray : Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            textLayout.Children.Add(nameLabel);
            textLayout.Children.Add(typeLabel);
            Grid.SetColumn(textLayout, 0);
            Grid.SetRow(textLayout, 0);

            // Значение фильтра (если есть)
            if (!string.IsNullOrEmpty(column.FilterValue))
            {
                var filterLabel = new Label
                {
                    Text = $"= {TruncateFilterValue(column.FilterValue)}",
                    FontSize = 8,
                    TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
                        Colors.LightBlue : Color.FromArgb("#0066CC"),
                    HorizontalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.TailTruncation
                };
                Grid.SetColumn(filterLabel, 0);
                Grid.SetRow(filterLabel, 1);
                mainLayout.Children.Add(filterLabel);
            }

            mainLayout.Children.Add(filterButton);
            mainLayout.Children.Add(textLayout);

            headerFrame.Content = mainLayout;
            return headerFrame;
        }

        private string TruncateFilterValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length > 12 ? value.Substring(0, 12) + "..." : value;
        }

        private Frame CreateDataCell(string value, int rowIndex, int colIndex, int globalRowIndex, string columnName)
        {
            var backgroundColor = GetRowColor(rowIndex);
            
            var cellFrame = new Frame
            {
                BackgroundColor = backgroundColor,
                BorderColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
                    Color.FromArgb("#505050") : Color.FromArgb("#C0C0C0"),
                CornerRadius = 0,
                HasShadow = false,
                Padding = CELL_PADDING,
                WidthRequest = CELL_MIN_WIDTH,
                HeightRequest = ROW_HEIGHT,
                Margin = 0
            };

            var valueLabel = new Label
            {
                Text = value ?? "NULL",
                FontSize = 12,
                TextColor = value == null ? Colors.Gray : 
                           (Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            // Добавляем обработчик клика для показа полного значения
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) => await ShowCellValuePopup(value ?? "NULL", colIndex, globalRowIndex, columnName);
            cellFrame.GestureRecognizers.Add(tapGesture);

            cellFrame.Content = valueLabel;
            return cellFrame;
        }

        private Color GetRowColor(int rowIndex)
        {
            if (rowIndex % 2 == 0)
            {
                return Application.Current.RequestedTheme == AppTheme.Dark ? 
                    Color.FromArgb("#2A2A2A") : Colors.White;
            }
            else
            {
                return Application.Current.RequestedTheme == AppTheme.Dark ? 
                    Color.FromArgb("#333333") : Color.FromArgb("#F8F8F8");
            }
        }


        private async Task ShowCellValuePopup(string value, int columnIndex, int rowIndex, string columnName)
{
    if (string.IsNullOrEmpty(value) || value == "NULL")
        return;

    var viewModel = BindingContext as TablePreviewViewModel;
    if (viewModel == null) return;

    // Создаем простую страницу для просмотра
    var viewPage = new ContentPage
    {
        Title = $"{columnName} [Строка {rowIndex + 1}]",
        BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
            Color.FromArgb("#1A1A1A") : Colors.White
    };

    var layout = new Grid
    {
        RowDefinitions = 
        {
            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
            new RowDefinition { Height = GridLength.Auto }
        },
        RowSpacing = 15,
        Padding = new Thickness(20, 10)
    };

    // Область с текстом и прокруткой
    var textFrame = new Frame
    {
        BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
            Color.FromArgb("#2A2A2A") : Color.FromArgb("#F5F5F5"),
        BorderColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
            Color.FromArgb("#404040") : Color.FromArgb("#DDDDDD"),
        CornerRadius = 8,
        Padding = 15,
        HasShadow = true
    };

    var scrollView = new ScrollView();
    var valueLabel = new Label
    {
        Text = value,
        FontSize = 14,
        FontFamily = "Courier New",
        TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black,
        LineBreakMode = LineBreakMode.WordWrap
    };
    
    scrollView.Content = valueLabel;
    textFrame.Content = scrollView;
    Grid.SetRow(textFrame, 0);

    // Кнопки внизу
    var buttonsLayout = new Grid
    {
        ColumnDefinitions = 
        {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
        },
        ColumnSpacing = 10
    };

    var copyButton = new Button
    {
        Text = "📋 Копировать",
        BackgroundColor = Color.FromArgb("#007ACC"),
        TextColor = Colors.White,
        CornerRadius = 8,
        HeightRequest = 50,
        FontSize = 14
    };

    var closeButton = new Button
    {
        Text = "Закрыть",
        BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
            Color.FromArgb("#404040") : Color.FromArgb("#E0E0E0"),
        TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black,
        CornerRadius = 8,
        HeightRequest = 50,
        FontSize = 14
    };

    copyButton.Clicked += async (s, e) => 
    {
        await viewModel.CopyToClipboardAsync(value);
        await DisplayAlert("Успех", "Текст скопирован в буфер обмена", "OK");
    };

    closeButton.Clicked += async (s, e) => await Navigation.PopModalAsync();

    Grid.SetColumn(copyButton, 0);
    Grid.SetColumn(closeButton, 1);
    buttonsLayout.Children.Add(copyButton);
    buttonsLayout.Children.Add(closeButton);
    Grid.SetRow(buttonsLayout, 1);

    layout.Children.Add(textFrame);
    layout.Children.Add(buttonsLayout);

    viewPage.Content = layout;

    // Используем NavigationPage для правильного отображения
    var navPage = new NavigationPage(viewPage)
    {
        BarBackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
            Color.FromArgb("#2A2A2A") : Color.FromArgb("#F0F0F0"),
        BarTextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
    };

    await Navigation.PushModalAsync(navPage);
}
        // private async Task ShowCellValuePopup(string value, int columnIndex, int rowIndex, string columnName)
        // {
        //     if (string.IsNullOrEmpty(value) || value == "NULL")
        //         return;

        //     // Создаем модальную страницу с прозрачным фоном
        //     var popupPage = new ContentPage
        //     {
        //         BackgroundColor = Color.FromArgb("#80000000"), // Полупрозрачный черный фон
        //         Padding = new Thickness(20, 60, 20, 60) // Отступы от краев экрана
        //     };

        //     // Основной контейнер popup
        //     var popupContainer = new Frame
        //     {
        //         BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
        //             Color.FromArgb("#2A2A2A") : Colors.White,
        //         BorderColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
        //             Color.FromArgb("#404040") : Color.FromArgb("#CCCCCC"),
        //         CornerRadius = 12,
        //         HasShadow = true,
        //         Padding = 0,
        //         VerticalOptions = LayoutOptions.Center,
        //         HorizontalOptions = LayoutOptions.Center,
        //         WidthRequest = 320,
        //         MaximumHeightRequest = 500
        //     };

        //     var mainLayout = new Grid
        //     {
        //         RowDefinitions = 
        //         {
        //             new RowDefinition { Height = GridLength.Auto },
        //             new RowDefinition { Height = GridLength.Auto },
        //             new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
        //             new RowDefinition { Height = GridLength.Auto }
        //         },
        //         RowSpacing = 0
        //     };

        //     // Заголовок
        //     var headerFrame = new Frame
        //     {
        //         BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
        //             Color.FromArgb("#404040") : Color.FromArgb("#F0F0F0"),
        //         CornerRadius = 0,
        //         HasShadow = false,
        //         Padding = new Thickness(15, 10),
        //         Margin = 0
        //     };

        //     var headerLabel = new Label
        //     {
        //         Text = $"{columnName} [Строка {rowIndex + 1}]",
        //         FontSize = 14,
        //         FontAttributes = FontAttributes.Bold,
        //         TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
        //     };

        //     headerFrame.Content = headerLabel;
        //     Grid.SetRow(headerFrame, 0);

        //     // Кнопка копирования
        //     var copyButton = new Button
        //     {
        //         Text = "📋 Копировать",
        //         BackgroundColor = Colors.Transparent,
        //         TextColor = Color.FromArgb("#007ACC"),
        //         FontSize = 13,
        //         FontAttributes = FontAttributes.Bold,
        //         HeightRequest = 40,
        //         CornerRadius = 0,
        //         BorderColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
        //             Color.FromArgb("#404040") : Color.FromArgb("#E0E0E0"),
        //         BorderWidth = 1,
        //         Margin = 0
        //     };
            
        //     copyButton.Clicked += async (s, e) =>
        //     {
        //         if (BindingContext is TablePreviewViewModel viewModel)
        //         {
        //             await viewModel.CopyToClipboardAsync(value);
        //         }
        //     };
        //     Grid.SetRow(copyButton, 1);

        //     // Область с текстом
        //     var textFrame = new Frame
        //     {
        //         BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
        //             Color.FromArgb("#1A1A1A") : Color.FromArgb("#F8F8F8"),
        //         CornerRadius = 0,
        //         HasShadow = false,
        //         Padding = new Thickness(15, 15),
        //         Margin = 0
        //     };

        //     var scrollView = new ScrollView
        //     {
        //         MaximumHeightRequest = 300
        //     };

        //     var valueLabel = new Label
        //     {
        //         Text = value,
        //         FontSize = 13,
        //         FontFamily = "Courier New",
        //         TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black,
        //         LineBreakMode = LineBreakMode.WordWrap
        //     };

        //     scrollView.Content = valueLabel;
        //     textFrame.Content = scrollView;
        //     Grid.SetRow(textFrame, 2);

        //     // Футер с кнопкой закрытия
        //     var footerFrame = new Frame
        //     {
        //         BackgroundColor = Colors.Transparent,
        //         CornerRadius = 0,
        //         HasShadow = false,
        //         Padding = new Thickness(15, 10),
        //         Margin = 0
        //     };

        //     var closeButton = new Button
        //     {
        //         Text = "Закрыть",
        //         BackgroundColor = Application.Current.RequestedTheme == AppTheme.Dark ? 
        //             Color.FromArgb("#404040") : Color.FromArgb("#E0E0E0"),
        //         TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black,
        //         CornerRadius = 8,
        //         HeightRequest = 40,
        //         FontSize = 14
        //     };
            
        //     closeButton.Clicked += async (s, e) => await Navigation.PopModalAsync();
        //     footerFrame.Content = closeButton;
        //     Grid.SetRow(footerFrame, 3);

        //     // Добавляем все в основной layout
        //     mainLayout.Children.Add(headerFrame);
        //     mainLayout.Children.Add(copyButton);
        //     mainLayout.Children.Add(textFrame);
        //     mainLayout.Children.Add(footerFrame);

        //     popupContainer.Content = mainLayout;

        //     // Добавляем обработчик клика по прозрачной области для закрытия
        //     var tapGesture = new TapGestureRecognizer();
        //     tapGesture.Tapped += async (s, e) => await Navigation.PopModalAsync();
        //     popupPage.GestureRecognizers.Add(tapGesture);

        //     // Центрируем контейнер popup
        //     var centeredLayout = new Grid
        //     {
        //         VerticalOptions = LayoutOptions.Center,
        //         HorizontalOptions = LayoutOptions.Center,
        //         Children = { popupContainer }
        //     };

        //     popupPage.Content = centeredLayout;

        //     // Показываем как модальное окно
        //     await Navigation.PushModalAsync(popupPage, false);
        // }

        protected override void OnDisappearing()
        {
            if (BindingContext is TablePreviewViewModel viewModel)
            {
                viewModel.DataLoaded -= OnDataLoaded;
            }
            base.OnDisappearing();
        }
    }
}