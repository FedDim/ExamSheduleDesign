using ExamSheduleDesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamSheduleDesign.Services
{
    class MockDataService : IDataService
    {
        private List<Teacher> _teachers;
        private List<Discipline> _disciplines;
        private List<Group> _groups;
        private List<Exam> _exams;
        private List<Exam> _generatedExams = new();

        public MockDataService()
        {
            // Тестовые преподаватели
            _teachers = new List<Teacher>
            {
                new Teacher { Id = 1, Name = "Иванова М.С." },
                new Teacher { Id = 2, Name = "Петров А.В." },
                new Teacher { Id = 3, Name = "Сидорова Е.Н." },
                new Teacher { Id = 4, Name = "Козлов Д.И." },
                new Teacher { Id = 5, Name = "Новикова Л.П." }
            };

            // Тестовые дисциплины
            _disciplines = new List<Discipline>
            {
                new Discipline { Id = 1, FullName = "Математический анализ" },
                new Discipline { Id = 2, FullName = "Программирование" },
                new Discipline { Id = 3, FullName = "Физика" },
                new Discipline { Id = 4, FullName = "Базы данных" }
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

        public void RemoveExams(IEnumerable<Exam> exams)
        {
            foreach (var exam in exams.ToList())
                _exams.Remove(exam);
        }

        public void AddTeacher(Teacher teacher)
        {
            teacher.Id = _teachers.Max(t => t.Id) + 1;
            _teachers.Add(teacher);
        }

        public void AddDiscipline(Discipline discipline)
        {
            discipline.Id = _disciplines.Max(d => d.Id) + 1;
            _disciplines.Add(discipline);
        }

        public void AddGroup(Group group)
        {
            group.Id = _groups.Max(g => g.Id) + 1;
            _groups.Add(group);
        }

        public void UpdateTeachers(IEnumerable<Teacher> updatedTeachers)
        {
            _teachers = updatedTeachers.ToList();
        }

        public void UpdateDisciplines(IEnumerable<Discipline> updatedDisciplines)
        {
            _disciplines = updatedDisciplines.ToList();
        }

        public void UpdateGroups(IEnumerable<Group> updatedGroups)
        {
            _groups = updatedGroups.ToList();
        }

        public void GenerateExams(int count)
        {
            var random = new Random();
            var teachers = _teachers;
            var disciplines = _disciplines;
            var groups = _groups;
            var times = new[] { "9:00", "10:40", "11:00", "13:00", "13:30", "14:35", "15:00", "16:20" };
            var rooms = new[] { "301", "205", "402", "110", "215", "101", "305" };
            var types = new[] { "Экзамен", "Консультация" };
            var startDate = DateTime.Today.AddDays(7); // через неделю

            var newExams = new List<Exam>();
            for (int i = 0; i < count; i++)
            {
                var exam = new Exam
                {
                    Date = startDate.AddDays(random.Next(0, 14)),
                    Time = times[random.Next(times.Length)],
                    Type = types[random.Next(types.Length)],
                    Teacher1 = teachers[random.Next(teachers.Count)],
                    Teacher2 = random.Next(2) == 0 ? null : teachers[random.Next(teachers.Count)],
                    Discipline = disciplines[random.Next(disciplines.Count)],
                    Group = groups[random.Next(groups.Count)],
                    Classroom = rooms[random.Next(rooms.Length)],
                    IsSelected = false
                };
                newExams.Add(exam);
            }
            _exams.AddRange(newExams);
            _generatedExams.AddRange(newExams);
        }

        public void ClearGeneratedExams()
        {
            foreach (var exam in _generatedExams)
                _exams.Remove(exam);
            _generatedExams.Clear();
        }

        public int GetGeneratedExamsCount() => _generatedExams.Count;

        public List<Exam> GetExams() => _exams;
    }
}
