using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Models
{
    public class ActionLog
    {
        [Key]
        public int ActionLogId { get; set; }
        public TimeSpan Duration {  get; set; }
        public string Controller { get; set; }
        public string Action {  get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        

        public ActionLog() { }
    }
}
