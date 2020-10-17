using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTestingSandbox.DataAccess.DataBase
{
    public class PostgresDataAccess : IDataAccess
    {
        private readonly PostgresDbContext _dbContext;

        public PostgresDataAccess(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Strings> Get(CancellationToken cancellationToken)
        {
            return Task.FromResult(_dbContext.Strings.FirstOrDefault());
        }

        public async Task Add(Strings value, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(value, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public interface IDataAccess
    {
        Task<Strings> Get(CancellationToken cancellationToken);

        Task Add(Strings value, CancellationToken cancellationToken);
    }
}