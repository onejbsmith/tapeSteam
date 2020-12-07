// other usings
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorStrap;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using tapeStream.Shared;
using tapeStream.Server.Data;
using tapeStream.Server.Hubs;
using Microsoft.AspNetCore.Http.Connections;

namespace tapeStream.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddScoped<tapeStream.Server.Data.BrowserService>(); // scoped service
            services.AddTransient<BlazorTimer>();

            services.AddBlazorise(options =>
           { options.ChangeTextOnKeyPress = true; }).AddBootstrapProviders().AddFontAwesomeIcons();
            // other services      
            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddTransient<TDAApiService>();
            services.AddSignalR();




            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddScoped<Radzen.DialogService>();
            services.AddBootstrapCss();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );

                options.AddPolicy("signalr",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()

                    .AllowCredentials()
                    .SetIsOriginAllowed(hostName => true));
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            // Make sure the CORS middleware is ahead of SignalR.
            app.UseCors(builder =>
            {
                 
                builder.WithOrigins("http://localhost:53911")
                    .AllowAnyHeader()
                    .WithMethods("GET", "HEAD", "POST")
                    .AllowCredentials();


                builder.WithOrigins("http://localhost:59788")
                    .AllowAnyHeader()
                    .WithMethods("GET", "HEAD", "POST")
                    .AllowCredentials();

                builder.WithOrigins("http://localhost:2575")
                    .AllowAnyHeader()
                    .WithMethods("GET", "HEAD", "POST")
                    .AllowCredentials();

                builder.WithOrigins("http://192.168.1.108:2575")
                    .AllowAnyHeader()
                    .WithMethods("GET", "HEAD", "POST")
                    .AllowCredentials();
                builder.WithOrigins("http://localhost:5000")
                    .AllowAnyHeader()
                    .WithMethods("GET", "HEAD", "POST")
                    .AllowCredentials();
                builder.WithOrigins("http://tapestreamclient.com")
                    .AllowAnyHeader()
                    .WithMethods("GET", "HEAD", "POST")
                    .AllowCredentials();
            });

            app.UseRouting();
            app.ApplicationServices.UseBootstrapProviders().UseFontAwesomeIcons();
            app.UseMvcWithDefaultRoute();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<ChatHub>("/chathub", options =>
                {
                    options.Transports =
                        HttpTransportType.WebSockets ;
                });

                endpoints.MapHub<TDAHub>("/tdahub", options =>
                {
                    options.Transports =
                        HttpTransportType.WebSockets ;
                });
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
