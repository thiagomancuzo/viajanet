using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Management;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViajaNet.ThiagoMancuzo.Metrics.Infra.Couchbase
{
    public class BucketProvider : IBucketProvider
    {
        public void Dispose()
        {
            
        }

        public IBucket GetBucket(string bucketName)
        {
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://127.0.0.1:8091") },                
                UseSsl = false
            });

            var authenticator = new PasswordAuthenticator("viajanetuser", "viajanetpassword");
            cluster.Authenticate(authenticator);
            return cluster.OpenBucket(bucketName);
        }

        public IBucket GetBucket(string bucketName, string password)
        {
            var cluster = new Cluster(new CouchbaseClientDefinition
            {
                Servers = new List<Uri> { new Uri("http://127.0.0.1:8091") },
                Username = "viajanetuser",
                Password = password
            });

            return cluster.OpenBucket(bucketName);
        }
    }
}
