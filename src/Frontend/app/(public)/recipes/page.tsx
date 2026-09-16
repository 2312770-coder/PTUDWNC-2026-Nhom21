// Trang danh sách công thức - CHƯA HIỆN THỰC.
// Gợi ý cho người làm FR-RCP-001: gọi GET /api/v1/recipes qua TanStack Query,
// render danh sách bằng component RecipeCard, thêm bộ lọc theo danh mục/độ khó.
// Dùng ISR (revalidate) để tối ưu SEO theo NFR-SEO.
export const revalidate = 3600;

export const metadata = { title: "Công thức nấu ăn" };

export default function RecipesPage() {
  return (
    <main className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8">Công thức nấu ăn</h1>
      <p className="text-gray-500">Danh sách công thức sẽ hiển thị ở đây.</p>
    </main>
  );
}
