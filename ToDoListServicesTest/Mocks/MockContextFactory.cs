using System;
using System.Collections.Generic;
using System.Text;
using ToDoListServices.Contracts;
using ToDoListServices.Data;

namespace ToDoListServicesTest.Mocks
{
    class MockContextFactory : ITodoContextFactory
    {
        private readonly TodoDbContext _context;
        public MockContextFactory(TodoDbContext context)
        {
            _context = context;
        }

        public TodoDbContext Create()
        {
            return _context;
        }
    }
}
