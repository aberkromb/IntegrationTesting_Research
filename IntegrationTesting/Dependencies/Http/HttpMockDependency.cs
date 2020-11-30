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
        private readonly MountebankClient _mountebankClient;
        private HttpImposter _imposter;
        private MountebankClient _client;

        public HttpMockDependency(HttpMockDependencyContext context, MountebankClient mountebankClient)
        {
            _context = context;
            _mountebankClient = mountebankClient;
            _client = new MountebankClient();
        }
        
        public HttpStub AddGetMock(string path, object response)
        {
            var (_, port) = _context.GetHostAndPort();
            var imposter = _client.CreateHttpImposter(port, $"{path}-mock-", recordRequests: true); 
            
            var stub = imposter.AddStub()
                .OnPathAndMethodEqual(path, Method.Get)
                .ReturnsJson(HttpStatusCode.OK, response);
            
            _client.Submit(imposter);
            
            return stub;
        }
    }
}