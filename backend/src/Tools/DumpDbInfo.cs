public static void DumpEfModelInfo(DbContext db)
{
    Console.WriteLine("Connection: " + db.Database.GetDbConnection().ConnectionString);
    Console.WriteLine("DB: " + db.Database.GetDbConnection().Database);
    var pt = db.Model.FindEntityType(typeof(Ecom.Domain.Entities.ProductTag));
    Console.WriteLine("ProductTag table: " + pt?.GetTableName());
    foreach (var p in pt?.GetProperties() ?? Enumerable.Empty<Microsoft.EntityFrameworkCore.Metadata.IProperty>())
        Console.WriteLine(p.Name + " -> " + p.GetColumnName());
}