namespace ExamSheduleDesign.Services
{
    public interface IConnectionStringProvider
    {
        string GetServerConnectionString();
        string GetLocalConnectionString();
        void Refresh();
    }
}