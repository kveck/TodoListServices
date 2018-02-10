# TodoListServices
.NET Core 2 Web API for managing a todo list

# Data 
The TodoList applicaiton uses an in-memory database that is initalized with dummy data (see DbInitializer class)

# Swagger UI
Swagger UI has been implemented and can be accessed via the /swagger end point (example: http://localhost:54892/swagger/)

# JWT Authentication
Authentication has been implemented, but is currently disabled
To enable it, edit TodoListController class and switch the attributes [Authorize] and [AllowAnonymous]
Once enabled, you will need to generate a token using the TokenContoller api POST /api/Token
with the parameters
{
  "username": "superuser",
  "password": "password"
}

Then pass the token in the header of each of the TodoList services. The Swagger UI is setup to handle passing the bearer token in the header
