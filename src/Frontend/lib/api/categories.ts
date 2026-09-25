import apiClient from "./client";
import type { ApiResponse, CategoryDto, CategoryDetailDto } from "@/types/api";

export const categoriesApi = {
  getAll: async (): Promise<CategoryDto[]> => {
    try {
      const response = await apiClient.get<ApiResponse<CategoryDto[]>>("/categories");
      return response.data.data;
    } catch {
      return [];
    }
  },

  getBySlug: async (slug: string): Promise<CategoryDetailDto> => {
    const response = await apiClient.get<ApiResponse<CategoryDetailDto>>(`/categories/${slug}`);
    return response.data.data;
  },
};