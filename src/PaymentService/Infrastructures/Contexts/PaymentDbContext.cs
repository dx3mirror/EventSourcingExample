using Microsoft.EntityFrameworkCore;

namespace PaymentService.Infrastructures.Contexts
{
    public sealed class PaymentDbContext(DbContextOptions<PaymentDbContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            CustomModelBuilder.OnModelCreating(modelBuilder);
        }
    }
}
