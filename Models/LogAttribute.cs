using BudgetManager.Data;
using BudgetManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            TimeSpan duration = DateTime.Now - startTime;
            string controller = (string)context.RouteData.Values["controller"];
            string actionTemp = (string)context.RouteData.Values["action"];
            string action = "";

            if (actionTemp != null)
            {
                switch (actionTemp.ToLower())
                {
                    case "index":
                        action = "Viewed Budget Page";
                        break;
                    case "create":
                        action = "Created Budget";
                        break;
                    case "edit":
                        action = "Edited Budget";
                        break;
                    case "details":
                        action = "Viewed Budget Details";
                        break;
                    case "delete":
                        action = "Deleted Budget ";
                        break;
                    case "savetodatabase":
                        action = "Save Multiple Budgets to the database";
                        break;
                    case "suggestbudgetchanges":
                        action = "Suggested a changes  to the Budget";
                        break;
                    case "acceptbudget":
                        action = "Approved the version of this Budget ";
                        break;
                    case "getacceptedbudgets":
                        action = "Viewed all Approved Budgets ";
                        break;
                    case "getunapproved":
                        action = "Viewed all Pending Budgets ";
                        break;
                    case "getflaggedbudgets":
                        action = "Viewed all Flagged Budgets ";
                        break;
                    case "exporttoexcel":
                        action = "Exported all selected Budgets ";
                        break;
                    case "approveselectedbudgets":
                        action = "Approved all selected Budgets";
                        break;
                    case "editbudgets":
                        action = "Is editing all selected Budgets";
                        break;
                    case "upload":
                        action = "Is uploading Budgets";
                        break;
                    case "rolemanagement":
                        action = "Viewed the Manage User page";
                        break;
                    case "assignrole":
                        action = "Edited the Role of a user";
                        break;
                    case "removerole":
                        action = "Deleated a user";
                        break;
                    default:
                        action = actionTemp;
                        break;
                }
            }

            string DoneOn = "";

            if (controller != null)
            {
                switch (controller)
                {
                    case "Budgets":
                        if (context.HttpContext.Request.Method == "POST") {
                              DoneOn = "BudgetID(s): ";
                            

                            foreach (var field in context.HttpContext.Request.Form)
                            {
                                // Assuming that field.Key is the property name and field.Value is the property value
                                var budget = new Budget();
                                // Initialize the properties based on the form fields
                                // Adjust the property names as needed, and handle data type conversions
                                Console.WriteLine(field.Key, ": hhhhhhhhhhhhhhhhh  ", field.Value);
                                //if (field.Key == "finYr")
                                //    budget.FinYr = finYr;

                                //if (float.TryParse(field.Value["Yr"], out var yr))
                                //    budget.Yr = yr;

                                //if (int.TryParse(field.Value["StoreNo"], out var storeNo))
                                //    budget.StoreNo = storeNo;

                                //if (int.TryParse(field.Value["DeptNo"], out var deptNo))
                                //    budget.DeptNo = deptNo;

                                //if (int.TryParse(field.Value["MonthNo"], out var monthNo))
                                //    budget.MonthNo = monthNo;


                                // ... initialize other properties

                                // Check if the budget exists in the database
                                var existingBudget = _context.Budget.FirstOrDefault(b =>
                                    b.FinYr == budget.FinYr &&
                                    b.MonthNo == budget.MonthNo &&
                                    b.StoreNo == budget.StoreNo &&
                                    b.DeptNo == budget.DeptNo);

                                if (existingBudget != null)
                                {
                                    // Budget already exists, you can handle this situation as needed
                                    // For example, log a message or update the existing budget
                                    // If you want to skip adding to DoneOn, you can use continue to move to the next iteration
                                    DoneOn += $": {existingBudget.BudgetID}";
                                    
                                }

                                // If the budget doesn't exist, add its information to DoneOn
                                
                                }

                        }
                        break;
                    case "Admin":
                        if (context.HttpContext.Request.Method == "POST") {
                            string userID = context.HttpContext.Request.Form["userId"].ToString();

                            var user = _context.Users.FirstOrDefault(u => u.Id == userID);

                            if (user != null)
                            {
                                // Check if the user was deleted
                                if (!user.UserName.Equals("DeletedUser"))
                                {
                                    // Retrieve roles assigned to the user
                                    var roles = _context.UserRoles
                                        .Where(ur => ur.UserId == user.Id)
                                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                                        .ToList();

                                    if (roles.Any())
                                    {
                                        DoneOn = $"Username: {user.UserName}, Roles: {string.Join(", ", roles)}";
                                    }
                                    else
                                    {
                                        DoneOn = $"Username: {user.UserName}, No Roles assigned";
                                    }
                                }
                                else
                                {
                                    DoneOn = $"User ({user.UserName}) was deleted";
                                }
                            }
                            else
                            {
                                DoneOn = $"User not found or User with ID ({userID}) was deleted";
                            }
                        }
                        break; 
                       
                    default:
                        DoneOn = "";
                        break;
                }
            }

            DateTime createdAt = DateTime.Now;
            // Access the user id from the HttpContext
            string userId = context.HttpContext.User.Identity.Name;

            // Create an ActionLog instance and save it to the database
            var actionLog = new ActionLog
            {
                User = userId,
                Controller = controller,
                Action = action,
                CreatedAt = createdAt,
                Duration = duration,
                ActionType = context.HttpContext.Request.Method,
                DoneOn = DoneOn
            };

            try
            {
                _context.ActionLogs.Add(actionLog);
                _context.SaveChanges();
            }
            catch
            {
                // Handle exceptions as needed
            }
        }
    }

}