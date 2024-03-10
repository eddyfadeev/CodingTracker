using System.Globalization;
using System.Text;
using CodingTracker.models;
using Microsoft.Extensions.Primitives;
using Spectre.Console;

namespace CodingTracker.views;

public class SummaryConstructor
{
    internal Table SummaryTable = new();
    private TimeSpan _totalDuration;
    
    internal void PopulateWithRecords(IEnumerable<CodingSession> sessions)
    {
        Table table = new()
        {
            Title = new TableTitle("Coding Sessions", new Style(Color.Grey100)),
            Border = TableBorder.Rounded,
            BorderStyle = new Style(Color.SpringGreen3),
        };
        table.AddColumn("Id");
        table.AddColumn("Start Date");
        table.AddColumn("End Date");
        table.AddColumn("Duration (hh:mm)");
        
        int counter = 1;
        foreach (var session in sessions)
        {
            var color = counter % 5 == 0 ? "blue" : "grey100";

            if (counter == 5)
            {
                counter = 0;
            }

            table.AddRow(
                new Markup($"[{color}]{session.Id}[/]"),
                new Markup($"[{color}]{session.StartTime}[/]"),
                new Markup($"[{color}]{session.EndTime}[/]"),
                new Markup($"[{color}]{session.Duration}[/]")
                );
            counter++;

            _totalDuration += session.Duration;
        }

        table.Caption = new TableTitle(
            "Total Duration: " + FormatDuration(),
            new Style(Color.Green)
            );
        
        SummaryTable = table;
    }
    
    private string FormatDuration()
    {
        var stringBuilder = new StringBuilder();
        
        string FormatUnit(string unit, int value) => value == 1 ? unit : unit + "s";

        return stringBuilder
            .Append((_totalDuration.Days > 0 ? _totalDuration.Days + " " + FormatUnit("day", _totalDuration.Days) + ", " : ""))
            .Append(_totalDuration.Hours + " " + FormatUnit("hour", _totalDuration.Hours) + ", ")
            .Append(_totalDuration.Minutes + " " + FormatUnit("minute", _totalDuration.Minutes))
            .ToString();
    }
}