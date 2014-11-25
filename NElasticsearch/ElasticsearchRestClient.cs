using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using NElasticsearch.Models;
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

        public async Task<IRestResponse> Execute(IRestRequest request)
        {
            while (true)
            {
                var taskSource = new TaskCompletionSource<IRestResponse>();
                var endpoint = _clientsPool.GetEndpoint();
                endpoint.RestClient.ExecuteAsync(request, (restResponse, handle) => taskSource.SetResult(restResponse));
                var rsp = await taskSource.Task;
                if (rsp.StatusCode == 0 && rsp.ErrorException is WebException)
                {
                    endpoint.MarkFailure();
                    continue;
                }
                return rsp;
            }
        }

        public async Task<IRestResponse<T>> Execute<T>(IRestRequest request) where T : new()
        {
            while (true)
            {
                var taskSource = new TaskCompletionSource<IRestResponse<T>>();
                var endpoint = _clientsPool.GetEndpoint();
                endpoint.RestClient.ExecuteAsync<T>(request, (restResponse, handle) => taskSource.SetResult(restResponse));
                var rsp = await taskSource.Task;
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
