using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Yakari.RedisClient;
using Yakari.Serializer.Newtonsoft;

namespace Yakari.Demo.Web
{
    public class Startup
    {
        private string RedisHostPort = "";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            
//            if (env.IsDevelopment())
//            {
//                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
//                builder.AddUserSecrets();
//            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();

            RedisHostPort = string.Format("{0}:{1}", Configuration["REDIS_PORT_6379_TCP_ADDR"], Configuration["REDIS_PORT_6379_TCP_PORT"]);
        }

        public IContainer ApplicationContainer { get; private set; }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Create the container builder.
            var builder = new ContainerBuilder();
            // Application name, desired to share same cache items
            var tribeName = "IstanbulCoders";
            // To seperate app instances, diagnostinc purposes, * Must ne unique: You can use Guid.NewGuid().ToString();
            var memberName = string.Format("Beaver-{0}-{1}", Environment.MachineName, Process.GetCurrentProcess().Id);
            //StackExchange.Redis connectionstring
            var redisConnectionString = "127.0.0.1:6379,abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true";
            if (RedisHostPort.Length > 1) {
                Console.WriteLine(RedisHostPort);
                redisConnectionString = string.Format("{0},abortConnect=false,defaultDatabase=1,keepAlive=300,resolveDns=false,synctimeout=5000,allowAdmin=true", RedisHostPort);
            }
            // Demo Helper
            builder.Register(c => new DemoHelper(tribeName, memberName)).As<IDemoHelper>().SingleInstance();
            // Default Logger
            var logger = new InMemoryLogger(LogLevel.Info);
            builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
            // Default Serializer
            builder.RegisterType<JsonNetSerializer>().As<ISerializer>().SingleInstance();
            //Redis Remote Cache Provider for Yakari
            builder.Register(
                        c =>
                            new RedisCacheProvider(redisConnectionString, c.Resolve<ISerializer>(), c.Resolve<ILogger>()))
                        .As<IRemoteCacheProvider>()
                        .SingleInstance();
            //Redis Subscription Manager for tribe communication.
            builder.Register(c => new RedisSubscriptionManager(redisConnectionString, tribeName, c.Resolve<ILogger>())).As<ISubscriptionManager>().SingleInstance();
            // Options class for LittleThunder.
            builder.RegisterType<LocalCacheProviderOptions>().As<ILocalCacheProviderOptions>().SingleInstance();
            // Little Thunder the Local Cache Provider
            builder.RegisterType<LittleThunder>().As<ILocalCacheProvider>().SingleInstance();
            // The Great Eagle
            builder.Register(
                        c =>
                            new GreatEagle(memberName, c.Resolve<ISubscriptionManager>(), c.Resolve<ISerializer>(),
                                c.Resolve<ILocalCacheProvider>(), c.Resolve<IRemoteCacheProvider>(),
                                c.Resolve<ILogger>())).As<ICacheObserver>().SingleInstance();

            builder.Populate(services);
            ApplicationContainer = builder.Build();

            // Great eagle start observing and loads every previous remote cache items in seperate thread
            ApplicationContainer.Resolve<ICacheObserver>();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
