namespace ToDoListServices.Data
{
    using Microsoft.EntityFrameworkCore;
    using ToDoListServices.Contracts;

    public class TodoDbContextFactory : ITodoContextFactory
    {
        private readonly string _connection;

        public TodoDbContextFactory(string connection)
        {
            _connection = connection;
        }

        public TodoDbContext Create()
        {
            return new TodoDbContext(new DbContextOptionsBuilder<TodoDbContext>().UseSqlServer(this._connection).Options);
        }
    }
}
