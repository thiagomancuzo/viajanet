using Google.Apis.Auth.OAuth2;

namespace ViajaNet.ThiagoMancuzo.Metrics.Reader.Google.AnalyticsReporting.Credentials
{
    public interface ICredentialsProvider
    {
        GoogleCredential GetCredential();
    }
}
