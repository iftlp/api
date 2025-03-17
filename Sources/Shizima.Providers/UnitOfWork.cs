using Microsoft.EntityFrameworkCore;

namespace Shizima.Providers
{
    public class UnitOfWork
    {
        public DatabaseContext _dbContext;

        public UnitOfWork(DatabaseContext context)
        {
            _dbContext = context;
            _dbContext.Database.EnsureCreated();
        }

        public int SaveChanges()
        {
            int result = 0;
            try
            {
                result = _dbContext.SaveChanges();
            }
            catch
            {
                RejectChanges();
                return -1;
            }
            foreach (var entity in _dbContext.ChangeTracker.Entries()
                         .Where(e => e.State != EntityState.Detached))
            {
                entity.State = EntityState.Detached;
            }
            return result;
        }

        public void RejectChanges()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries()
                         .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}
