// Ví dụ mẫu cách gọi API - dùng làm khuôn cho các file api khác
// (recipes.ts, auth.ts...). Backend bọc response trong { data }, nên luôn
// nhớ lấy res.data.data chứ không phải res.data.
import apiClient from "./client";
import type { ApiResponse, CategoryDto } from "@/types/api";

export const categoriesApi = {
  // FR-CAT-001: GET /api/v1/categories
  getAll: async (): Promise<CategoryDto[]> => {
    const res = await apiClient.get<ApiResponse<CategoryDto[]>>("/api/v1/categories");
    return res.data.data;
  },
};
