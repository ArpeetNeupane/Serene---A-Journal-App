using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Serene.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using PdfColors = QuestPDF.Helpers.Colors;

namespace Serene.Services;

public interface IExportService
{
    Task<byte[]> GeneratePdfExportAsync(DateTime start, DateTime end);
}


/// <summary>
/// Generates a PDF export of journal entries within a specified date range.
/// </summary>
/// <param name="start">The start date of the export range.</param>
/// <param name="end">The end date of the export range.</param>
/// <returns>
/// A byte array representing the generated PDF file, or null if no entries exist
/// within the given date range.
/// </returns>
/// <remarks>
/// This method retrieves journal entries from the database, removes any HTML
/// formatting from the content, and formats the entries into a structured,
/// readable PDF document using QuestPDF.
/// </remarks>
public class ExportService : IExportService
{
    private readonly AppDbContext _context;

    public ExportService(AppDbContext context) => _context = context;

    public async Task<byte[]> GeneratePdfExportAsync(DateTime start, DateTime end)
    {
        var entries = await _context.JournalEntries
            .Where(e => e.EntryDate >= start.Date && e.EntryDate <= end.Date)
            .OrderBy(e => e.EntryDate)
            .ToListAsync();

        if (!entries.Any()) return null;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("Serene Journal Export").FontSize(24).Bold();

                page.Content().Column(col =>
                {
                    col.Spacing(20);
                    foreach (var entry in entries)
                    {
                        col.Item().Column(entryCol =>
                        {
                            entryCol.Item().PaddingTop(25).PaddingBottom(5).Text(string.IsNullOrWhiteSpace(entry.Title) ? "Untitled Entry" : entry.Title).FontSize(16).Bold();
                            entryCol.Item().PaddingBottom(5).Text(entry.EntryDate.ToString("D")).FontSize(10).Italic();

                            //stripping HTML safely
                            var html = entry.ContentHtml ?? "";
                            var plainText = Regex.Replace(html, "<.*?>", string.Empty);

                            entryCol.Item().PaddingTop(10).Text(string.IsNullOrWhiteSpace(plainText) ? "[Empty Entry]" : plainText);

                            if (!string.IsNullOrEmpty(entry.Tags))
                                entryCol.Item().PaddingTop(10).Text($"Tags: {entry.Tags}").FontSize(9).FontColor(PdfColors.Grey.Medium);
                        });
                    }
                });
            });
        });

        return document.GeneratePdf();
    }
}