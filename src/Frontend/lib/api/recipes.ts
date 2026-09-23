// Frontend API module cho Recipes.
// TODO: Người phụ trách FR-RCP sẽ hiện thực các hàm gọi API tại đây.

import type { RecipeListItemDto, RecipeDetailDto, RecipeDifficulty } from "@/types/api";

export interface GetRecipesParams {
  page?: number;
  pageSize?: number;
  categoryId?: string;
  difficulty?: RecipeDifficulty;
  maxCookTime?: number;
  maxTotalTime?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  sort?: string;
}

// Placeholder – các thành viên sẽ hiện thực khi hoàn tất Backend endpoint tương ứng.
export const recipesApi = {
  // FR-RCP-001: GET /api/v1/recipes
  getAll: async (params?: GetRecipesParams): Promise<{ items: RecipeListItemDto[]; total: number }> => {
    void params;
    // TODO: Gọi API thực tế khi Backend endpoint sẵn sàng.
    return { items: [], total: 0 };
  },

  // FR-RCP-002: GET /api/v1/recipes/{slug}
  getBySlug: async (slug: string): Promise<RecipeDetailDto | null> => {
    void slug;
    // TODO: Gọi API thực tế khi Backend endpoint sẵn sàng.
    return null;
  },
};
