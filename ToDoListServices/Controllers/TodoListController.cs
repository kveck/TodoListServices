namespace ToDoListServices.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using ToDoListServices.Common.ErrorHandling;
    using ToDoListServices.Contracts;
    using ToDoListServices.Data.Dto;

    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Authorize]
    //  -- switch [Authorize] to [AllowAnonymous] attribute to turn off JWT check
    [AllowAnonymous]
    public class TodoListController : Controller
    {
        private readonly ITodoListServices _services;
        private readonly ILogger _logger;

        public TodoListController(ITodoListServices services, ILogger<TodoListController> logger)
        {
            Guard.NotNull(logger, nameof(logger));
            Guard.NotNull(services, nameof(services));

            _services = services;
            _logger = logger;
        }

        //TODO: Add endpoint to get list of valid status, identifying start/end

        /// <summary>
        /// Get all to-do items
        /// </summary>
        /// <returns>List of TodoListDto object instances</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTodoItems()
        {
            //TODO: Add paging for returning large results
            _logger.LogInformation($"In {nameof(GetAllTodoItems)}");
            try
            {
                var todoItems = await this._services.GetAllItemsAsync();
               
                _logger.LogTrace("Returning data {@data}", todoItems);

                return Ok(todoItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetAllTodoItems)} unexpected error");

                //TODO: add better error handling to return errorDto 
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(GetAllTodoItems)}");
            }
        }

        /// <summary>
        /// get requested item by id
        /// </summary>
        /// <param name="id">id of the to-do item to retrieve</param>
        /// <response code="200">Returns the requested to-do item</response>
        /// <response code="400">If the requested item does not exist</response>            
        /// <response code="401">If the request is not authorized</response>
        [ProducesResponseType(typeof(TodoItemDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        [HttpGet("{id}", Name = "GetTodoItem")]
        public async Task<IActionResult> GetTodoItem(int id)
        {
            _logger.LogInformation($"In {nameof(GetTodoItem)}");
            try
            {
                var dto = await this._services.FindItemAsync(id);
                return Ok(dto);
            }
            catch (ItemNotExistsException ineEx)
            {
                _logger.LogError(ineEx, $"{nameof(GetTodoItem)} requested item does not exist");
                return BadRequest(ineEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetTodoItem)} unexpected error");

                //TODO: add better error handling to return errorDto 
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(GetTodoItem)}");
            }
        }

        /// <summary>
        /// add specified item
        /// </summary>
        /// <param name="itemDto"></param>
        /// <response code="201">Returns the newly-created item</response>
        /// <response code="400">If the item is null</response>            
        [ProducesResponseType(typeof(TodoItemDto), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        [HttpPost]
        public async Task<IActionResult> AddTodoItem([FromBody]TodoItemDto itemDto)
        {
            _logger.LogInformation($"In {nameof(AddTodoItem)}");
            try
            {
                Guard.NotNull(itemDto, nameof(itemDto));

                // add to db
                var newItem = await this._services.AddItemAsync(itemDto);

                //return added item
                return CreatedAtRoute("GetTodoItem", new {Controller = "TodoList", id = newItem.ItemId}, newItem);
            }
            catch (ItemNotExistsException ineEx)
            {
                _logger.LogError(ineEx, $"{nameof(AddTodoItem)} requested item does not exist");
                return BadRequest(ineEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddTodoItem)} unexpected error");

                //TODO: add better error handling to return errorDto 
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(AddTodoItem)}");
            }
        }

        /// <summary>
        /// update specified item
        /// </summary>
        /// <param name="id">to-do item id</param>
        /// <param name="itemDto">only description and status can be updated</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, [FromBody]TodoItemDto itemDto)
        {
            _logger.LogInformation($"In {nameof(UpdateTodoItem)}");
            try
            {
                Guard.NotNull(itemDto, nameof(itemDto));

                //set the to-do item id for the update call
                itemDto.ItemId = id;

                // update item
                var updatedItem = await this._services.UpdateItemAsync(itemDto);

                // return updated item
                return CreatedAtRoute("GetTodoItem", new { Controller = "TodoList", id = updatedItem.ItemId }, updatedItem);
            }
            catch (TodoServicesException sEx)
            {
                _logger.LogError(sEx, $"{nameof(UpdateTodoItem)} bad request");
                return BadRequest(sEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateTodoItem)} unexpected error");

                //TODO: add better error handling to return errorDto 
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(UpdateTodoItem)}");
            }
        }

        /// <summary>
        /// delete specified item
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            try
            {
                //delete from db
                await this._services.DeleteItemAsync(id);

                return Ok();
            }
            catch (ItemNotExistsException ineEx)
            {
                _logger.LogError(ineEx, $"{nameof(DeleteTodoItem)} requested item does not exist");
                return BadRequest(ineEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DeleteTodoItem)} unexpected error");

                //TODO: add better error handling to return errorDto 
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogInformation($"Out {nameof(DeleteTodoItem)}");
            }
        }
    }
}
