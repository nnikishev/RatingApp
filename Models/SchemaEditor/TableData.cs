using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace RatingApp.Models
{
    public partial class TableData : ObservableObject
    {
        [ObservableProperty]
        private string tableName;

        [ObservableProperty]
        private List<TableColumn> columns = new();

        [ObservableProperty]
        private List<Dictionary<string, object>> rows = new();

        [ObservableProperty]
        private int totalRows;

        [ObservableProperty]
        private int previewRowsCount;

        [ObservableProperty]
        private bool isLoading;
    }

    public partial class TableColumn : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string dataType;

        [ObservableProperty]
        private int ordinalPosition;

        [ObservableProperty]
        private string filterValue;

        [ObservableProperty]
        private bool isFiltered;
    }
}