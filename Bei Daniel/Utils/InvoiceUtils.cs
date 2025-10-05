
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace Bei_Daniel.Utils
{
    class InvoiceUtils
    {


        public class Product
        {
            public string Quantity { get; set; }
            public string ProductName { get; set; }
            public double UnitPrice { get; set; }
            public double Total { get; set; }
        }

        public class ReceiptDocument : IDocument
        {

            public string CompanyName { get; set; } = "EINZELUNT. D. IVANOV. ";
            public string CompanyAddress { get; set; } = "Wien,21. BRÜNNERSTR. 47-48";
            public string CompanyPhone { get; set; } = "+436643842241";
            public string CustomerName { get; set; } = "testpf";
            public string CompanyNumber { get; set; } = "ATU 67897537";
            public string DeliveryDate {  get; set; }
            public string InvoiceNr { get; set; }
            public List<Product> Products { get; set; }
            public double Netto { get; set; }
            public double MwSt => Math.Round(Netto * 0.10, 2);
            public double Brutto => Netto + MwSt;
            public string LogoPath { get; set; } 

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeTable);
                    page.Footer().Element(ComposeFooter);
                });
            }

            void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    // Logo
                    if (!string.IsNullOrEmpty(LogoPath) && File.Exists(LogoPath))
                    {
                        row.RelativeColumn(1).Height(50).Image(LogoPath, ImageScaling.FitArea);
                    }
                    else
                    {
                        row.RelativeColumn(1).Text("LOGO").Bold().FontSize(18);
                    }

                    // Company + date
                    row.RelativeColumn(2).Column(col =>
                    {
                        col.Item().AlignRight().Text(CompanyName).Bold().FontSize(14);
                        col.Item().AlignRight().Text(CompanyAddress).FontSize(10);
                        col.Item().AlignRight().Text($"Datum: {DateTime.Now:dd.MM.yyyy}").FontSize(10);
                    });
                });
            }

            void ComposeTable(IContainer container)
            {
                container.PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Empfänger: {CustomerName}").Bold().FontSize(12);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // Anzahl
                            columns.RelativeColumn(4); // Produkt
                            columns.RelativeColumn(2); // Einzelpreis
                            columns.RelativeColumn(2); // Total
                        });

                        // Header row
                        table.Header(header =>
                        {
                            header.Cell().Text("Anzahl").Bold();
                            header.Cell().Text("Produkt").Bold();
                            header.Cell().Text("Einzelpreis").Bold();
                            header.Cell().Text("Total").Bold();
                        });

                        // Data rows
                        foreach (var item in Products)
                        {
                            table.Cell().Text(item.Quantity);
                            table.Cell().Text(item.ProductName);
                            table.Cell().Text($"{item.UnitPrice:F2}");
                            table.Cell().Text($"{item.Total:F2}");
                        }
                    });
                });
            }

            void ComposeFooter(IContainer container)
            {
                container.PaddingTop(10).Column(col =>
                {
                    col.Item().AlignRight().Text($"Betrag NETTO: {Netto:F2}").Bold();
                    col.Item().AlignRight().Text($"10% MwSt: {MwSt:F2}");
                    col.Item().AlignRight().Text($"Endbetrag BRUTTO: {Brutto:F2}").Bold();

                    col.Item().PaddingTop(10)
                        .Background(Colors.Green.Lighten2)
                        .AlignCenter()
                        .Text("Vielen Dank für Ihre Bestellung!")
                        .Bold()
                        .FontSize(12);
                });
            }
        }
    }
}
