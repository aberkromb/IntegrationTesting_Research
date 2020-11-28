﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTestingSandbox.DataAccess;
using IntegrationTestingSandbox.DataAccess.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IntegrationTestingSandbox.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiTestController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ApiTestController> _logger;

        // The Web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
        static readonly string[] scopeRequiredByApi = new string[] {"access_as_user"};

        private readonly IDataAccess _dataAccess;

        private readonly IHttpClientFactory _httpClientFactory;

        public ApiTestController(ILogger<ApiTestController> logger, IDataAccess dataAccess, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }

        [HttpGet, Route("postgres")]
        public async Task<string> InsertToPostgres([FromQuery] string toInsert ,CancellationToken cancellationToken)
        {
            await _dataAccess.Add(new Strings {String = toInsert}, cancellationToken);
            var value = (await _dataAccess.Get(cancellationToken)).String;
            return value;
        }

        [HttpGet, Route("google")]
        public async Task<string> PingGoogle(CancellationToken cancellationToken)
        {
            var response = await _httpClientFactory
                .CreateClient("google")
                .GetAsync("http://google.com/search?q=ping", cancellationToken);

            return await response.Content.ReadAsStringAsync();
        }
    }
}