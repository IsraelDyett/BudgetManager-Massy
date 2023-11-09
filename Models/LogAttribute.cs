using BudgetManager.Data;
using BudgetManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BudgetManager.Models
{
    public class LogAttribute : ActionFilterAttribute
    {
        private DateTime startTime;
        private readonly ApplicationDbContext _context;

        public LogAttribute(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            startTime = DateTime.Now;
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            TimeSpan duration = DateTime.Now - startTime;
            string controller = (string)context.RouteData.Values["controller"];
            string action = (string)context.RouteData.Values["action"];
            DateTime createdAt = DateTime.Now;
            // Access the user id from the HttpContext
            string userId = context.HttpContext.User.Identity.Name;

            

            // Create an ActionLog instance and save it to the database
            var actionLog = new ActionLog
            {
                UserId = userId,
                Controller = controller,
                Action = action,
                CreatedAt = createdAt,
                Duration = duration,
                
            };

           
            try
            { 
                _context.ActionLogs.Add(actionLog);
                 _context.SaveChangesAsync();
            }
            catch
            {
               
            }
        }
    }
}