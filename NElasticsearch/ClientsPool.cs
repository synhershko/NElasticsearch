using System;
using System.Collections.Concurrent;

namespace NElasticsearch
{
    class ClientsPool<T>
    {
        public class ElasticsearchEndpoint
        {
            public ElasticsearchEndpoint(T client)
            {
                RestClient = client;
            }

            public T RestClient { get; set; }
            public DateTime LatestFailure { get; set; }
            public int NumberOfFailures { get; set; }

            public void ResetFailures()
            {
                NumberOfFailures = 0;
                LatestFailure = DateTime.MinValue;
            }

            public void MarkFailure()
            {
                LatestFailure = DateTime.UtcNow;
                NumberOfFailures++;
            }
        }

        public ClientsPool()
        {
            ShouldUseFailedEndpoint = (numberOfFailures, lastFailure) =>
            {
                if (numberOfFailures > 5)
                    return false;
                if (lastFailure.AddMinutes(1) > DateTime.UtcNow)
                    return false;
                return true;
            };

            ShouldMarkEndpointAsDead = (numberOfFailures, lastFailure) => numberOfFailures > 5;
        }

        public Func<int, DateTime, bool> ShouldUseFailedEndpoint { get; set; }
        public Func<int, DateTime, bool> ShouldMarkEndpointAsDead { get; set; }

        private readonly ConcurrentQueue<ElasticsearchEndpoint> _clients = new ConcurrentQueue<ElasticsearchEndpoint>();
        private readonly ConcurrentQueue<ElasticsearchEndpoint> _deadClients = new ConcurrentQueue<ElasticsearchEndpoint>();

        public void Add(T client)
        {
            _clients.Enqueue(new ElasticsearchEndpoint(client));
        }

        public ElasticsearchEndpoint GetEndpoint()
        {
            var clientsCount = _clients.Count;
            while (!_clients.IsEmpty && clientsCount > 0)
            {
                ElasticsearchEndpoint endpoint;
                if (!_clients.TryDequeue(out endpoint))
                    // TODO try the dead queue
                    throw new ElasticsearchException("No clients found in the list");

                --clientsCount;

                if (endpoint.NumberOfFailures > 0 &&
                    ShouldMarkEndpointAsDead(endpoint.NumberOfFailures, endpoint.LatestFailure))
                {
                    _deadClients.Enqueue(endpoint);
                    continue;
                }

                if (endpoint.NumberOfFailures == 0 || ShouldUseFailedEndpoint(endpoint.NumberOfFailures, endpoint.LatestFailure))
                    return endpoint;

                _clients.Enqueue(endpoint);
            }
            throw new ElasticsearchException("No available Elasticsearch endpoints found");
        }

        public void ReleaseEndpoint(ElasticsearchEndpoint endpoint)
        {
            _clients.Enqueue(endpoint);
        }
    }
}
