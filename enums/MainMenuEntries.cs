using System.ComponentModel.DataAnnotations;

namespace CodingTracker.enums;

internal enum MainMenuEntries
{
    [Display(Name = "Add record")]
    AddRecord,
    
    [Display(Name = "View Records")]
    ViewRecords,
    
    [Display(Name = "Delete Record")]
    DeleteRecord,
    
    [Display(Name = "Update Record")]
    UpdateRecord,
    
    Quit
}