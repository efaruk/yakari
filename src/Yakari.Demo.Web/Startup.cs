namespace Yakari.Demo.Web
{
    using System;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Yakari.RedisClient;
    using Yakari.Serializer.Newtonsoft;

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
            // TODO: Take from settings
            var tribeName = "sioux";
            var memberName = $"beaver-{Environment.MachineName}-{Process.GetCurrentProcess().Id}";
            var redisConnectionString = "redis:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=true,synctimeout=5000,allowAdmin=true";

            services.AddSingleton<IDemoHelper, DemoHelper>(p => new DemoHelper(tribeName, memberName));
            services.AddSingleton<ISerializer,JsonNetSerializer>();
            services.AddSingleton<ISubscriptionManager, RedisSubscriptionManager>(p => new RedisSubscriptionManager(redisConnectionString, tribeName, p.GetRequiredService<ILogger<RedisSubscriptionManager>>()));
            services.AddSingleton<IRemoteCacheProvider, RedisCacheProvider>(p => new RedisCacheProvider(redisConnectionString, p.GetRequiredService<ISerializer>(), p.GetRequiredService<ILogger<RedisCacheProvider>>()));
            services.AddSingleton<ILocalCacheProviderOptions, LocalCacheProviderOptions>();
            services.AddSingleton<ILocalCacheProvider, LittleThunder>();
            services.AddSingleton<ICacheObserver, GreatEagle>(p => new GreatEagle(memberName, p.GetRequiredService<ISubscriptionManager>(), p.GetRequiredService<ISerializer>(), 
                p.GetRequiredService<ILocalCacheProvider>(), p.GetRequiredService<IRemoteCacheProvider>(), p.GetRequiredService<ILogger<GreatEagle>>()));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // Construct and start ICacheObserver and load cache
            _ = app.ApplicationServices.GetRequiredService<ICacheObserver>();
        }
    }
}
