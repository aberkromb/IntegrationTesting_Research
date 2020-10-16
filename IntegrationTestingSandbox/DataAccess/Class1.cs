﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTestingSandbox.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly TestDbContext _dbContext;

        public DataAccess(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> Get(CancellationToken cancellationToken)
        {
            return  _dbContext.Strings.FirstOrDefault()?.String;
        }
    }

    public interface IDataAccess
    {
        Task<string> Get(CancellationToken cancellationToken);
    }
}