using ExamSheduleDesign.Models;
using System.Collections.Generic;

namespace ExamSheduleDesign.Services
{
    class MockDataService : IDataService
    {
        private List<Teacher> _teachers;
        private List<Discipline> _disciplines;
        private List<Group> _groups;
        private List<Exam> _exams;

        public MockDataService()
        {
            // Тестовые преподаватели
            _teachers = new List<Teacher>
            {
                new Teacher { Id = 1, FullName = "Иванова М.С." },
                new Teacher { Id = 2, FullName = "Петров А.В." },
                new Teacher { Id = 3, FullName = "Сидорова Е.Н." },
                new Teacher { Id = 4, FullName = "Козлов Д.И." },
                new Teacher { Id = 5, FullName = "Новикова Л.П." }
            };

            // Тестовые дисциплины
            _disciplines = new List<Discipline>
            {
                new Discipline { Id = 1, Name = "Математический анализ" },
                new Discipline { Id = 2, Name = "Программирование" },
                new Discipline { Id = 3, Name = "Физика" },
                new Discipline { Id = 4, Name = "Базы данных" }
            };

            // Тестовые группы
            _groups = new List<Group>
            {
                new Group { Id = 1, Name = "ПИ-21", Department = "Информатика" },
                new Group { Id = 2, Name = "ПИ-22", Department = "Информатика" },
                new Group { Id = 3, Name = "ИВТ-21", Department = "Информатика" },
                new Group { Id = 4, Name = "ИВТ-22", Department = "Информатика" }
            };

            // Несколько тестовых экзаменов
            _exams = new List<Exam>();
        }

        public List<Teacher> GetTeachers() => _teachers;
        public List<Discipline> GetDisciplines() => _disciplines;
        public List<Group> GetGroups() => _groups;

        public void AddExam(Exam exam)
        {
            exam.Id = _exams.Count + 1;
            _exams.Add(exam);
        }

        public List<Exam> GetExams() => _exams;
    }
}
