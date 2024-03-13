using System.ComponentModel.DataAnnotations;
using static CodingTracker.enums.EnumHelpers;

namespace CodingTracker.enums;

internal enum ReportTypes
{
    [Display(Name = "Date to Today"), Method("DateToToday")]
    DateToToday,
    
    [Display(Name = "Date Range"), Method("DateRange")]
    DateRange,
    
    [Display(Name = "Total"), Method("Total")]
    Total,
    
    [Display(Name = "Total for Month"), Method("TotalForMonth")]
    TotalForMonth,
    
    [Display(Name = "Total for Year"), Method("TotalForYear")]
    TotalForYear,
    
    [Display(Name = "Return to Main Menu")]
    BackToMainMenu
}