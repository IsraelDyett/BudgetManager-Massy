using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Models
{
    public class ActionLog
    {
        [Key]
        public int ActionLogId { get; set; }
        public string User { get; set; }
        public string Controller { get; set; }
        public string? ActionType { get; set; }
        public string? Action {  get; set; }
        public string? DoneOn { get; set; }
        public string? Field { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan Duration {  get; set; }
        
        

        public ActionLog() { }
    }
}
