import apiClient from "./client";
import type { ApiResponse, CategoryDto } from "@/types/api";

export interface CreateCategoryRequest {
  name: string;
  description?: string;
  imageUrl?: string;
  orderIndex?: number;
}

export const categoriesApi = {
  // FR-CAT-001: Lấy danh sách toàn bộ danh mục
  getAll: async (): Promise<CategoryDto[]> => {
    const response = await apiClient.get<ApiResponse<CategoryDto[]>>("/categories");
    return response.data.data;
  },

  // FR-CAT-003: Admin tạo danh mục mới
  create: async (data: CreateCategoryRequest): Promise<CategoryDto> => {
    const response = await apiClient.post<ApiResponse<CategoryDto>>("/categories", data);
    return response.data.data;
  },
};