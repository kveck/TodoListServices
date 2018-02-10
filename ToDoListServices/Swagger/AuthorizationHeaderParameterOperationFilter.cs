namespace ToDoListServices.Swagger
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Implementation of Swagger <see cref="IOperationFilter"/> to add
    /// authorization header of type bearer token to api calls
    /// </summary>
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter, IDocumentFilter
    {
        /// <summary>
        /// Required <see cref="IOperationFilter"/> method 
        /// to add authorization header to Swagger UI
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline
                .Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is AuthorizeFilter);

            var allowAnonymous = filterPipeline
                .Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is IAllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();
                operation.Parameters.Add(new NonBodyParameter
                {
                    Default = "Bearer <replace with JWT>",
                    Name = "Authorization",
                    In = "header",
                    Description = "access token",
                    Required = true,
                    Type = "string"
                });
            }
        }

        /// <summary>
        /// required <see cref="IDocumentFilter"/> method
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            IList<IDictionary<string, IEnumerable<string>>> security = swaggerDoc
                .SecurityDefinitions
                .Select(securityDefinition => new Dictionary<string, IEnumerable<string>>
                {
                    {securityDefinition.Key, new string[] {"yourapi"}}
                })
                .Cast<IDictionary<string, IEnumerable<string>>>()
                .ToList();

            swaggerDoc.Security = security;
        }
    }
}
