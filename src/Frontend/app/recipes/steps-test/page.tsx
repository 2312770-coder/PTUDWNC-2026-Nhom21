// ============================================================================
// TRANG KIỂM THỬ: /recipes/steps-test
// CHỨC NĂNG: Kiểm thử trực tiếp tính năng FR-RCP-010 (Quản lý các bước nấu ăn)
// THÀNH VIÊN: Nguyễn Đình Tuấn (MSSV: 2312792)
// ============================================================================

'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import StepListEditor from '@/components/recipes/StepListEditor';
import { recipesApi } from '@/lib/api/recipes';
import type { RecipeListItemDto, RecipeStepDto } from '@/types/api';

export default function RecipeStepsTestPage() {
  // 1. Danh sách các công thức mẫu tải từ Database
  const [recipes, setRecipes] = useState<RecipeListItemDto[]>([]);
  const [selectedRecipeId, setSelectedRecipeId] = useState<string>('');
  const [currentSteps, setCurrentSteps] = useState<RecipeStepDto[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isLoadingSteps, setIsLoadingSteps] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // 2. Tải danh sách công thức khi mở trang
  useEffect(() => {
    async function loadRecipes() {
      setIsLoading(true);
      setError(null);
      try {
        const result = await recipesApi.getAll({ pageSize: 20 });
        setRecipes(result.items);
        if (result.items.length > 0) {
          // Mặc định chọn công thức đầu tiên
          setSelectedRecipeId(result.items[0].id);
        }
      } catch (err: unknown) {
        const msg = err instanceof Error ? err.message : 'Không thể kết nối API công thức.';
        setError(msg);
      } finally {
        setIsLoading(false);
      }
    }
    loadRecipes();
  }, []);

  // 3. Tải danh sách các bước nấu khi công thức được chọn thay đổi
  useEffect(() => {
    if (!selectedRecipeId) return;

    async function loadSteps() {
      setIsLoadingSteps(true);
      try {
        const steps = await recipesApi.getSteps(selectedRecipeId);
        setCurrentSteps(steps);
      } catch (err: unknown) {
        console.error('Lỗi khi tải các bước nấu:', err);
      } finally {
        setIsLoadingSteps(false);
      }
    }
    loadSteps();
  }, [selectedRecipeId]);

  const selectedRecipe = recipes.find((r) => r.id === selectedRecipeId);

  return (
    <div className="min-h-screen bg-neutral-50 py-10">
      <div className="max-w-5xl mx-auto px-4 sm:px-6">
        {/* Breadcrumb và tiêu đề trang */}
        <div className="mb-8">
          <div className="flex items-center gap-2 text-sm text-neutral-500 mb-2">
            <Link href="/" className="hover:text-orange-600 transition-colors">Trang chủ</Link>
            <span>/</span>
            <span className="text-neutral-800 font-medium">Kiểm thử quản lý bước nấu (FR-RCP-010)</span>
          </div>

          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
            <div>
              <h1 className="text-3xl font-extrabold text-neutral-900 tracking-tight">
                Quản lý các bước nấu ăn
              </h1>
              <p className="text-sm text-neutral-600 mt-1">
                Kiểm thử các thao tác Thêm, Sửa, Xóa, Tải ảnh minh họa MinIO và tự động đánh số thứ tự (D9).
              </p>
            </div>

            <Link
              href="http://localhost:5000/scalar/v1"
              target="_blank"
              className="inline-flex items-center gap-2 px-3.5 py-2 bg-neutral-900 hover:bg-neutral-800 text-white rounded-xl text-xs font-semibold shadow-xs"
            >
              <span>📑</span> Mở Scalar API Docs
            </Link>
          </div>
        </div>

        {/* Thông báo lỗi nếu có */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
            ⚠️ {error}
          </div>
        )}

        {/* Khung chọn công thức mẫu để kiểm thử */}
        <div className="bg-white rounded-2xl border border-neutral-200 p-5 mb-8 shadow-xs">
          <label className="block text-xs font-bold text-neutral-700 uppercase tracking-wider mb-2">
            📌 Chọn công thức để xem và quản lý bước nấu:
          </label>

          {isLoading ? (
            <div className="py-4 text-center text-sm text-neutral-500">Đang tải danh sách công thức từ CSDL...</div>
          ) : recipes.length === 0 ? (
            <div className="py-4 text-center text-sm text-amber-600">
              Chưa có công thức nào trong CSDL. Hãy đảm bảo Docker và Database Seeder đã chạy!
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
              {recipes.map((recipe) => {
                const isSelected = recipe.id === selectedRecipeId;
                return (
                  <button
                    key={recipe.id}
                    type="button"
                    onClick={() => setSelectedRecipeId(recipe.id)}
                    className={`text-left p-3.5 rounded-xl border transition-all ${
                      isSelected
                        ? 'border-orange-500 bg-orange-50/60 ring-2 ring-orange-400'
                        : 'border-neutral-200 hover:border-neutral-300 bg-white'
                    }`}
                  >
                    <p className="font-bold text-neutral-900 text-sm truncate">{recipe.title}</p>
                    <p className="text-xs text-neutral-500 mt-1 flex items-center gap-2">
                      <span>🏷️ {recipe.categoryName}</span>
                      <span>•</span>
                      <span>⏱️ {recipe.cookTime}p</span>
                    </p>
                  </button>
                );
              })}
            </div>
          )}
        </div>

        {/* Phần Component quản lý các bước nấu (StepListEditor) */}
        {selectedRecipeId && (
          <div className="space-y-4">
            {selectedRecipe && (
              <div className="bg-neutral-900 text-white rounded-2xl p-5 shadow-sm flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                <div>
                  <span className="text-xs uppercase tracking-wider text-orange-400 font-bold">
                    Công thức đang chọn
                  </span>
                  <h2 className="text-xl font-bold mt-0.5">{selectedRecipe.title}</h2>
                  <p className="text-xs text-neutral-300 mt-1">{selectedRecipe.description}</p>
                </div>
                <div className="text-right text-xs text-neutral-400">
                  <p>Mã ID: <code className="text-neutral-200">{selectedRecipe.id}</code></p>
                </div>
              </div>
            )}

            {isLoadingSteps ? (
              <div className="p-12 text-center text-neutral-500 bg-white rounded-2xl border border-neutral-200">
                Đang tải các bước nấu...
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
