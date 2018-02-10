namespace ToDoListServicesTest
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ToDoListServices.Common;
    using ToDoListServices.Data;
    using ToDoListServices.Data.Dto;
    using ToDoListServices.Services;
    using ToDoListServicesTest.Mocks;

    /// <summary>
    /// Test to-do list unit of work actions
    /// </summary>
    [TestClass]
    public class TodoListServicesTests
    {
        private TodoDbContext _context;
        private SqliteConnection _connection;
        private MockContextFactory _contextFactory;

        [TestInitialize]
        public void SetupTest()
        {
            // In-memory database only exists while the connection is open
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<TodoDbContext>()
                    .UseSqlite(_connection)
                    .Options;

                // Create the schema in the database
                _context = new TodoDbContext(options);
                DbInitializer.Initialize(_context);

                _contextFactory = new MockContextFactory(_context);
            }
            catch(Exception)
            {
                _connection.Close();
                _context = null;
                _connection = null;
            }
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _context?.Dispose();
            _connection?.Close();

            _context = null;
            _connection = null;
            _contextFactory = null;
        }

        [TestMethod]
        public async Task TestAddTodoItemSuccess()
        {
            Assert.IsNotNull(_contextFactory);

            var work = new TodoListServices(_contextFactory, 
                new Microsoft.Extensions.Logging.Abstractions.NullLogger<TodoListServices>());

            var itemDto = new TodoItemDto()
            {
                Description = "newitem"
            };

            var newItem = await work.AddItemAsync(itemDto);

            Assert.IsNotNull(newItem);
            Assert.IsTrue(itemDto.CurrentStatus == StatusValues.New);
            Assert.IsTrue(itemDto.Description == "newitem");
        }

        //TODO: add many many more test cases!!
    }
}
