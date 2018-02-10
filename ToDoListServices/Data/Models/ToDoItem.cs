namespace ToDoListServices.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ToDoItem
    {
        [Column("item_id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }

        [Column("description"), Required, StringLength(250)]
        public string Description { get; set; }

        [Column("create_date")]
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// status is a one-to-many relationship to the to-do item
        /// </summary>
        public ICollection<TodoItemStatus> StatusHistory { get; set; }
    }
}
