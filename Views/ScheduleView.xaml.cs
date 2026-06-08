using ExamSheduleDesign.Models;
using ExamSheduleDesign.ViewModels;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ExamSheduleDesign.Views
{
    public partial class ScheduleView : UserControl
    {
        private readonly ScheduleViewModel _viewModel;

        public ScheduleView(ScheduleViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += async (s, e) => await _viewModel.LoadExamsAsync();

            // Подписываемся на изменение свойства SortColumn в ViewModel
            _viewModel.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(ScheduleViewModel.SortColumn))
                {
                    ReorderColumns();
                }
            };
        }

        private void ReorderColumns()
        {
            // Маппинг между отображаемым именем столбца и его контролом
            var columnMapping = new Dictionary<string, DataGridColumn>
            {
                ["Преподаватель 1"] = Teacher1Column,
                ["Преподаватель 2"] = Teacher2Column,
                ["Дата"] = DateColumn,
                ["Дисциплина"] = DisciplineColumn,
                ["Группа"] = GroupColumn,
                ["Отделение"] = DepartmentColumn,
                ["Время"] = TimeColumn,
                ["Аудитория"] = ClassroomColumn,
                ["Тип"] = TypeColumn
            };

            if (columnMapping.TryGetValue(_viewModel.SortColumn, out var selectedColumn))
            {
                // Устанавливаем DisplayIndex выбранной колонки = 1 (после чекбокса)
                selectedColumn.DisplayIndex = 1;

                // Остальные колонки (кроме чекбокса и выбранной) получают индексы, начиная с 2
                int index = 2;
                foreach (var column in new DataGridColumn[]
                {
                    Teacher1Column, Teacher2Column, DateColumn, DisciplineColumn,
                    GroupColumn, DepartmentColumn, TimeColumn, ClassroomColumn, TypeColumn
                })
                {
                    if (column != selectedColumn && column.DisplayIndex != 0) // не трогаем чекбокс
                    {
                        column.DisplayIndex = index++;
                    }
                }
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Находим строку, на которую кликнули (через визуальное дерево)
            var hit = e.OriginalSource as System.Windows.DependencyObject;
            while (hit != null && !(hit is DataGridRow))
                hit = System.Windows.Media.VisualTreeHelper.GetParent(hit);

            if (hit is DataGridRow row && row.DataContext is Exam exam)
            {
                exam.IsSelected = !exam.IsSelected;
            }
        }
    }
}