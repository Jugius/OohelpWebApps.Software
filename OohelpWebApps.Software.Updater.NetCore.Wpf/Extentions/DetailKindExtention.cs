using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Extentions;
internal static class DetailKindExtention
{
    public static string ToValueString(this DetailKind kind) => kind switch
    {
        DetailKind.Changed => "Изменения:",
        DetailKind.Fixed => "Исправления:",
        DetailKind.Updated => "Обновления:",
        DetailKind.Implemented => "Новое:",
        _ => kind.ToString()
    };
}
