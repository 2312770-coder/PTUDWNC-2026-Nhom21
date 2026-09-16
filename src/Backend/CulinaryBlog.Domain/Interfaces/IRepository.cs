using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Interfaces;

// SRS mục 6.2 - interface repository đặt ở tầng Domain, implementation nằm ở
// Infrastructure (Dependency Inversion: tầng trong định nghĩa hợp đồng).
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    IQueryable<T> Query();
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);           // hard delete
    void SoftDelete(T entity);       // set IsDeleted = true
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
