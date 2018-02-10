namespace ToDoListServices.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ToDoListServices.Common;
    using ToDoListServices.Data.Models;

    public static class DbInitializer
    {
        public static void Initialize(TodoDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.ToDoItems.Any())
            {
                return;   // DB has been seeded
            }

            try
            {
                var items = new List<ToDoItem>
                {
                    new ToDoItem {Description = "Todo Item 1", CreateDateTime = DateTime.UtcNow},
                    new ToDoItem {Description = "Todo Item 2", CreateDateTime = DateTime.Parse("2018-01-01")},
                    new ToDoItem {Description = "Todo Item 3", CreateDateTime = DateTime.Parse("2018-01-15")},
                    new ToDoItem {Description = "Todo Item 4", CreateDateTime = DateTime.Parse("2018-01-31")},
                    new ToDoItem {Description = "Todo Item 5", CreateDateTime = DateTime.Parse("2018-02-01")},
                    new ToDoItem {Description = "Todo Item 6", CreateDateTime = DateTime.Parse("2018-01-05")}
                };

                items.ForEach(i => context.ToDoItems.Add(i));
                context.SaveChanges();

                var status = new List<TodoItemStatus>
                {
                    new TodoItemStatus {ItemId = 1, Status = StatusValues.New, StatusDateTime = DateTime.UtcNow},
                    new TodoItemStatus {ItemId = 2, Status = StatusValues.New, StatusDateTime = DateTime.Parse("2018-01-01")},
                    new TodoItemStatus {ItemId = 3, Status = StatusValues.New, StatusDateTime = DateTime.Parse("2018-01-15")},
                    new TodoItemStatus {ItemId = 4, Status = StatusValues.New, StatusDateTime = DateTime.Parse("2018-01-31")},
                    new TodoItemStatus {ItemId = 5, Status = StatusValues.New, StatusDateTime = DateTime.Parse("2018-02-01")},
                    new TodoItemStatus {ItemId = 6, Status = StatusValues.New, StatusDateTime = DateTime.Parse("2018-01-05")},
                };

                status.ForEach(s => context.TodoItemStatuses.Add(s));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
