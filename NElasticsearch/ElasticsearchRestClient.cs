using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;

namespace NElasticsearch
{
    public class ElasticsearchRestClient
    {
        private readonly ClientsPool _clientsPool = new ClientsPool();

        public ElasticsearchRestClient(params string[] elasticsearchUrls)
            : this(elasticsearchUrls.Select(x => new Uri(x)).ToArray())
        {
            
        }

        public ElasticsearchRestClient(params Uri[] elasticsearchUrls)
        {
            foreach (var elasticsearchUrl in elasticsearchUrls)
            {
                var internalClient = new RestClient(elasticsearchUrl.ToString());
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

        public Uri BuildUri(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncGet(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncPost(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncGet<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncPost<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse ExecuteAsGet(IRestRequest request, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse ExecuteAsPost(IRestRequest request, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> ExecuteAsGet<T>(IRestRequest request, string httpMethod) where T : new()
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> ExecuteAsPost<T>(IRestRequest request, string httpMethod) where T : new()
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public CookieContainer CookieContainer { get; set; }
        public string UserAgent { get; set; }
        public int Timeout { get; set; }
        public bool UseSynchronizationContext { get; set; }
        public IAuthenticator Authenticator { get; set; }
        public string BaseUrl { get; set; }
        public IList<Parameter> DefaultParameters { get; private set; }
        public X509CertificateCollection ClientCertificates { get; set; }
        public IWebProxy Proxy { get; set; }

        public string DefaultIndexName { get; set; }                
    }
}
