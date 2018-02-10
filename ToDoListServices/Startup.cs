namespace ToDoListServices
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json.Serialization;
    using Swashbuckle.AspNetCore.Swagger;
    using ToDoListServices.Contracts;
    using ToDoListServices.Data;
    using ToDoListServices.Services;
    using ToDoListServices.Swagger;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(setup => setup
                    .SerializerSettings
                    .ContractResolver = new CamelCasePropertyNamesContractResolver());

            // auth options to control user permissions
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,    // validate the server that created that token
                        ValidateAudience = true,  // ensure that the recipient of the token is authorized to receive it 
                        ValidateLifetime = true,  // check that the token is not expired and that the signing key of the issuer is valid 
                        ValidateIssuerSigningKey = true, // verify that the key used to sign the incoming token is part of a list of trusted keys 
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            //using Dependency Injection
            services.AddScoped<ITodoListServices, TodoListServices>();
            services.AddSingleton<ITodoContextFactory>(s => new TodoDbContextFactory(Configuration.GetConnectionString("DefaultConnection")));

            // add swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "TodoList API",
                    Version = "v1",
                    Description = "A simple example .NET Core 2 Web API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Kristin Veck",
                        Email = "dsc_kveck@yahoo.com",
                        Url = "https://www.linkedin.com/in/krisveck/"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "ToDoListServices.xml");
                c.IncludeXmlComments(xmlPath);

                // Add the authorization header
                c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                c.DocumentFilter<AuthorizationHeaderParameterOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole().AddDebug();

            //TODO: add more involved error handling to return more useful information
            //var errHandler = new ErrorHandlerMiddleware(includeFullErrorStack: true);
            //app.UseExceptionHandler(new ExceptionHandlerOptions() { ExceptionHandler = errHandler.Invoke });

            // TODO: restrict CORS??
            // allow cross-domain requests (must preceed - useMvc call)
            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            // add support for JWT authorization
            app.UseAuthentication();

            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo List API V1");
            });

            app.UseMvc();
        }
    }
}
