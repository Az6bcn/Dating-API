using AutoMapper;
using DatingAPI.Data;
using DatingAPI.DTOs.Profiles;
using DatingAPI.Helpers;
using DatingAPI.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;

namespace Dating_API
{
    public class Startup
    {
        private readonly ILogger _logger;
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /* Specify the services we want to consume in our App, these services will be used as DI by controllers, Repositories etc*/

            _logger.LogInformation("Adddddiiiiinnngggg Services in startUp");


            // Register DbContext bcos it'll be injected into other parts of our App 
            services.AddDbContext<DataDbContext>(
                // tell and configure the dB provider we want to use e.g: sqllite, sql server etc )
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")));

            // Automatically perform database migration (Azure)
            services.BuildServiceProvider().GetService<DataDbContext>().Database.Migrate();

            // Register CORS 
            services.AddCors();

            // Register Repository 
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();

            //register the concrete seedData class with transient and use this class in the pipeline to seed data to DB
            services.AddTransient<SeedData>();

            //register helper class
            services.AddScoped<ICloudinaryHelper, CloudinaryHelper>();


            IConfigurationSection jwtKey = Configuration.GetSection(nameof(JwtKey));
            string key = jwtKey[nameof(JwtKey.SignKey)];
            byte[] signKey = Encoding.ASCII.GetBytes(key); // encode key as Byte[]

            // Add Authentication => to authenticate and only allow authenticated users 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // parameters to validate in clients token
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(signKey),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });



            // Register IOptions
            services.AddOptions();
            services.Configure<JwtKey>(Configuration.GetSection(nameof(JwtKey))); // Loads up "Jwt" from appsetting.json into Jwt POCO.
            services.Configure<CloudinarySettings>(Configuration.GetSection(nameof(CloudinarySettings)));     // Add our Config object so it can be injected

            // Register the Swagger generator, defining 1 or more Swagger documents: generates the swagger json endpoint
            services.AddSwaggerGen(option =>
           {
               option.SwaggerDoc("v1", new Info
               {
                   Version = "v1",
                   Title = "Dating API",
                   Description = "ASP.NET Core Dating Web API",
                   TermsOfService = "None",
                   Contact = new Contact
                   {
                       Name = "Azeez Odumosu",
                       Email = "az6bcn@gmail.com",
                       Url = "https://twitter.com/az6bcn"
                   },
                   License = new License
                   {
                       Name = "Use under LICX",
                       Url = "https://example.com/license"
                   }
               });
           });


            //AutoMapper
            MapperConfiguration mapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile<UserProfile>();
            });
            services.AddAutoMapper();

            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SeedData seeder)
        {

            /* pipeline for our httpRequest*/
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            // Configure CORS
            app.UseCors(optionPolicy => optionPolicy.AllowAnyMethod()
                                                    .AllowAnyOrigin()
                                                    .AllowAnyHeader());


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dating API V1");
            });


            //seed data to DB
            seeder.Seed();

            // Add Authentication to the pipeline
            app.UseAuthentication();

            // set default homepage to index.html of the compiled Angular app
            app.UseDefaultFiles();

            //serve static file: to serve built Angular app in wwwwrooot
            app.UseStaticFiles();

            app.UseMvc(route =>
                {
                    route.MapSpaFallbackRoute(
                          name: "spa-fallback",
                          defaults: new { controller = "Fallback", action = "index" }
                      );
                });
        }
    }
}
