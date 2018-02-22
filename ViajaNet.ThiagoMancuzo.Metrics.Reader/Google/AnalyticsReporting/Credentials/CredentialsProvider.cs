using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.Auth.OAuth2;

namespace ViajaNet.ThiagoMancuzo.Metrics.Reader.Google.AnalyticsReporting.Credentials
{
    public class CredentialsProvider : ICredentialsProvider
    {
        public GoogleCredential GetCredential()
        {
            using (var stream = new FileStream("client_secret.json",
                 FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential.FromStream(stream)
                            .CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly);
            }
        }
    }
}
