using SimplePayrollApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Threading.Tasks;
// Explicitly use QuestPDF Colors and IContainer, not MAUI versions
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace SimplePayrollApp.Services
{
    public class PdfService : IPdfService
    {
        public async Task<string> GeneratePayrollPdfAsync(PayrollData payrollData)
        {
            // Create filename and path
            string fileName = $"Payroll_{payrollData.EmployeeName}_{DateTime.Now:yyyyMMdd}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Generate PDF on a background thread to keep UI responsive
            return await Task.Run(() =>
            {
                // Create the document
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(50);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Element(ComposeHeader);

                        page.Content().Element(content =>
                        {
                            ComposeContent(content, payrollData);
                        });

                        page.Footer().Element(ComposeFooter);
                    });
                })
                .GeneratePdf(filePath);

                return filePath;
            });

            // Helper method to compose the header
            void ComposeHeader(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("COMPANY NAME")
                            .FontSize(20)
                            .SemiBold()
                            .FontColor(Colors.Green.Medium);

                        column.Item().Text("Payroll Statement")
                            .FontSize(16)
                            .FontColor(Colors.Black);

                        column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });
                });
            }

            // Helper method to compose the content
            void ComposeContent(IContainer container, PayrollData data)
            {
                container.PaddingTop(10).Column(column =>
                {
                    // Employee information
                    column.Item().Text($"Employee: {data.EmployeeName}").SemiBold();
                    column.Item().Text($"ID: {data.EmployeeID}");
                    column.Item().Text($"Pay Period: {data.PayPeriod:MMMM yyyy}");

                    column.Item().PaddingTop(10);

                    // Earnings section
                    column.Item().Text("EARNINGS").FontSize(14).SemiBold().FontColor(Colors.Green.Medium);
                    column.Item().PaddingTop(5);

                    column.Item().Table(table =>
                    {
                        // Define columns
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });

                        // Headers
                        table.Header(header =>
                        {
                            header.Cell().Text("Description");
                            header.Cell().AlignRight().Text("Amount");

                            header.Cell().ColumnSpan(2).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        });

                        // Basic Salary
                        table.Cell().Text("Basic Salary");
                        table.Cell().AlignRight().Text($"${data.BasicSalary:N2}");

                        // Allowances
                        table.Cell().Text("Allowances");
                        table.Cell().AlignRight().Text($"${data.Allowances:N2}");

                        // Bonus
                        table.Cell().Text("Bonus");
                        table.Cell().AlignRight().Text($"${data.Bonus:N2}");

                        // Overtime
                        table.Cell().Text("Overtime");
                        table.Cell().AlignRight().Text($"${data.Overtime:N2}");

                        // Divider
                        table.Cell().ColumnSpan(2).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

                        // Gross Salary
                        table.Cell().Text("Gross Salary").SemiBold();
                        table.Cell().AlignRight().Text($"${data.GrossSalary:N2}").SemiBold().FontColor(Colors.Green.Medium);
                    });

                    column.Item().PaddingTop(20);

                    // Deductions section
                    column.Item().Text("DEDUCTIONS").FontSize(14).SemiBold().FontColor(Colors.Red.Medium);
                    column.Item().PaddingTop(5);

                    column.Item().Table(table =>
                    {
                        // Define columns
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });

                        // Headers
                        table.Header(header =>
                        {
                            header.Cell().Text("Description");
                            header.Cell().AlignRight().Text("Amount");

                            header.Cell().ColumnSpan(2).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        });

                        // SSF
                        table.Cell().Text("SSF Contribution (5.5%)");
                        table.Cell().AlignRight().Text($"${data.SSF:N2}");

                        // PAYE
                        table.Cell().Text("PAYE Tax");
                        table.Cell().AlignRight().Text($"${data.PAYE:N2}");

                        // Divider
                        table.Cell().ColumnSpan(2).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

                        // Total Deductions
                        table.Cell().Text("Total Deductions").SemiBold();
                        table.Cell().AlignRight().Text($"${data.SSF + data.PAYE:N2}").SemiBold();
                    });

                    column.Item().PaddingTop(20);

                    // Net Salary
                    column.Item().Background(Colors.Grey.Lighten3).Padding(10).Row(row =>
                    {
                        row.RelativeItem().Text("NET SALARY").SemiBold().FontSize(14);
                        row.RelativeItem().AlignRight().Text($"${data.NetSalary:N2}")
                            .SemiBold()
                            .FontSize(14)
                            .FontColor(Colors.Green.Medium);
                    });
                });
            }

            // Helper method to compose the footer
            void ComposeFooter(IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        column.Item().Text($"Generated on {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium);
                    });
                });
            }
        }
    }
}
