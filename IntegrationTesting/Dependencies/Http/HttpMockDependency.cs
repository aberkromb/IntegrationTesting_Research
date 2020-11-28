using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Imposters;
using MbDotNet.Models.Stubs;

namespace IntegrationTesting.Dependencies.Http
{
    public class HttpMockDependency : IDependency
    {
        private readonly HttpMockDependencyContext _context;
        private HttpImposter _imposter;
        private MountebankClient _client;

        public HttpMockDependency(HttpMockDependencyContext context)
        {
            _context = context;
            _client = new MountebankClient();
        }
        
        public Task<Requests> GetRequests()
        {
            throw new NotImplementedException();
        }

        public Task AddMock()
        {
            throw new NotImplementedException();
        }

        public HttpStub AddGetMock(string query, object response)
        {
            var (host, port) = _context.GetHostAndPort();
            var imposter = _client.CreateHttpImposter(port, "integration-tests", recordRequests: true); 
            
            var stub = imposter.AddStub()
                .OnPathAndMethodEqual(query, Method.Get)
                .ReturnsJson(HttpStatusCode.OK, response);
            
            _client.Submit(imposter);
            
            return stub;
        }
    }

    public class Requests
    {
    }
}