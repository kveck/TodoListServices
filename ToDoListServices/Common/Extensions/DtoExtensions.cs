namespace ToDoListServices.Common.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using ToDoListServices.Data.Dto;
    using ToDoListServices.Common;
    using ToDoListServices.Common.ErrorHandling;
    using ToDoListServices.Data.Models;

    /// <summary>
    /// contains all extensions related to the DTO
    /// </summary>
    public static class DtoExtensions
    {
        public static IList<TodoItemDto> ToDto(this IEnumerable<ToDoItem> todoItems)
        {
            Guard.NotNull(todoItems, nameof(todoItems));

            return todoItems.Select(entity => entity.ToDto()).ToList();
        }

        /// <summary>
        /// converts a <see cref="ToDoItem"/> entity instance
        /// to a new <see cref="TodoItemDto"/> instance
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TodoItemDto ToDto(this ToDoItem entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var item = new TodoItemDto
            {
                ItemId = entity.ItemId,
                CreateDateTime = entity.CreateDateTime,
                Description = entity.Description,
            };

            // set status info
            if (entity.StatusHistory == null || !entity.StatusHistory.Any())
                return item;
            
            // find to-do status with most recent modification date
            var currentStatus = entity
                .StatusHistory
                .OrderByDescending(s => s.StatusDateTime)
                .FirstOrDefault();

            if (currentStatus == null)
                return item;

            item.LastModifiedDateTime = currentStatus.StatusDateTime;
            item.CurrentStatus = currentStatus.Status;

            return item;
        }

        /// <summary>
        /// converts a <see cref="TodoItemDto"/> instance to a 
        /// new <see cref="ToDoItem"/> entity reference
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static ToDoItem ToEntity(this TodoItemDto dto)
        {
            Guard.NotNull(dto, nameof(dto));

            return new ToDoItem()
            {
                ItemId = dto.ItemId,
                CreateDateTime = dto.CreateDateTime,
                Description = dto.Description,
                StatusHistory = new List<TodoItemStatus>
                {
                    new TodoItemStatus {Status = dto.CurrentStatus ?? StatusValues.New, StatusDateTime = dto?.LastModifiedDateTime}
                }
            };
        }
    }
}
