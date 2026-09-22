import apiClient from "./client";
import type { ApiResponse, CategoryDto } from "@/types/api";

export const categoriesApi = {
  getAll: async (): Promise<CategoryDto[]> => {
    const response = await apiClient.get<ApiResponse<CategoryDto[]>>("/categories");
    return response.data.data;
  },
};