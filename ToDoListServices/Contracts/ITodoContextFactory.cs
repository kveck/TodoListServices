namespace ToDoListServices.Contracts
{
    using ToDoListServices.Data;

    public interface ITodoContextFactory
    {
        TodoDbContext Create();
    }
}
