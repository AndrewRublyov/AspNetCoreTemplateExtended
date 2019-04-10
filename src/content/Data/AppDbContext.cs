using AspNetCoreTemplateExtended.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTemplateExtended.Data
{
  public class AppDbContext : IdentityDbContext<User>
  {
    // TODO: Add DbSet and Configuration

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
  }
}