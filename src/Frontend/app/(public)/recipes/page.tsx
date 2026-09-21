import { categoriesApi } from "@/lib/api/categories";
import Image from "next/image";
import Link from "next/link";

export default async function CategoriesPage() {
  // Lấy dữ liệu danh mục từ Backend (đã cấu hình api ở lib/api/categories.ts)
  const categories = await categoriesApi.getAll();

  return (
    <div className="container mx-auto py-10 px-4">
      <h1 className="text-3xl font-bold mb-8 text-center text-gray-800">
        Khám Phá Danh Mục Ẩm Thực
      </h1>
      
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {categories.map((category) => (
          <Link href={`/categories/${category.slug}`} key={category.id} className="block group">
            <div className="border border-gray-200 rounded-xl overflow-hidden shadow-sm hover:shadow-lg transition-all bg-white h-full flex flex-col">
              {/* Hình ảnh danh mục */}
              {category.imageUrl ? (
                <div className="relative h-48 w-full bg-gray-100 overflow-hidden">
                  <Image 
                    src={category.imageUrl} 
                    alt={category.name} 
                    fill 
                    className="object-cover group-hover:scale-105 transition-transform duration-500"
                    sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
                  />
                </div>
              ) : (
                <div className="h-48 w-full bg-gray-200 flex items-center justify-center">
                  <span className="text-gray-400">Chưa có ảnh</span>
                </div>
              )}
              
              {/* Nội dung text */}
              <div className="p-5 flex flex-col flex-grow">
                <h2 className="text-xl font-bold mb-2 group-hover:text-orange-500 transition-colors">
                  {category.name}
                </h2>
                <p className="text-gray-600 text-sm mb-4 line-clamp-2 flex-grow">
                  {category.description || "Chưa có mô tả cho danh mục này."}
                </p>
                
                {/* Số lượng công thức đã Publish (recipeCount) */}
                <div className="mt-auto">
                  <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-orange-100 text-orange-800">
                    {category.recipeCount} công thức
                  </span>
                </div>
              </div>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}