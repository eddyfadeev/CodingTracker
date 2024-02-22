using System.ComponentModel.DataAnnotations;

namespace CodingTracker.enums;

internal enum MainMenuEntries
{
    [Display(Name = "Add record"), EnumHelpers.Method("AddRecord")]
    AddRecord,
    
    [Display(Name = "View Records"), EnumHelpers.Method("ViewRecords")]
    ViewRecords,
    
    [Display(Name = "Delete Record"), EnumHelpers.Method("DeleteRecord")]
    DeleteRecord,
    
    [Display(Name = "Update Record"), EnumHelpers.Method("UpdateRecord")]
    UpdateRecord,
    
    [Display(Name = "Create Report"), EnumHelpers.Method("CreateReport")]
    CreateReport,
    
    Quit
}