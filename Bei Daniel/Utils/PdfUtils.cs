//using PdfSharp.Pdf;
//using MigraDoc.DocumentObjectModel;
//using MigraDoc.Rendering;
//using Bei_Daniel.Models;

//public static class PdfUtils
//{
//    public static void GenerateOrderPdf(string filePath, Order order)
//    {
//        var document = new Document();
//        var section = document.AddSection();

//        // Title
//        var title = section.AddParagraph("Bestellung");
//        title.Format.Font.Size = 16;
//        title.Format.Font.Bold = true;
//        title.Format.SpaceAfter = "1cm";

//        // Table
//        var table = section.AddTable();
//        table.Borders.Width = 0.75;

//        // Columns
//        table.AddColumn("3cm"); // Menge
//        table.AddColumn("6cm"); // Produkt
//        table.AddColumn("3cm"); // Einzelpreis
//        table.AddColumn("3cm"); // Total

//        // Header row
//        var row = table.AddRow();
//        row.Shading.Color = Colors.LightGray;
//        row.Cells[0].AddParagraph("Menge");
//        row.Cells[1].AddParagraph("Produkt");
//        row.Cells[2].AddParagraph("Einzelpreis");
//        row.Cells[3].AddParagraph("Total");

//        // Order row
//        var orderRow = table.AddRow();
//        orderRow.Cells[0].AddParagraph(order.Amount.ToString());
//        orderRow.Cells[1].AddParagraph(order.Product?.Name ?? "");
//        orderRow.Cells[2].AddParagraph($"{order.ProductPrice:0.00} €");
//        orderRow.Cells[3].AddParagraph($"{order.InLineTotal:0.00} €");

//        // Total line
//        section.AddParagraph($"\nGesamt: {order.InLineTotal:0.00} €")
//               .Format.Font.Bold = true;

//        // Render
//        var renderer = new PdfDocumentRenderer(true)
//        {
//            Document = document
//        };
//        renderer.RenderDocument();
//        renderer.PdfDocument.Save(filePath);
//    }
//}
