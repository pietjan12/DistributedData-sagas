using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Other_APIS.Handlers;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Serilog;

namespace Other_APIS
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

            services.AddRebus(configure => configure
           .Logging(l => l.Serilog(Log.Logger))
           .Transport(t => t.UseRabbitMq(Configuration.GetValue<string>("Connection:rabbitmq"), Configuration.GetValue<string>("rabbitmq:queuename")))
           .Sagas(sa => sa.StoreInSqlServer(Configuration.GetConnectionString("Database"), Configuration.GetValue<string>("SagaDataTableName"), Configuration.GetValue<string>("SagaIndexTableName"), true))
           .Timeouts(to => to.StoreInSqlServer(Configuration.GetConnectionString("Database"), Configuration.GetValue<string>("TimeoutsTableName"), true))
           .Routing(x => x.TypeBased()));

            services.AddRebusHandler<PaymentHandler>();
            services.AddRebusHandler<StockHandler>();
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
                await bus.Subscribe<PaymentReserveEvent>();
                await bus.Subscribe<StockReserveEvent>();
                await bus.Subscribe<PaymentRefundEvent>();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
