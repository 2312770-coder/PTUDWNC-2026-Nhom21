import apiClient from "./client";
import type { ApiResponse, CategoryDto } from "@/types/api";

export const FALLBACK_CATEGORIES: CategoryDto[] = [
  {
    id: "abe95eb8-8a4c-42cc-a12f-0a76085caa13",
    name: "Món Chính",
    slug: "mon-chinh",
    description: "Những món ăn chính thơm ngon, đậm đà cho bữa cơm gia đình và tiệc tùng.",
    imageUrl: "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=800&q=80",
    orderIndex: 1,
    recipeCount: 2,
  },
  {
    id: "a973e76c-13b5-4eb8-b7d8-8c593bc325ab",
    name: "Món Nước & Canh",
    slug: "mon-nuoc-canh",
    description: "Các món bún, phở, miến và canh thanh mát giải nhiệt, ấm lòng ngày mưa.",
    imageUrl: "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?auto=format&fit=crop&w=800&q=80",
    orderIndex: 2,
    recipeCount: 1,
  },
  {
    id: "3576fd60-b377-415a-a954-7e4756c602e9",
    name: "Khai Vị & Ăn Vặt",
    slug: "khai-vi-an-vat",
    description: "Món ăn nhẹ, gỏi cuốn, bánh mặn bắt vị kích thích vị giác đầu bữa ăn.",
    imageUrl: "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=800&q=80",
    orderIndex: 3,
    recipeCount: 1,
  },
  {
    id: "e63c5c32-0478-47aa-a4e1-168b0e5f094c",
    name: "Món Tráng Miệng",
    slug: "mon-trang-mieng",
    description: "Các món chè, bánh ngọt, trái cây và kem ngọt ngào kết thúc bữa ăn trọn vẹn.",
    imageUrl: "https://images.unsplash.com/photo-1551024709-8f23befc6f87?auto=format&fit=crop&w=800&q=80",
    orderIndex: 4,
    recipeCount: 1,
  },
  {
    id: "7cb04861-c1dc-456a-b839-9f027e07e04c",
    name: "Món Chay",
    slug: "mon-chay",
    description: "Ẩm thực thuần chay thanh tịnh, dinh dưỡng cân bằng và tốt cho sức khỏe.",
    imageUrl: "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=800&q=80",
    orderIndex: 5,
    recipeCount: 1,
  },
  {
    id: "aa7ffced-c65c-4238-a337-5b6460cac05f",
    name: "Đồ Uống & Giải Khát",
    slug: "do-uong-giai-khat",
    description: "Trà thảo mộc, nước ép hoa quả, sinh tố và đồ uống mát lạnh sảng khoái.",
    imageUrl: "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?auto=format&fit=crop&w=800&q=80",
    orderIndex: 6,
    recipeCount: 0,
  },
];

export const categoriesApi = {
  // FR-CAT-001: GET /api/v1/categories
  getAll: async (): Promise<CategoryDto[]> => {
    try {
      const res = await apiClient.get<ApiResponse<CategoryDto[]>>("/api/v1/categories");
      return res.data.data && res.data.data.length > 0 ? res.data.data : FALLBACK_CATEGORIES;
    } catch {
      return FALLBACK_CATEGORIES;
    }
  },
};
