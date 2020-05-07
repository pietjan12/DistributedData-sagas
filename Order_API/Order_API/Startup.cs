    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order_API.Handler;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Serilog;

namespace Order_API
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
            services.AddControllers();

            //configure rebus
            services.AddRebus(configure => configure
            .Logging(l => l.Serilog(Log.Logger))
            .Transport(t => t.UseRabbitMq(Configuration.GetValue<string>("Connection:rabbitmq"), Configuration.GetValue<string>("rabbitmq:queuename")))
            //TIMEOUTS EN SAGAS CONNECTION STRING IS AAN HET KUTTEN. TODO FIXEN
            .Sagas(sa => sa.StoreInSqlServer(Configuration.GetConnectionString("Database"), Configuration.GetValue<string>("SagaDataTableName"), Configuration.GetValue<string>("SagaIndexTableName"), true))
            .Timeouts(to => to.StoreInSqlServer(Configuration.GetConnectionString("Database"), Configuration.GetValue<string>("TimeoutsTableName"), true))
            .Routing(x => x.TypeBased()));

            services.AddRebusHandler<OrderSaga>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.UseRebus(async bus => {
                //tell rebus which events to watch out for
                await bus.Subscribe<OrderCreatedEvent>();
                await bus.Subscribe<OrderStockAvailableEvent>();
                await bus.Subscribe<OrderPaymentReservedEvent>();
                await bus.Subscribe<OrderFailedEvent>();
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
