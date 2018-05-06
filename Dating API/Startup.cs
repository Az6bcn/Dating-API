using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatingAPI.Data;
using DatingAPI.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dating_API
{
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
            /* Specify the services we want to consume in our App, these services will be used as DI by controllers, Repositories etc*/

            

            // Register DbContext bcos it'll be injected into other parts of our App 
            services.AddDbContext<DataDbContext>(
                // tell and configure the dB provider we want to use e.g: sqllite, sql server etc )
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")));

            
            // Register CORS 
            services.AddCors();

            // Register Repository 
            services.AddScoped<IAuthRepository, AuthRepository>();



            var jwtKey = Configuration.GetSection(nameof(JwtKey));
            var key = jwtKey[nameof(JwtKey.SignKey)];
            var signKey = Encoding.ASCII.GetBytes(key); // encode key as Byte[]

            // Add Authentication => to authenticate and only allow authenticated users 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                              {
                                  options.TokenValidationParameters = new TokenValidationParameters
                                                                      {
                                                                          // parameters to validate in clients token
                                                                          ValidateIssuerSigningKey = true,
                                                                          IssuerSigningKey =
                                                                              new SymmetricSecurityKey(signKey),
                                                                          ValidateIssuer   = false,
                                                                          ValidateAudience = false
                                                                      };
                              });



            // Register IOptions
            services.AddOptions();
            services.Configure<JwtKey>(Configuration.GetSection(nameof(JwtKey))); // Loads up "Jwt" from appsetting.json into Jwt POCO.

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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




            // Add Authentication to the pipeline
            app.UseAuthentication();


            
            app.UseMvc();
        }
    }
}
