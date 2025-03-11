namespace CodingTest.Data;

public class DbCoding(DbContextOptions<DbCoding> o) : DbContext(o)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnConfiguring(DbContextOptionsBuilder o)
    {
        o.UseSqlite("Data Source=CodingTest.db;Cache=Shared");
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.FirstName).HasColumnType("varchar(50)").HasColumnName("first_name");
            entity.Property(u => u.LastName).HasColumnType("varchar(50)").HasColumnName("last_name");
            entity.Property(o => o.Age).HasColumnType("int").HasColumnName("age");
        });
    }
}
