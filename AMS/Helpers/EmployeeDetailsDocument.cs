using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using AMS.Models;
using AMS.Models.ViewModel;
using System;

namespace AMS.Helpers
{
    public class EmployeeDetailsDocument : IDocument
    {
        private readonly EmployeeDetailsViewModel _model;

        public EmployeeDetailsDocument(EmployeeDetailsViewModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Show header only on the first page
                page.Header().ShowOnce().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(60).Image("wwwroot/images/company-logo.png", ImageScaling.FitArea);
                        row.RelativeItem(); // spacer
                    });

                    col.Item().AlignCenter().Text("Employee Attendance Details")
                        .SemiBold().FontSize(20).FontColor(Colors.Black);
                });

                page.Content().Column(col =>
                {
                    col.Spacing(5);

                    // ⬇️ Add some space between the title and the employee details box
                    col.Item().PaddingTop(15).Border(1).Padding(10).Column(empBox =>
                    {
                        empBox.Spacing(5);

                        // Employee Info - Two columns layout
                        empBox.Item().Row(row =>
                        {
                            row.RelativeItem().Text(txt =>
                            {
                                txt.Span("👤 Name: ").SemiBold();
                                txt.Span($"{_model.Employee?.FirstName} {_model.Employee?.LastName}");
                            });

                            row.RelativeItem().Text(txt =>
                            {
                                txt.Span("📧 Email: ").SemiBold();
                                txt.Span($"{_model.Employee?.Email ?? "N/A"}");
                            });
                        });

                        //empBox.Item().Row(row =>
                        //{
                        //    row.RelativeItem().Text(txt =>
                        //    {
                        //        txt.Span("📱 Phone: ").SemiBold();
                        //        txt.Span($"{_model.Employee?.PhoneNumber ?? "N/A"}");
                        //    });

                        //    row.RelativeItem().Text(txt =>
                        //    {
                        //        txt.Span("🧑‍💼 Designation: ").SemiBold();
                        //        txt.Span($"{_model.Employee?.Designation ?? "N/A"}");
                        //    });
                        //});

                        empBox.Item().Row(row =>
                        {
                            row.RelativeItem().Text(txt =>
                            {
                                txt.Span("🏢 Department: ").SemiBold();
                                txt.Span($"{_model.Employee?.Department ?? "N/A"}");
                            });

                            row.RelativeItem().Text(txt =>
                            {
                                txt.Span("🧑‍💼 Designation: ").SemiBold();
                                txt.Span($"{_model.Employee?.Designation ?? "N/A"}");
                            });

                            //row.RelativeItem().Text(""); // Empty to balance layout
                        });
                    });

                    var monthYear = _model.AttendanceRecord?.FirstOrDefault()?.AttendanceDate.ToString("MMMM yyyy") ?? "";
                    col.Item().PaddingTop(10).Text($"📅 Attendance Record: {monthYear}")
                        .Bold().FontSize(12).FontColor(Colors.Black);

                    // Attendance Table
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Date
                            columns.RelativeColumn(2); // Check-in
                            columns.RelativeColumn(2); // Check-out
                            columns.RelativeColumn(2); // Status
                            columns.RelativeColumn(2); // Hours
                        });

                        // Table Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Date").Bold().FontSize(10);
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Check-in").Bold().FontSize(10);
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Check-out").Bold().FontSize(10);
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Status").Bold().FontSize(10);
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Hours").Bold().FontSize(10);
                        });

                        // Table Rows
                        if (_model.AttendanceRecord != null)
                        {
                            foreach (var attendance in _model.AttendanceRecord)
                            {
                                TimeSpan? totalHours = attendance.CheckOutTime.HasValue
                                    ? attendance.CheckOutTime.Value - attendance.CheckInTime
                                    : null;

                                string status = attendance.Status?.ToLower() ?? "unknown";
                                string bgColor = status == "absent" ? Colors.Red.Lighten3 :
                                                 status == "present" ? Colors.Green.Lighten4 :
                                                 Colors.Yellow.Lighten3;

                                table.Cell().Background(bgColor).Padding(2).Text(attendance.AttendanceDate.ToString("dd MMM yyyy")).FontSize(9);
                                table.Cell().Background(bgColor).Padding(2).Text(DateTime.Today.Add(attendance.CheckInTime).ToString("hh:mm tt")).FontSize(9);
                                table.Cell().Background(bgColor).Padding(2).Text(
                                    attendance.CheckOutTime.HasValue
                                        ? DateTime.Today.Add(attendance.CheckOutTime.Value).ToString("hh:mm tt")
                                        : "--").FontSize(9);
                                table.Cell().Background(bgColor).Padding(2).Text(attendance.Status ?? "--").FontSize(9);
                                table.Cell().Background(bgColor).Padding(2).Text(
                                    totalHours.HasValue
                                        ? $"{totalHours.Value.Hours}h {totalHours.Value.Minutes}m"
                                        : "--").FontSize(9);
                            }
                        }
                    });

                    //col.Item().PaddingTop(60).Row(row =>
                    //{
                    //    row.RelativeItem().AlignRight().Text("🖊️ HR Signature: ____________________________");
                    //});
                });


                //page.Footer().AlignCenter().Text(x =>
                //{
                //    x.CurrentPageNumber();
                //    x.Span(" / ");
                //    x.TotalPages();

                //    x.Span("  Generated on: ");
                //    x.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).FontColor(Colors.Grey.Darken1);
                //});

                page.Footer().Column(footer =>
                {
                    footer.Spacing(5);

                    footer.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight().Text("🖊️ HR Signature: ____________________________");
                        //row.RelativeItem().AlignRight().Text(text =>
                        //{
                        //    text.CurrentPageNumber();
                        //    text.Span(" / ");
                        //    text.TotalPages();
                        //});
                    });

                    footer.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Generated on: ");
                        text.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).FontColor(Colors.Grey.Darken1);
                    });
                });
            });
        }
    }
}
