using Microsoft.EntityFrameworkCore;
using TmsApi.Domain.Entities;

namespace TmsApi.Infrastructure.Persistence;

public sealed class TmsDbContext(DbContextOptions<TmsDbContext> options) : DbContext(options)
{
	public DbSet<Course> Courses => Set<Course>();
	public DbSet<Enrollment> Enrollments => Set<Enrollment>();
}