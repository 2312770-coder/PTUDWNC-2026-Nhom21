using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Infrastructure.Persistence.Repositories;

public class RecipeRepository : Repository<Recipe>, IRecipeRepository
{
    public RecipeRepository(CulinaryBlogDbContext db) : base(db) { }

    // AsSplitQuery: Recipe có 3 collection (Steps/Ingredients/Images), nếu JOIN
    // hết trong 1 câu SQL sẽ sinh ra tích Descartes rất nhiều dòng thừa.
    // AsSplitQuery tách thành nhiều câu SQL nhỏ, tổng dữ liệu ít hơn hẳn.
    public async Task<Recipe?> GetDetailBySlugAsync(string slug, CancellationToken ct = default)
        => await BuildDetailQuery().FirstOrDefaultAsync(r => r.Slug == slug, ct);

    public async Task<Recipe?> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
        => await BuildDetailQuery().FirstOrDefaultAsync(r => r.Id == id, ct);

    private IQueryable<Recipe> BuildDetailQuery()
        => Db.Recipes
            .AsSplitQuery()
            .Include(r => r.Category)
            .Include(r => r.Author)
            .Include(r => r.Steps.OrderBy(s => s.StepNumber))
            .Include(r => r.Ingredients.OrderBy(i => i.OrderIndex))
            .Include(r => r.Images.OrderBy(i => i.OrderIndex));

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
        => await Db.Recipes.AnyAsync(r => r.Slug == slug, ct);

    // FR-SRCH-001 - CHƯA HIỆN THỰC, dành cho người phụ trách FR-SRCH-001.
    //
    // Gợi ý:
    //   1. Trước tiên phải thêm cột SearchVector + GIN index vào
    //      RecipeConfiguration (xem TODO trong file đó) rồi tạo migration mới.
    //   2. Bật extension trong PostgreSQL: CREATE EXTENSION IF NOT EXISTS unaccent;
    //   3. Truy vấn:
    //        var tsQuery = EF.Functions.PlainToTsQuery("simple", keyword);
    //        query.Where(r => r.SearchVector.Matches(tsQuery))
    //             .OrderByDescending(r => r.SearchVector.Rank(tsQuery))
    //   4. Chỉ tìm trong recipe Status = Published.
    //   5. Trả về cả danh sách lẫn tổng số kết quả để phục vụ phân trang.
    public Task<(IReadOnlyList<Recipe> Items, int Total)> SearchAsync(
        string keyword, int page, int pageSize, CancellationToken ct = default)
        => throw new NotImplementedException(
            "FR-SRCH-001 (Full-Text Search) chưa được hiện thực trong RecipeRepository.");
}
