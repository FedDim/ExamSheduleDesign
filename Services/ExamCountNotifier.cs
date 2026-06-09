using System;

namespace ExamSheduleDesign.Services
{
    public class ExamCountNotifier : IExamCountNotifier
    {
        public event Action<int> CountChanged;

        public void UpdateCount(int count) => CountChanged?.Invoke(count);
    }
}