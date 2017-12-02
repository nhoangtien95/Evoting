namespace Evoting.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Evoting : DbContext
    {
        public Evoting()
            : base("name=Evoting")
        {
        }

        public virtual DbSet<Citizen> Citizens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Citizen>()
                .Property(e => e.ID)
                .IsUnicode(false);
        }
    }
}
