namespace ToDoListServices.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TodoItemStatus
    {
        /// <summary>
        /// primary key
        /// </summary>
        [Column("status_id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatusId { get; set; }

        [Column("status"), Required, StringLength(20)]
        public String Status { get; set; }

        [Column("status_date")]
        public DateTime? StatusDateTime { get; set; }

        /// <summary>
        /// foreign key mapped to itemId in ToDoItem table
        /// </summary>
        [Column("item_id"), ForeignKey("ToDoItem")]
        public int ItemId { get; set; }

        public virtual ToDoItem TodoItem { get; set; }
    }
}