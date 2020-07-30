using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoneBook.Common;
using PhoneBook.Network;
using PhoneBook.ViewModels;
using PhoneBook.Views;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhoneBook
{
    public class App : Application
    {
        public IServiceProvider Container { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            InitContainer();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Container.GetService<MainWindowViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void InitContainer()
        {
            var host = Host.CreateDefaultBuilder()
              .ConfigureServices(services =>
              {
                  services.UseMicrosoftDependencyResolver();
                  ConfigureServices(services);
              })
              .ConfigureLogging(loggingBuilder =>
              {
                  //loggingBuilder.AddSplat();
              })
              .UseEnvironment(Environments.Development)
              .Build();

            Container = host.Services;
            Container.UseMicrosoftDependencyResolver();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<GetFileService>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<IXmlPhoneBookParser, XmlPhoneBookParser>();
            services.AddSingleton<IPhoneBookService, PhoneBookService>();


            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true);
            IConfiguration configuration = configBuilder.Build();
            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}