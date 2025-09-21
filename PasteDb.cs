// PasteDb.cs
using Microsoft.EntityFrameworkCore;

public class PasteDb : DbContext
{
    public PasteDb(DbContextOptions<PasteDb> options) : base(options) { }

    public DbSet<Paste> Pastes { get; set; }
}