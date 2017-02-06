namespace TTools.EF
{
    using System.Data.Entity;
    using TTools.Models;

    public class TechnoDB : DbContext
    {
        public TechnoDB()
            : base("name=TechnoDB")
        {
        }

        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<ProductItem> ProductItems { get; set; }
    }
}