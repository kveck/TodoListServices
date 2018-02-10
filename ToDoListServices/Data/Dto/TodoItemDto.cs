namespace ToDoListServices.Data.Dto
{
    using System;

    public class TodoItemDto
    {
        public int ItemId { get; set; }

        public string Description { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public String CurrentStatus { get; set; }

        public DateTime? LastModifiedDateTime { get; set; }
    }
}
