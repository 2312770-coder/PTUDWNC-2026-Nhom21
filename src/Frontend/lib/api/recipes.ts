// Frontend API module cho Recipes.
// TODO: Người phụ trách FR-RCP sẽ hiện thực các hàm gọi API tại đây.

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import apiClient from "./client";
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import type { ApiResponse, RecipeListItemDto, RecipeDetailDto, RecipeDifficulty } from "@/types/api";

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
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  getAll: async (_params?: GetRecipesParams): Promise<{ items: RecipeListItemDto[]; total: number }> => {
    // TODO: Gọi API thực tế khi Backend endpoint sẵn sàng.
    return { items: [], total: 0 };
  },

  // FR-RCP-002: GET /api/v1/recipes/{slug}
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  getBySlug: async (_slug: string): Promise<RecipeDetailDto | null> => {
    // TODO: Gọi API thực tế khi Backend endpoint sẵn sàng.
    return null;
  },
};
