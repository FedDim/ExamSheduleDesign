using ExamSheduleDesign.Services;
using ExamSheduleDesign.ViewModels;
using ExamSheduleDesign.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ExamSheduleDesign
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Сервисы
            services.AddSingleton<IDataService, MockDataService>();
            services.AddSingleton<INotificationService, NotificationService>();

            // ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ScheduleViewModel>();
            services.AddTransient<AddDataViewModel>();
            services.AddTransient<EditDataViewModel>();
            services.AddTransient<DevViewModel>();
            services.AddTransient<SettingsViewModel>();

            // Views
            services.AddTransient<MainWindow>();
            services.AddTransient<MainView>();
            services.AddTransient<ScheduleView>();
            services.AddTransient<AddDataView>();
            services.AddTransient<EditDataView>();
            services.AddTransient<DevView>();
            services.AddTransient<SettingsView>();
            services.AddTransient<NavigationBar>();

            Services = services.BuildServiceProvider();
            // Регистрируем сам провайдер для внедрения в ViewModel
            services.AddSingleton<IServiceProvider>(Services);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}