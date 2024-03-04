using System.Globalization;
using CodingTracker.models;
using Spectre.Console;

namespace CodingTracker.views;

public class SummaryConstructor
{
    internal Table ShowRecords(IEnumerable<CodingSession> sessions)
    {
        Table table = new()
        {
            Title = new TableTitle("Coding Sessions"),
            Border = TableBorder.Rounded
        };
        table.AddColumn("Id");
        table.AddColumn("Start Date");
        table.AddColumn("End Date");
        table.AddColumn("Duration (hh:mm)");

        foreach (var session in sessions)
        {
            table.AddRow(
                session.Id.ToString(),
                session.StartTime.ToString(CultureInfo.InvariantCulture), 
                session.EndTime.ToString(CultureInfo.InvariantCulture),
                session.Duration.ToString()
                );
        }

        return table;
    }
}