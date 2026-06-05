using ExamSheduleDesign.Repositories;
using ExamSheduleDesign.Services;
using ExamSheduleDesign.Services.Interfaces;
using ExamSheduleDesign.Services.Logging;
using ExamSheduleDesign.ViewModels;
using ExamSheduleDesign.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDataService, SqlDataService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IAppLogger, FileLogger>();
            services.AddSingleton<IDocumentGenerator, WordDocumentGenerator>();
            services.AddSingleton<DataImporter>();
            services.AddSingleton<IConnectionSettingsService, ConnectionSettingsService>();
            services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IBufferService, BufferService>();

            // Репозитории
            services.AddSingleton<ITeacherRepository, TeacherRepository>();
            services.AddSingleton<IDisciplineRepository, DisciplineRepository>();
            services.AddSingleton<IGroupRepository, GroupRepository>();
            services.AddSingleton<IExamRepository, ExamRepository>();

            // ViewModels – теперь Singleton для сохранения состояния
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ScheduleViewModel>();
            services.AddSingleton<AddDataViewModel>();
            services.AddSingleton<EditDataViewModel>();
            services.AddSingleton<DevViewModel>();
            services.AddSingleton<SettingsViewModel>();

            // Views – оставляем Transient (они создаются каждый раз)
            services.AddTransient<MainWindow>();
            services.AddTransient<MainView>();
            services.AddTransient<ScheduleView>();
            services.AddTransient<AddDataView>();
            services.AddTransient<EditDataView>();
            services.AddTransient<DevView>();
            services.AddTransient<SettingsView>();
            services.AddTransient<NavigationBar>();

            Services = services.BuildServiceProvider();

            // Получаем фабрику подключений из DI
            var connectionFactory = Services.GetRequiredService<IDbConnectionFactory>();

            Task.Run(() =>
            {
                try
                {
                    using (var conn = connectionFactory.CreateServerConnection())
                    {
                        conn.Open();
                        Debug.WriteLine("Сервер SQL Server доступен.");
                    }
                }
                catch (Exception)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Внимание: сервер SQL Server недоступен. Справочные данные не загружены. Работа возможна только с локальными экзаменами.", "Офлайн-режим", MessageBoxButton.OK, MessageBoxImage.Warning);
                    });
                }
            });

            // Создаём главное окно с защитой от ошибок
            try
            {
                var mainWindow = Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка при запуске:\n{ex.Message}\n\n{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}