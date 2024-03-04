using System.ComponentModel.DataAnnotations;

namespace CodingTracker.enums;

internal enum ReportTypes
{
    [Display(Name = "Date to Today"), EnumHelpers.Method("DateToToday")]
    DateToToday,
    
    [Display(Name = "Date Range"), EnumHelpers.Method("DateRange")]
    DateRange,
    
    [Display(Name = "Total"), EnumHelpers.Method("Total")]
    Total,
    
    [Display(Name = "Total for Month"), EnumHelpers.Method("TotalForMonth")]
    TotalForMonth,
    
    [Display(Name = "Total for Year"), EnumHelpers.Method("TotalForYear")]
    TotalForYear,
    
    [Display(Name = "Return to Main Menu")]
    BackToMainMenu
}