using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using RestSharp;
using RestSharp.Deserializers;

namespace NElasticsearch
{
    public class ElasticsearchRestClient
    {
        private readonly ClientsPool<RestClient> _clientsPool = new ClientsPool<RestClient>();

        public ElasticsearchRestClient(params string[] elasticsearchUrls)
            : this(elasticsearchUrls.Select(x => new Uri(x)).ToArray())
        {
            
        }

        public ElasticsearchRestClient(params Uri[] elasticsearchUrls)
        {
            foreach (var elasticsearchUrl in elasticsearchUrls)
            {
                var internalClient = new RestClient(elasticsearchUrl.ToString())
                {
                    Authenticator = Authenticator,
                    CookieContainer = CookieContainer,
                    Proxy = Proxy,
                    UserAgent = UserAgent,
                    UseSynchronizationContext = UseSynchronizationContext,
                    Timeout = Timeout,
                    ClientCertificates = ClientCertificates,
                };
                internalClient.ClearHandlers();
                internalClient.AddHandler("application/json", new JsonDeserializer());
                internalClient.AddHandler("text/json", new JsonDeserializer());
                internalClient.AddHandler("text/x-json", new JsonDeserializer());
                internalClient.AddHandler("*", new JsonDeserializer());
                _clientsPool.Add(internalClient);
            }            
        }

        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            return _clientsPool.GetEndpoint().RestClient.ExecuteAsync(request, callback);
        }

        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback)
        {
            return _clientsPool.GetEndpoint().RestClient.ExecuteAsync<T>(request, callback);
        }

        public IRestResponse Execute(IRestRequest request)
        {
            while (true)
            {
                var endpoint = _clientsPool.GetEndpoint();
                var rsp = endpoint.RestClient.Execute(request);
                if (rsp.StatusCode == 0 && rsp.ErrorException is WebException)
                {
                    endpoint.MarkFailure();
                    continue;
                }
                return rsp;
            }
        }

        public IRestResponse<T> Execute<T>(IRestRequest request) where T : new()
        {
            while (true)
            {
                var endpoint = _clientsPool.GetEndpoint();
                var rsp = endpoint.RestClient.Execute<T>(request);
                if (rsp.StatusCode == 0 && rsp.ErrorException is WebException)
                {
                    endpoint.MarkFailure();
                    continue;
                }
                return rsp;
            }
        }        

        public CookieContainer CookieContainer { get; set; }
        public string UserAgent { get; set; }
        public int Timeout { get; set; }
        public bool UseSynchronizationContext { get; set; }
        public IAuthenticator Authenticator { get; set; }
        public X509CertificateCollection ClientCertificates { get; set; }
        public IWebProxy Proxy { get; set; }

        public string DefaultIndexName { get; set; }                
    }
}
