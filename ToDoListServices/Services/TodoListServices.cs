namespace ToDoListServices.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using ToDoListServices.Contracts;
    using ToDoListServices.Common;
    using ToDoListServices.Common.Extensions;
    using ToDoListServices.Common.ErrorHandling;
    using ToDoListServices.Data.Dto;
    using ToDoListServices.Data.Models;

    /// <inheritdoc />
    /// <summary>
    /// Services class to isolate logic from controller
    /// </summary>
    public class TodoListServices : ITodoListServices
    {
        private ITodoContextFactory TodoContextFactory { get; }

        private readonly ILogger _logger;

        public TodoListServices(ITodoContextFactory contextFactory, ILogger<TodoListServices> logger)
        {
            Guard.NotNull(logger, nameof(logger));
            Guard.NotNull(contextFactory, nameof(contextFactory));

            this.TodoContextFactory = contextFactory;
            this._logger = logger;
        }

        /// <summary>
        /// Adds the to-do itemDto to the db
        /// </summary>
        /// <param name="itemDto"></param>
        /// <returns></returns>
        public async Task<TodoItemDto> AddItemAsync(TodoItemDto itemDto)
        {
            Guard.NotNull(itemDto, nameof(itemDto));
            Guard.NotNull(itemDto.Description, nameof(itemDto.Description));

            _logger.LogInformation($"In {nameof(AddItemAsync)}");
            try
            {
                var todoItem = itemDto.ToEntity();

                //set create time to now
                todoItem.CreateDateTime = DateTime.UtcNow;
                var status = todoItem.StatusHistory.First();
                if (todoItem.StatusHistory == null || status == null)
                {
                    status = new TodoItemStatus {Status = itemDto.CurrentStatus ?? StatusValues.New};
                    todoItem.StatusHistory = new List<TodoItemStatus> {status};
                }

                status.StatusDateTime = todoItem.CreateDateTime;

                using (var context = TodoContextFactory.Create())
                {
                    var todoResult = await context.ToDoItems.AddAsync(todoItem);
                    await context.SaveChangesAsync();

                    itemDto.CreateDateTime = todoResult.Entity.CreateDateTime;
                    itemDto.ItemId = todoResult.Entity.ItemId;

                    // set the status fields
                    status = todoResult.Entity.StatusHistory.First();
                    itemDto.CurrentStatus = status.Status;
                    itemDto.LastModifiedDateTime = status.StatusDateTime;
                }

                return itemDto;
            }
            catch (Exception)
            {
                //log error
                throw;
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(AddItemAsync)}");
            }
        }

        /// <summary>
        /// Get all To-do items
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<TodoItemDto>> GetAllItemsAsync()
        {
            try
            {
                using (var context = TodoContextFactory.Create())
                {
                    var items = await context
                        .ToDoItems
                        .Include(i => i.StatusHistory).AsNoTracking()
                        .Select(item => item.ToDto())                        
                        .ToListAsync();

                    return items;
                }
            }
            catch (Exception)
            {
                //log error
                throw;
            }
        }

        /// <summary>
        /// Find and return the requested to-do itemDto
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task<TodoItemDto> FindItemAsync(int itemId)
        {
            _logger.LogInformation($"In {nameof(FindItemAsync)}");

            try
            {
                using (var context = TodoContextFactory.Create())
                {
                    var todoItem =  await context
                        .ToDoItems
                        .Include(i => i.StatusHistory).AsNoTracking()
                        .FirstOrDefaultAsync(item => item.ItemId == itemId);

                    if (todoItem == null)
                        throw new ItemNotExistsException(itemId, "Failed to find to-do item.");

                    return todoItem.ToDto();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(FindItemAsync)}");
            }
        }

        public async Task DeleteItemAsync(int itemId)
        {
            _logger.LogInformation($"In {nameof(DeleteItemAsync)}");
            try
            {
                using (var context = TodoContextFactory.Create())
                {
                    // try to ge the to-do item with its status history
                    var todoItem = await context
                        .ToDoItems
                        .Include(i => i.StatusHistory)
                        .FirstOrDefaultAsync(i => i.ItemId == itemId);

                    if (todoItem == null)
                        throw new ItemNotExistsException(itemId, "Failed to delete to-do item.");

                    // first delete all status items
                    context.TodoItemStatuses.RemoveRange(todoItem.StatusHistory);

                    // now delete to-do itemDto
                    context.ToDoItems.Remove(todoItem);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(DeleteItemAsync)}");
            }
        }

        /// <summary>
        /// updates specified item's description and status if changed
        /// </summary>
        /// <param name="itemDto"></param>
        /// <returns>updated DTO</returns>
        public async Task<TodoItemDto> UpdateItemAsync(TodoItemDto itemDto)
        {
            _logger.LogInformation($"In {nameof(UpdateItemAsync)}");
            Guard.NotNull(itemDto, nameof(itemDto));

            try
            {
                var updateStatus = !string.IsNullOrEmpty(itemDto.CurrentStatus);
                var updateDescription = !string.IsNullOrEmpty(itemDto.Description);
                using (var context = TodoContextFactory.Create())
                {
                    // try to get requesetd item
                    var todoItem = await context.ToDoItems.FirstOrDefaultAsync(i => i.ItemId == itemDto.ItemId);
                    if (todoItem == null)
                        throw new ItemNotExistsException(itemDto.ItemId, "Failed to update to-do item.");

                    // check if discription is changed
                    if (updateDescription &&
                        !todoItem.Description.Equals(itemDto.Description, StringComparison.CurrentCultureIgnoreCase))
                    {
                        todoItem.Description = itemDto.Description;
                    }

                    // check if status has changed
                    if (updateStatus)
                    {
                        // check if new status is valid
                        if (!StatusValues.IsValid(itemDto.CurrentStatus))
                            throw new InvalidStatusException(itemDto.CurrentStatus,
                                "Failed to update to-do item status.");

                        // get current status string from to-do item entity, if exists
                        var currentSavedStatus = string.Empty;
                        if (todoItem.StatusHistory != null)
                        {
                            currentSavedStatus = todoItem.StatusHistory
                                .OrderByDescending(s => s.StatusDateTime)
                                .First()
                                ?.Status;
                        }

                        // check if current saved status is the same as the new status, if not then update status
                        if (string.IsNullOrEmpty(currentSavedStatus) ||
                            !currentSavedStatus.Equals(itemDto.CurrentStatus, StringComparison.OrdinalIgnoreCase))
                        {
                            if (todoItem.StatusHistory == null)
                            {
                                todoItem.StatusHistory = new List<TodoItemStatus>();
                            }

                            // add new status entry
                            todoItem.StatusHistory.Add(new TodoItemStatus()
                            {
                                ItemId = todoItem.ItemId,
                                Status = itemDto.CurrentStatus,
                                StatusDateTime = DateTime.UtcNow
                            });
                        }
                    }

                    // save changes
                    await context.SaveChangesAsync();

                    // return updated to-do item
                    return todoItem.ToDto();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(UpdateItemAsync)}");
            }
        }
    }
}
