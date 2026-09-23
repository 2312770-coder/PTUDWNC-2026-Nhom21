// ============================================================================
// TRANG QUẢN LÝ QUY TRÌNH CHẾ BIẾN MÓN ĂN
// Chức năng: Quản lý chi tiết từng bước nấu ăn cho các món trong hệ thống
// Tác giả: Nguyễn Đình Tuấn
// ============================================================================

'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import StepListEditor from '@/components/recipes/StepListEditor';
import { recipesApi } from '@/lib/api/recipes';
import type { RecipeListItemDto, RecipeStepDto } from '@/types/api';

export default function RecipeStepsPage() {
  // Danh sách các công thức món ăn tải từ máy chủ
  const [recipes, setRecipes] = useState<RecipeListItemDto[]>([]);
  // ID món ăn hiện đang được chọn để quản lý bước nấu
  const [selectedRecipeId, setSelectedRecipeId] = useState<string>('');
  // Danh sách các bước nấu của món ăn đang chọn
  const [currentSteps, setCurrentSteps] = useState<RecipeStepDto[]>([]);
  // Từ khóa tìm kiếm món ăn trong danh sách
  const [searchQuery, setSearchQuery] = useState<string>('');
  // Trạng thái đang tải danh sách món ăn
  const [isLoading, setIsLoading] = useState<boolean>(true);
  // Trạng thái đang tải các bước nấu của món ăn đã chọn
  const [isLoadingSteps, setIsLoadingSteps] = useState<boolean>(false);
  // Thông báo lỗi nếu xảy ra sự cố kết nối máy chủ
  const [error, setError] = useState<string | null>(null);

  // 1. Tải danh sách món ăn khi khởi chạy trang
  useEffect(() => {
    async function loadRecipes() {
      setIsLoading(true);
      setError(null);
      try {
        const result = await recipesApi.getAll({ pageSize: 50 });
        setRecipes(result.items);
        if (result.items.length > 0) {
          // Mặc định chọn món ăn đầu tiên để hiển thị ngay
          setSelectedRecipeId(result.items[0].id);
        }
      } catch (err: unknown) {
        const msg = err instanceof Error ? err.message : 'Không thể kết nối đến máy chủ dữ liệu.';
        setError(msg);
      } finally {
        setIsLoading(false);
      }
    }
    loadRecipes();
  }, []);

  // 2. Tải danh sách các bước nấu mỗi khi chọn món ăn khác
  useEffect(() => {
    if (!selectedRecipeId) return;

    async function loadSteps() {
      setIsLoadingSteps(true);
      try {
        const steps = await recipesApi.getSteps(selectedRecipeId);
        setCurrentSteps(steps);
      } catch (err: unknown) {
        console.error('Lỗi khi tải các bước thực hiện:', err);
      } finally {
        setIsLoadingSteps(false);
      }
    }
    loadSteps();
  }, [selectedRecipeId]);

  // Tìm thông tin chi tiết của món ăn đang được chọn
  const selectedRecipe = recipes.find((r) => r.id === selectedRecipeId);

  // Lọc danh sách món ăn theo ô tìm kiếm
  const filteredRecipes = recipes.filter((r) =>
    r.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
    r.categoryName?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="min-h-screen bg-[#FBF9F7] py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-6xl mx-auto space-y-6">

        {/* Thanh điều hướng Breadcrumb phong cách ẩm thực */}
        <nav aria-label="Breadcrumb" className="flex items-center gap-2 text-sm text-neutral-500">
          <Link href="/" className="hover:text-orange-600 transition font-medium">Trang chủ</Link>
          <span>/</span>
          <Link href="/recipes" className="hover:text-orange-600 transition font-medium">Khám phá công thức</Link>
          <span>/</span>
          <span className="text-neutral-900 font-semibold">Quy trình & Các bước nấu nướng</span>
        </nav>

        {/* Tiêu đề chính trang web */}
        <div className="bg-white rounded-3xl p-6 sm:p-8 border border-neutral-200/70 shadow-sm flex flex-col md:flex-row md:items-center justify-between gap-6">
          <div className="space-y-2">
            <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-orange-100 text-orange-800 text-xs font-bold tracking-wide uppercase">
              <span>👨‍🍳</span> Không gian ẩm thực
            </div>
            <h1 className="text-2xl sm:text-3xl font-extrabold text-neutral-900 tracking-tight">
              Quản Lý Quy Trình Chế Biến Món Ăn
            </h1>
            <p className="text-sm text-neutral-600 max-w-2xl leading-relaxed">
              Thiết lập chi tiết từng bước nấu ăn, thời gian hẹn giờ và hình ảnh trực quan giúp người nấu dễ dàng thực hiện thành công từng món ăn ngon.
            </p>
          </div>

          <div className="flex items-center gap-3">
            <Link
              href="/dashboard/recipes/new"
              className="inline-flex items-center gap-2 px-4 py-2.5 bg-orange-600 hover:bg-orange-700 text-white rounded-2xl text-sm font-semibold transition shadow-sm"
            >
              <span>+</span> Tạo món mới
            </Link>
          </div>
        </div>

        {/* Thông báo lỗi kết nối nếu có */}
        {error && (
          <div className="p-4 bg-red-50 border border-red-200 rounded-2xl text-red-700 text-sm flex items-center justify-between">
            <div className="flex items-center gap-2">
              <span>⚠️</span>
              <span>{error}</span>
            </div>
            <button
              onClick={() => window.location.reload()}
              className="text-xs font-semibold underline hover:no-underline"
            >
              Tải lại trang
            </button>
          </div>
        )}

        {/* Khung lựa chọn món ăn */}
        <div className="bg-white rounded-3xl border border-neutral-200/70 p-6 shadow-sm space-y-4">
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 pb-3 border-b border-neutral-100">
            <div>
              <h2 className="text-base font-bold text-neutral-900 flex items-center gap-2">
                <span>🍲</span> Chọn món ăn để biên soạn bước nấu:
              </h2>
              <p className="text-xs text-neutral-500 mt-0.5">
                Nhấn vào từng món ăn bên dưới để xem và tinh chỉnh các bước thực hiện.
              </p>
            </div>

            {/* Ô tìm kiếm món ăn nhanh */}
            <div className="w-full sm:w-64">
              <input
                type="text"
                placeholder="Tìm món ăn hoặc danh mục..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full px-3.5 py-1.5 text-xs bg-neutral-50 border border-neutral-200 rounded-xl outline-none focus:border-orange-500 focus:bg-white transition"
              />
            </div>
          </div>

          {isLoading ? (
            <div className="py-8 text-center text-sm text-neutral-500 animate-pulse">
              Đang tải danh sách các món ăn hấp dẫn...
            </div>
          ) : recipes.length === 0 ? (
            <div className="py-8 text-center text-sm text-neutral-500">
              Hiện chưa có món ăn nào trong thực đơn.
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3.5 max-h-64 overflow-y-auto pr-1">
              {filteredRecipes.map((recipe) => {
                const isSelected = recipe.id === selectedRecipeId;
                return (
                  <button
                    key={recipe.id}
                    type="button"
                    onClick={() => setSelectedRecipeId(recipe.id)}
                    className={`text-left p-3.5 rounded-2xl border transition-all ${
                      isSelected
                        ? 'border-orange-500 bg-orange-50/70 ring-2 ring-orange-400/40 shadow-xs'
                        : 'border-neutral-200/80 hover:border-orange-200 bg-white hover:bg-neutral-50/50'
                    }`}
                  >
                    <div className="flex items-center justify-between gap-2">
                      <p className="font-bold text-neutral-900 text-sm truncate">{recipe.title}</p>
                      {isSelected && (
                        <span className="text-[10px] font-bold px-2 py-0.5 rounded-full bg-orange-600 text-white flex-shrink-0">
                          Đang chọn
                        </span>
                      )}
                    </div>
                    <div className="text-xs text-neutral-500 mt-1.5 flex items-center gap-2">
                      <span className="inline-flex items-center gap-1 font-medium text-orange-700 bg-orange-100/60 px-2 py-0.5 rounded-lg text-[11px]">
                        🏷️ {recipe.categoryName || 'Món truyền thống'}
                      </span>
                      <span>•</span>
                      <span>⏱️ {recipe.cookTime || 30} phút nấu</span>
                    </div>
                  </button>
                );
              })}
            </div>
          )}
        </div>

        {/* Khung biên tập các bước nấu của món ăn đang chọn */}
        {selectedRecipeId && selectedRecipe && (
          <div className="space-y-5">
            {/* Thẻ thông tin món ăn đang biên tập */}
            <div className="bg-gradient-to-r from-neutral-900 via-neutral-800 to-neutral-900 text-white rounded-3xl p-6 shadow-md flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
              <div className="space-y-1">
                <div className="flex items-center gap-2 text-xs text-orange-400 font-bold uppercase tracking-wider">
                  <span>✨</span> Món ăn đang tinh chỉnh quy trình
                </div>
                <h2 className="text-xl sm:text-2xl font-bold tracking-tight">{selectedRecipe.title}</h2>
                <p className="text-xs sm:text-sm text-neutral-300 max-w-2xl line-clamp-2">
                  {selectedRecipe.description || 'Món ăn đậm đà hương vị truyền thống thơm ngon khó cưỡng.'}
                </p>
              </div>

              <div className="flex items-center gap-4 bg-white/10 px-4 py-2.5 rounded-2xl backdrop-blur-xs text-xs text-neutral-200">
                <div>
                  <p className="text-[10px] text-neutral-400 uppercase font-semibold">Khẩu phần</p>
                  <p className="font-bold text-white text-sm">{selectedRecipe.servings || 4} người</p>
                </div>
                <div className="w-px h-6 bg-white/20" />
                <div>
                  <p className="text-[10px] text-neutral-400 uppercase font-semibold">Chuẩn bị</p>
                  <p className="font-bold text-white text-sm">{selectedRecipe.prepTime || 15}p</p>
                </div>
                <div className="w-px h-6 bg-white/20" />
                <div>
                  <p className="text-[10px] text-neutral-400 uppercase font-semibold">Nấu</p>
                  <p className="font-bold text-white text-sm">{selectedRecipe.cookTime || 30}p</p>
                </div>
              </div>
            </div>

            {/* Component quản lý các bước nấu (StepListEditor) */}
            {isLoadingSteps ? (
              <div className="p-12 text-center text-neutral-500 bg-white rounded-3xl border border-neutral-200/80 animate-pulse">
                Đang chuẩn bị danh sách các bước nấu ăn...
              </div>
            ) : (
              <StepListEditor
                key={selectedRecipeId}
                recipeId={selectedRecipeId}
                initialSteps={currentSteps}
                onStepsChange={(newSteps) => setCurrentSteps(newSteps)}
              />
            )}
          </div>
        )}
      </div>
    </div>
  );
}
