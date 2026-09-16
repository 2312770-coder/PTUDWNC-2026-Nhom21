namespace CulinaryBlog.Domain.Entities;

// SRS mục 7.2.1 - Owned Entity, không có bảng riêng.
// Các cột nhúng thẳng vào bảng "Recipes" với tiền tố "Nutrition_".
// Tất cả đều nullable vì không phải công thức nào cũng khai báo dinh dưỡng.
public record RecipeNutrition(
    decimal? Calories,       // kcal / serving
    decimal? Protein,        // gram / serving
    decimal? Carbohydrates,  // gram / serving
    decimal? Fat,            // gram / serving
    decimal? Fiber,          // gram / serving
    decimal? Sodium          // mg / serving
);
