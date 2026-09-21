// Frontend API module cho Categories.
// TODO: Người phụ trách FR-CAT sẽ hiện thực các hàm gọi API tại đây.

import apiClient from "./client";
import type { ApiResponse, CategoryDto } from "@/types/api";

// Placeholder – các thành viên sẽ hiện thực khi hoàn tất Backend endpoint tương ứng.
export const categoriesApi = {
  // FR-CAT-001: GET /api/v1/categories
  getAll: async (): Promise<CategoryDto[]> => {
    // TODO: Gọi API thực tế khi Backend endpoint sẵn sàng.
    return [];
  },
};
