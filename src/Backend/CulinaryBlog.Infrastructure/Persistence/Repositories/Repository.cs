using CulinaryBlog.Domain.Common;
using CulinaryBlog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Infrastructure.Persistence.Repositories;

// Repository dùng chung cho mọi entity kế thừa BaseEntity.
// Các repository cụ thể (RecipeRepository, CategoryRepository) kế thừa class
// này nên không phải viết lại các thao tác CRUD cơ bản.
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly CulinaryBlogDbContext Db;
    protected readonly DbSet<T> DbSet;

    public Repository(CulinaryBlogDbContext db)
    {
        Db = db;
        DbSet = db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await DbSet.AsNoTracking().ToListAsync(ct);

    // Trả IQueryable để Handler tự chain thêm Where/OrderBy/Include.
    // Global Query Filter (IsDeleted = false) đã được áp dụng sẵn.
    public IQueryable<T> Query() => DbSet.AsQueryable();

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await DbSet.AddAsync(entity, ct);

    public void Update(T entity) => DbSet.Update(entity);

    // Xóa vĩnh viễn khỏi database.
    public void Remove(T entity) => DbSet.Remove(entity);

    // Xóa mềm: chỉ đánh dấu IsDeleted = true, bản ghi vẫn còn trong DB
    // nhưng bị Global Query Filter lọc ra khỏi mọi truy vấn thông thường.
    public void SoftDelete(T entity)
    {
        entity.MarkDeleted();
        DbSet.Update(entity);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await DbSet.AnyAsync(e => e.Id == id, ct);

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await DbSet.CountAsync(ct);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await Db.SaveChangesAsync(ct);
}
