using System;

namespace ExamSheduleDesign.Services
{
    public interface IExamCountNotifier
    {
        event Action<int> CountChanged;
        void UpdateCount(int count);
    }
}