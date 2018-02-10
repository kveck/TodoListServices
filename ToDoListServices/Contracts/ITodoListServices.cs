namespace ToDoListServices.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ToDoListServices.Data;
    using ToDoListServices.Data.Dto;

    /// <summary>
    /// interface for DI for controller
    /// </summary>
    public interface ITodoListServices
    {
        Task<TodoItemDto> AddItemAsync(TodoItemDto itemDto);

        Task<ICollection<TodoItemDto>> GetAllItemsAsync();

        Task<TodoItemDto> FindItemAsync(int itemId);

        Task DeleteItemAsync(int itemId);

        Task<TodoItemDto> UpdateItemAsync(TodoItemDto itemDto);
    }
}
