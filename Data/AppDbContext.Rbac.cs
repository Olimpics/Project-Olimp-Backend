using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;

namespace OlimpBack.Data;

public partial class AppDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<RolePermission>();
        modelBuilder.Ignore<UserRole>();
    }
}
