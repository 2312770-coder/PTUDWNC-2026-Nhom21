import apiClient from "./client";
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

// Fallback data dùng khi Backend chưa khởi động để UI luôn hiển thị đẹp mắt
export const FALLBACK_RECIPES: RecipeListItemDto[] = [
  {
    id: "91cbf233-2335-4502-a990-5b2d35dbb690",
    title: "Phở Bò Tái Lăn Hà Nội",
    slug: "pho-bo-tai-lan-ha-noi",
    description: "Món phở bò tái lăn chuẩn vị phố cổ với nước dùng trong veo, thơm nức mùi gừng nướng, quế hồi cùng thịt bò xào lửa lớn mềm ngọt.",
    prepTime: 30,
    cookTime: 120,
    servings: 4,
    difficulty: "Medium",
    status: "Published",
    primaryImageUrl: "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?auto=format&fit=crop&w=1200&q=80",
    categoryName: "Món Nước & Canh",
    authorDisplayName: "Bếp Trưởng An",
    publishedAt: new Date().toISOString(),
  },
  {
    id: "a30fb9ae-8640-42e3-b8a4-4dade5574626",
    title: "Cơm Tấm Sườn Bì Chả Sài Gòn",
    slug: "com-tam-suon-bi-cha-sai-gon",
    description: "Đĩa cơm tấm chuẩn vị Sài Gòn với sườn nướng mỡ hành óng ánh, chả trứng bùi béo, bì dai giòn cùng nước mắm chua ngọt đậm đà.",
    prepTime: 45,
    cookTime: 40,
    servings: 4,
    difficulty: "Medium",
    status: "Published",
    primaryImageUrl: "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=1200&q=80",
    categoryName: "Món Chính",
    authorDisplayName: "Bếp Trưởng An",
    publishedAt: new Date().toISOString(),
  },
  {
    id: "ebe90cda-0fcf-4340-8127-29394f6c621d",
    title: "Bánh Xèo Giòn Rụm Miền Tây",
    slug: "banh-xeo-gion-rum-mien-tay",
    description: "Vỏ bánh xèo mỏng tang, vàng ươm nghệ và cốt dừa thơm béo, nhân tôm thịt tươi ngon cuốn cùng cả rổ rau rừng thanh mát.",
    prepTime: 30,
    cookTime: 30,
    servings: 4,
    difficulty: "Easy",
    status: "Published",
    primaryImageUrl: "https://images.unsplash.com/photo-1559847844-5315695dadae?auto=format&fit=crop&w=1200&q=80",
    categoryName: "Món Chính",
    authorDisplayName: "Bếp Trưởng An",
    publishedAt: new Date().toISOString(),
  },
  {
    id: "00501794-1c51-4ab0-bff3-5181704a0839",
    title: "Gỏi Cuốn Tôm Thịt Thanh Mát",
    slug: "goi-cuon-tom-thit-thanh-mat",
    description: "Món khai vị trứ danh tươi mát với tôm đỏ au, thịt luộc thái mỏng, bún tươi và các loại rau thơm cuộn tròn trong lớp bánh tráng dẻo mềm.",
    prepTime: 25,
    cookTime: 15,
    servings: 4,
    difficulty: "Easy",
    status: "Published",
    primaryImageUrl: "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=1200&q=80",
    categoryName: "Khai Vị & Ăn Vặt",
    authorDisplayName: "Bếp Trưởng An",
    publishedAt: new Date().toISOString(),
  },
  {
    id: "7b66c7d9-4671-4431-b8e6-d5779cd690b0",
    title: "Chè Bưởi An Giang Không Đắng",
    slug: "che-buoi-an-giang-khong-dang",
    description: "Chè bưởi với cùi bưởi giòn sần sật, đượm vị ngọt ngào của đường thốt nốt, đậu xanh bùi bở hòa quyện cùng nước cốt dừa béo ngậy.",
    prepTime: 40,
    cookTime: 45,
    servings: 6,
    difficulty: "Medium",
    status: "Published",
    primaryImageUrl: "https://images.unsplash.com/photo-1551024709-8f23befc6f87?auto=format&fit=crop&w=1200&q=80",
    categoryName: "Món Tráng Miệng",
    authorDisplayName: "Bếp Trưởng An",
    publishedAt: new Date().toISOString(),
  },
  {
    id: "486b2ff6-33e7-4da9-beff-746d1ba5d95f",
    title: "Nấm Đùi Gà Kho Tiêu Chay Đậm Đà",
    slug: "nam-dui-ga-kho-tieu-chay-dam-da",
    description: "Món nấm kho tiêu chay thơm nồng tiêu xanh, nấm dai ngọt thấm đẫm nước sốt đậm đà, cực kỳ đưa cơm trong những ngày ăn chay thanh tịnh.",
    prepTime: 15,
    cookTime: 20,
    servings: 3,
    difficulty: "Easy",
    status: "Published",
    primaryImageUrl: "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=1200&q=80",
    categoryName: "Món Chay",
    authorDisplayName: "Bếp Trưởng An",
    publishedAt: new Date().toISOString(),
  },
];

export const recipesApi = {
  // FR-RCP-001: GET /api/v1/recipes
  getAll: async (params?: GetRecipesParams): Promise<{ items: RecipeListItemDto[]; total: number }> => {
    try {
      const res = await apiClient.get<ApiResponse<RecipeListItemDto[]>>("/api/v1/recipes", {
        params,
      });
      return {
        items: res.data.data ?? [],
        total: res.data.meta?.total ?? res.data.data?.length ?? 0,
      };
    } catch {
      return {
        items: FALLBACK_RECIPES,
        total: FALLBACK_RECIPES.length,
      };
    }
  },

  // FR-RCP-002: GET /api/v1/recipes/{slug}
  getBySlug: async (slug: string): Promise<RecipeDetailDto | null> => {
    try {
      const res = await apiClient.get<ApiResponse<RecipeDetailDto>>(`/api/v1/recipes/${slug}`);
      return res.data.data;
    } catch {
      return null;
    }
  },
};
