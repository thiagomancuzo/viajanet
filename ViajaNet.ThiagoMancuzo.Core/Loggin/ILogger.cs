namespace ViajaNet.ThiagoMancuzo.Core.Loggin
{

    public interface ILogger
    {
        void LogWarning(string log);
        void LogInformation(string log);
        void LogCritical(string log);
    }
    
}