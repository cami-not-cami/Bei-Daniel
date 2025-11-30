using Bei_Daniel.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Bei_Daniel.Utils
{
    class InvoiceUtils
    {


        public class Product
        {
            public string Quantity { get; set; }

            public string ProductQuantityType { get; set; }
            public string ProductName { get; set; }
            public double UnitPrice { get; set; }
            public double Total { get; set; }


        }

        public class ReceiptDocument : IDocument
        {

            public string CompanyName { get; set; } = "EINZELUNT. D. IVANOV. ";
            public string CompanyAddress { get; set; } = "Wien,21. BRÜNNERSTR. 47-48";
            public string CompanyPhone { get; set; } = "+436643842241";
            public string CustomerAddress { get; set; }
            public string CustomerName { get; set; }
            public string CompanyNumber { get; set; } = "ATU 67897537";
            public string Iban = "AT61 1500 0042 1108 4845";
            public string DeliveryDate { get; set; }
            public string InvoiceNr { get; set; }
            public List<Product> Products { get; set; }
            public double Netto { get; set; }
            public double MwSt => Math.Round(Netto * 0.10, 2);
            public double Brutto => Netto + MwSt;
            public string LogoPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "Helpers", "Icons", "BeiDanielLogo.png");

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    // Header only on the first page
                    page.Header().ShowOnce().Element(ComposeHeader);

                    page.Content().Element(container =>
                    {
                        container.Column(col =>
                        {
                            col.Item().Element(ComposeBody);
                            col.Item().PaddingTop(25).Element(ComposeFooter);
                        });
                    });
                });
            }
            void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    // Left: Logo and Client Info
                    row.RelativeColumn(2).Column(col =>
                    {
                        if (!string.IsNullOrEmpty(LogoPath) && File.Exists(LogoPath))
                            col.Item().Height(200).Image(LogoPath, ImageScaling.FitArea);
                        else
                            col.Item().Text(CompanyName).Bold().FontSize(18);


                    });

                    // Right: Company Info and Invoice Data
                    row.RelativeColumn(2).AlignRight().Column(col =>
                    {
                        // Invoice title
                        col.Item().Text("RECHNUNG").Bold().FontSize(18);

                        // Company info (spaced for readability)
                        col.Item().PaddingTop(5).Text(CompanyName).Bold().FontSize(12);
                        col.Item().PaddingTop(2).Text(CompanyAddress).FontSize(10);
                        col.Item().PaddingTop(2).Text($"IBAN: {Iban}").FontSize(10);  // <-- Add this line
                        col.Item().PaddingTop(2).Text("daniel.ivanov2707@gmail.com").FontSize(10).FontColor(Colors.Grey.Darken1);

                        // Invoice details table (add vertical spacing)
                        col.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Cell().Text("Rechnungsnummer").Bold();
                            table.Cell().AlignRight().Text(InvoiceNr);

                            table.Cell().Text("Lieferdatum").Bold();
                            table.Cell().AlignRight().Text(DeliveryDate);
                        });
                    });
                });
            }

            void ComposeBody(IContainer container)
            {
                container.PaddingTop(40).Column(col =>
                {
                    // === Customer Header ===
                    col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10).Column(customer =>
                    {
                        customer.Item().Text("Kunde / Empfänger")
                            .Bold()
                            .FontSize(15)
                            .FontColor(Colors.Grey.Darken3);

                        customer.Item().PaddingTop(3).Text(CustomerName)
                            .FontSize(14)
                            .FontColor(Colors.Black);

                        customer.Item().PaddingTop(1).Text(CustomerAddress)
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken3);
                    });

                    // Add a little spacing after the customer section
                    col.Item().PaddingTop(15);

                    // === Product Table ===
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4); // Beschreibung
                            columns.RelativeColumn(2); // Menge
                            columns.RelativeColumn(2); // Preis
                            columns.RelativeColumn(2); // Betrag
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Text("BESCHREIBUNG").Bold();
                            header.Cell().Text("MENGE").Bold();
                            header.Cell().Text("PREIS (€)").Bold();
                            header.Cell().Text("BETRAG (€)").Bold();
                            header.Cell().PaddingBottom(5)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten2);
                        });

                        // Rows
                        foreach (var item in Products)
                        {
                            table.Cell().PaddingVertical(4)
                                .BorderBottom(0.5f)
                                .BorderColor(Colors.Grey.Lighten3)
                                .Text(item.ProductName);

                            table.Cell().PaddingVertical(4)
                                .BorderBottom(0.5f)
                                .BorderColor(Colors.Grey.Lighten3)
                                .Text(item.Quantity);

                            table.Cell().PaddingVertical(4)
                                .BorderBottom(0.5f)
                                .BorderColor(Colors.Grey.Lighten3)
                                .Text($"{item.UnitPrice:F2}");

                            table.Cell().PaddingVertical(4)
                                .BorderBottom(0.5f)
                                .BorderColor(Colors.Grey.Lighten3)
                                .Text($"{item.Total:F2}");
                        }
                    });

                    // Add spacing before summary
                    col.Item().PaddingTop(25);

                    // === Summary ===
                    col.Item().AlignRight().Column(summary =>
                    {
                        summary.Item().PaddingBottom(3).Row(row =>
                        {
                            row.RelativeColumn(2).AlignRight().Text("Betrag NETTO")
                                .Bold().FontSize(11).FontColor(Colors.Grey.Darken3);
                            row.RelativeColumn(1).AlignRight().Text($"{Netto:F2} €")
                                .FontSize(11).FontColor(Colors.Grey.Darken3);
                        });

                        summary.Item().PaddingBottom(3).Row(row =>
                        {
                            row.RelativeColumn(2).AlignRight().Text("10% MwSt")
                                .Bold().FontSize(11).FontColor(Colors.Grey.Darken3);
                            row.RelativeColumn(1).AlignRight().Text($"{MwSt:F2} €")
                                .FontSize(11).FontColor(Colors.Grey.Darken3);
                        });

                        summary.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeColumn(2).AlignRight().Text("ZU BEZAHLEN")
                                .Bold().FontSize(13).FontColor(Colors.Blue.Medium);
                            row.RelativeColumn(1).AlignRight().Text($"{Brutto:F2} €")
                                .Bold().FontSize(13).FontColor(Colors.Blue.Medium);
                        });
                    });
                });
            }




            void ComposeFooter(IContainer container)
            {
                container.PaddingTop(20).Column(col =>
                {

                    col.Item().PaddingTop(15)
                        .Background(Colors.Green.Lighten2)
                        .Padding(10)
                        .AlignCenter()
                        .Text("Vielen Dank für Ihre Bestellung!")
                        .Bold()
                        .FontSize(14);
                });
            }
        }

        public static string GetInvoiceNumber()
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            var invoiceFile = Path.Combine(folder, "invoice_counter.txt");
            string invoiceNr = "1";
            if (File.Exists(invoiceFile))
            {
                invoiceNr = File.ReadAllText(invoiceFile).Trim();
            }
            return invoiceNr;

        }

        public static void IncrementInvoiceNumber()
        {
            string currentInvoiceNr = GetInvoiceNumber();
            if (int.TryParse(currentInvoiceNr, out int invoiceNumber))
            {
                invoiceNumber++;
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
                var invoiceFile = Path.Combine(folder, "invoice_counter.txt");
                File.WriteAllText(invoiceFile, invoiceNumber.ToString());

            }
        }

        internal static void GenerateMonthlyInvoicePdf(int restaurantId, DateOnly startDate, DateOnly endDate, Models.AppDbContext appDbContext)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Restaurant restaurant = RestaurantUtils.GetRestaurantById(restaurantId, appDbContext);
            var orders = OrderUtils.GetOrders(restaurantId, startDate, endDate);

            // Bundle identical products
            orders = ProductUtils.BundleIdenticalProducts(orders);

            // Build product list
            List<InvoiceUtils.Product> products = new List<InvoiceUtils.Product>();
            foreach (var order in orders)
            {
                var product = new InvoiceUtils.Product
                {
                    ProductName = order.Product?.Name ?? string.Empty,
                    ProductQuantityType = order.ProductQuantityType, // keep type separately if needed later
                    Quantity = $"{order.Amount} {order.ProductQuantityType}", // correct usage
                    UnitPrice = order.ProductPrice,
                    Total = order.ProductPrice * order.Amount
                };
                products.Add(product);
            }

            // Apply sorting (store the result)
            products = products.OrderBy(p => p.ProductName).ToList();

            var filePath = RestaurantUtils.generateRestaurantFilePath(restaurantId, startDate, endDate, appDbContext);
            var invoiceNumber = InvoiceUtils.GetInvoiceNumber();
            var document = new ReceiptDocument
            {
                CustomerAddress = restaurant.Address,
                CustomerName = restaurant.Name,
                InvoiceNr = invoiceNumber,
                Products = products,
                Netto = OrderUtils.GetOrderTotal(restaurantId, startDate, endDate),
                DeliveryDate = $"{startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}"
            };
            document.GeneratePdf(filePath);
            MessageBox.Show($"Receipt saved to: {filePath}");
        }

    }
}
