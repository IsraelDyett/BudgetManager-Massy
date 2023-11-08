using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Models
{
    public class CsvFileUploadModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [Display(Name = "CSV File")]
        public IFormFile CsvFile { get; set; }
    }

}
