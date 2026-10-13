using System.Data;

public static class Utils
{
    public static void PrintTable(DataTable table)
    {
        foreach (DataRow row in table.Rows)
        {
            if (row.RowState == DataRowState.Deleted)
            {
                Console.WriteLine($"Строка {row["id", DataRowVersion.Original]} удалена");
                continue;
            }

            Console.WriteLine($"{string.Join(" | ", row.ItemArray)}");
        }
    }

    public static void PrintView(DataView view)
    {
        foreach (DataRowView rowView in view)
        {
            Console.WriteLine(string.Join(" | ", rowView.Row.ItemArray));
        }
    }

}