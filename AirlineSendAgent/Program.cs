// See https://aka.ms/new-console-template for more information

using AirlineSendAgent.App;
using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
{
  services.AddSingleton<IAppHost, AppHost>();
  services.AddSingleton<IWebhookClient, WebhookClient>();
  services.AddDbContext<SendAgentDbContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("AirlineConnection")));
  services.AddHttpClient();
}).Build();

host.Services.GetService<IAppHost>().Run();