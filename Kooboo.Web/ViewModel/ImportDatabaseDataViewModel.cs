namespace Kooboo.Web.ViewModel;

internal class ImportDatabaseDataViewModel
{
    public bool OverwriteExisting { get; set; }

    public List<ImportFieldItem> Fields { get; set; }

    public List<Dictionary<string, string>> Records { get; set; }
}

internal class ImportFieldItem
{
    public string DbFieldName { get; set; }

    public string CsvFieldName { get; set; }

    public bool Unique { get; set; }

    public bool Required { get; set; }
}

