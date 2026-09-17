using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<CulinaryBlogDbContext>>();
        var db = services.GetRequiredService<CulinaryBlogDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            logger.LogInformation("Bắt đầu kiểm tra và nạp dữ liệu mẫu (Database Seeding)...");

            // 1. Roles
            var roles = new[] { "Admin", "Author", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Đã tạo vai trò: {Role}", role);
                }
            }

            // 2. Users (Admin & Author)
            var adminEmail = "admin@culinary.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = ApplicationUser.Create(adminEmail, "Quản Trị Viên", "admin");
                adminUser.EmailConfirmed = true;
                adminUser.Bio = "Quản trị viên hệ thống Culinary Blog.";
                adminUser.AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=300&q=80";

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Đã tạo tài khoản Admin mặc định: {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Lỗi khi tạo Admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            var authorEmail = "bep_truong_an@culinary.local";
            var authorUser = await userManager.FindByEmailAsync(authorEmail);
            if (authorUser == null)
            {
                authorUser = ApplicationUser.Create(authorEmail, "Bếp Trưởng An", "bep_truong_an");
                authorUser.EmailConfirmed = true;
                authorUser.Bio = "Đầu bếp đam mê ẩm thực truyền thống Việt Nam với hơn 10 năm kinh nghiệm.";
                authorUser.AvatarUrl = "https://images.unsplash.com/photo-1577219491135-ce391730fb2c?auto=format&fit=crop&w=300&q=80";

                var result = await userManager.CreateAsync(authorUser, "Author@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(authorUser, "Author");
                    logger.LogInformation("Đã tạo tài khoản Author mặc định: {Email}", authorEmail);
                }
            }

            // 3. Categories (6 danh mục)
            if (!await db.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    Category.Create(
                        "Món Chính",
                        "Những món ăn chính thơm ngon, đậm đà cho bữa cơm gia đình và tiệc tùng.",
                        "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=800&q=80",
                        1
                    ),
                    Category.Create(
                        "Món Nước & Canh",
                        "Các món bún, phở, miến và canh thanh mát giải nhiệt, ấm lòng ngày mưa.",
                        "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?auto=format&fit=crop&w=800&q=80",
                        2
                    ),
                    Category.Create(
                        "Khai Vị & Ăn Vặt",
                        "Món ăn nhẹ, gỏi cuốn, bánh mặn bắt vị kích thích vị giác đầu bữa ăn.",
                        "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=800&q=80",
                        3
                    ),
                    Category.Create(
                        "Món Tráng Miệng",
                        "Các món chè, bánh ngọt, trái cây và kem ngọt ngào kết thúc bữa ăn trọn vẹn.",
                        "https://images.unsplash.com/photo-1551024709-8f23befc6f87?auto=format&fit=crop&w=800&q=80",
                        4
                    ),
                    Category.Create(
                        "Món Chay",
                        "Ẩm thực thuần chay thanh tịnh, dinh dưỡng cân bằng và tốt cho sức khỏe.",
                        "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=800&q=80",
                        5
                    ),
                    Category.Create(
                        "Đồ Uống & Giải Khát",
                        "Trà thảo mộc, nước ép hoa quả, sinh tố và đồ uống mát lạnh sảng khoái.",
                        "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?auto=format&fit=crop&w=800&q=80",
                        6
                    )
                };

                await db.Categories.AddRangeAsync(categories);
                await db.SaveChangesAsync();
                logger.LogInformation("Đã nạp {Count} danh mục mẫu.", categories.Count);
            }

            // 4. Recipes (6 công thức hoàn chỉnh)
            if (!await db.Recipes.AnyAsync())
            {
                var author = await userManager.FindByEmailAsync("bep_truong_an@culinary.local")
                             ?? await userManager.FindByEmailAsync("admin@culinary.local");

                if (author == null) return;

                var catMonChinh = await db.Categories.FirstAsync(c => c.Slug == "mon-chinh");
                var catMonNuoc = await db.Categories.FirstAsync(c => c.Slug == "mon-nuoc-canh");
                var catKhaiVi = await db.Categories.FirstAsync(c => c.Slug == "khai-vi-an-vat");
                var catTrangMieng = await db.Categories.FirstAsync(c => c.Slug == "mon-trang-mieng");
                var catMonChay = await db.Categories.FirstAsync(c => c.Slug == "mon-chay");

                var recipes = new List<Recipe>();

                // Recipe 1: Phở Bò Tái Lăn Hà Nội
                var phoBo = Recipe.Create(
                    "Phở Bò Tái Lăn Hà Nội",
                    "Món phở bò tái lăn chuẩn vị phố cổ với nước dùng trong veo, thơm nức mùi gừng nướng, quế hồi cùng thịt bò xào lửa lớn mềm ngọt.",
                    "Nấu nước dùng từ xương bò nướng, xào thịt bò nhanh tay với tỏi trên lửa lớn rồi chan nước dùng sôi sùng sục.",
                    catMonNuoc.Id,
                    author.Id,
                    prepTime: 30,
                    cookTime: 120,
                    servings: 4,
                    difficulty: RecipeDifficulty.Medium
                );
                phoBo.SetNutrition(new RecipeNutrition(550, 38, 65, 14, 3, 980));
                phoBo.Ingredients.Add(RecipeIngredient.Create(phoBo.Id, "Bánh phở tươi", 500, "g", "chần qua nước sôi", 1));
                phoBo.Ingredients.Add(RecipeIngredient.Create(phoBo.Id, "Thịt bò thăn / bắp hoa", 400, "g", "thái lát mỏng", 2));
                phoBo.Ingredients.Add(RecipeIngredient.Create(phoBo.Id, "Xương ống bò", 1000, "g", "ninh lấy nước dùng", 3));
                phoBo.Ingredients.Add(RecipeIngredient.Create(phoBo.Id, "Gừng & hành tây", 2, "củ", "nướng cháy thơm", 4));
                phoBo.Ingredients.Add(RecipeIngredient.Create(phoBo.Id, "Hoa hồi, quế, thảo quả", 1, "gói", "rang thơm", 5));
                phoBo.Ingredients.Add(RecipeIngredient.Create(phoBo.Id, "Hành lá, mùi tàu, ớt tươi", null, null, "rửa sạch thái nhỏ", 6));

                phoBo.Steps.Add(RecipeStep.Create(phoBo.Id, 1, "Ninh nước dùng phở", "Rửa sạch xương bò, luộc sơ rồi rửa lại. Cho vào nồi cùng gừng, hành nướng và quế hồi. Ninh nhỏ lửa trong 2 tiếng, hớt bọt thường xuyên.", 120));
                phoBo.Steps.Add(RecipeStep.Create(phoBo.Id, 2, "Xào thịt bò tái lăn", "Đun chảo thật nóng với chút dầu ăn và tỏi đập dập. Cho thịt bò vào đảo nhanh tay ở lửa lớn trong 1 phút vừa chín tới rồi tắt bếp.", 3));
                phoBo.Steps.Add(RecipeStep.Create(phoBo.Id, 3, "Chần bánh phở và xếp bát", "Chần bánh phở qua nước sôi, cho vào tô. Xếp thịt bò tái lăn lên trên, thêm hành hoa và mùi tàu thái nhỏ.", 2));
                phoBo.Steps.Add(RecipeStep.Create(phoBo.Id, 4, "Chan nước dùng và thưởng thức", "Chan nước dùng sôi sùng sục ngập bánh phở. Dùng kèm tương ớt, giấm tỏi và chanh tươi.", 1));

                phoBo.Images.Add(RecipeImage.Create(
                    phoBo.Id,
                    "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?auto=format&fit=crop&w=1200&q=80",
                    "Tô Phở Bò Tái Lăn Hà Nội thơm ngào ngạt",
                    isPrimary: true,
                    orderIndex: 0
                ));
                phoBo.Publish();
                recipes.Add(phoBo);

                // Recipe 2: Cơm Tấm Sườn Bì Chả Sài Gòn
                var comTam = Recipe.Create(
                    "Cơm Tấm Sườn Bì Chả Sài Gòn",
                    "Đĩa cơm tấm chuẩn vị Sài Gòn với sườn nướng mỡ hành óng ánh, chả trứng bùi béo, bì dai giòn cùng nước mắm chua ngọt đậm đà.",
                    "Ướp sườn qua đêm, nướng trên than hồng, hấp chả trứng và pha nước mắm kẹo.",
                    catMonChinh.Id,
                    author.Id,
                    prepTime: 45,
                    cookTime: 40,
                    servings: 4,
                    difficulty: RecipeDifficulty.Medium
                );
                comTam.SetNutrition(new RecipeNutrition(720, 42, 80, 24, 4, 1100));
                comTam.Ingredients.Add(RecipeIngredient.Create(comTam.Id, "Gạo tấm thơm", 400, "g", "nấu vừa nước", 1));
                comTam.Ingredients.Add(RecipeIngredient.Create(comTam.Id, "Sườn cốt lết heo", 4, "miếng", "chặt dày 1.5cm", 2));
                comTam.Ingredients.Add(RecipeIngredient.Create(comTam.Id, "Bì heo trộn thính", 150, "g", "sợi mỏng dai giòn", 3));
                comTam.Ingredients.Add(RecipeIngredient.Create(comTam.Id, "Thịt băm, trứng vịt, mộc nhĩ", 250, "g", "làm chả trứng hấp", 4));
                comTam.Ingredients.Add(RecipeIngredient.Create(comTam.Id, "Mỡ hành & đồ chua cà rốt", null, null, "ăn kèm", 5));
                comTam.Ingredients.Add(RecipeIngredient.Create(comTam.Id, "Nước mắm chua ngọt", 1, "chén", "pha sánh tỏi ớt", 6));

                comTam.Steps.Add(RecipeStep.Create(comTam.Id, 1, "Ướp và nướng sườn", "Ướp sườn với sữa đặc, sả băm, tỏi, dầu hào và nước tương trong 30 phút. Nướng trên than hồng lật đều tay đến khi vàng óng.", 30));
                comTam.Steps.Add(RecipeStep.Create(comTam.Id, 2, "Hấp chả trứng", "Trộn thịt băm, mộc nhĩ, miến và lòng đỏ trứng. Hấp cách thủy 25 phút, phết lòng đỏ lên mặt cho màu đẹp.", 25));
                comTam.Steps.Add(RecipeStep.Create(comTam.Id, 3, "Nấu cơm tấm và làm mỡ hành", "Nấu cơm tấm chín tới. Rưới dầu sôi vào bát hành lá thái nhỏ làm mỡ hành thơm phức.", 20));
                comTam.Steps.Add(RecipeStep.Create(comTam.Id, 4, "Trình bày đĩa cơm tấm", "Xới cơm ra đĩa phẳng, xếp sườn nướng, chả trứng, bì, dưa leo, cà chua và rưới mỡ hành lên mặt. Thưởng thức cùng nước mắm kẹo.", 5));

                comTam.Images.Add(RecipeImage.Create(
                    comTam.Id,
                    "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=1200&q=80",
                    "Đĩa cơm tấm sườn bì chả Sài Gòn đầy đặn",
                    isPrimary: true,
                    orderIndex: 0
                ));
                comTam.Publish();
                recipes.Add(comTam);

                // Recipe 3: Bánh Xèo Giòn Rụm Miền Tây
                var banhXeo = Recipe.Create(
                    "Bánh Xèo Giòn Rụm Miền Tây",
                    "Vỏ bánh xèo mỏng tang, vàng ươm nghệ và cốt dừa thơm béo, nhân tôm thịt tươi ngon cuốn cùng cả rổ rau rừng thanh mát.",
                    "Pha bột với nước cốt dừa và bia để giòn lâu, chiên áp chảo lửa lớn cuốn rau cải xanh và chấm nước mắm chua ngọt.",
                    catMonChinh.Id,
                    author.Id,
                    prepTime: 30,
                    cookTime: 30,
                    servings: 4,
                    difficulty: RecipeDifficulty.Easy
                );
                banhXeo.SetNutrition(new RecipeNutrition(460, 22, 52, 18, 5, 850));
                banhXeo.Ingredients.Add(RecipeIngredient.Create(banhXeo.Id, "Bột bánh xèo pha sẵn", 400, "g", "hòa cùng nước cốt dừa", 1));
                banhXeo.Ingredients.Add(RecipeIngredient.Create(banhXeo.Id, "Tôm thẻ tươi", 300, "g", "bỏ đầu rút chỉ đen", 2));
                banhXeo.Ingredients.Add(RecipeIngredient.Create(banhXeo.Id, "Thịt ba chỉ heo", 250, "g", "thái lát mỏng", 3));
                banhXeo.Ingredients.Add(RecipeIngredient.Create(banhXeo.Id, "Giá đỗ và hành lá", 200, "g", "rửa sạch để ráo", 4));
                banhXeo.Ingredients.Add(RecipeIngredient.Create(banhXeo.Id, "Rau cải non, xà lách, rau thơm", null, null, "rổ rau ăn kèm tươi xanh", 5));

                banhXeo.Steps.Add(RecipeStep.Create(banhXeo.Id, 1, "Pha bột bánh xèo", "Hòa bột bánh xèo với nước cốt dừa, nước lọc, hành lá cắt nhỏ và chút bột nghệ để màu sắc bắt mắt.", 15));
                banhXeo.Steps.Add(RecipeStep.Create(banhXeo.Id, 2, "Xào sơ nhân tôm thịt", "Cho tôm và thịt ba chỉ vào chảo xào chín tới cùng chút hạt nêm tiêu rồi múc ra đĩa riêng.", 5));
                banhXeo.Steps.Add(RecipeStep.Create(banhXeo.Id, 3, "Đổ bánh xèo giòn tan", "Tráng chảo dầu nóng, múc 1 vá bột láng đều thật mỏng quanh lòng chảo. Thêm tôm, thịt, giá đỗ rồi đậy nắp trong 2 phút. Mở nắp hạ lửa để vành bánh giòn rụm.", 10));
                banhXeo.Steps.Add(RecipeStep.Create(banhXeo.Id, 4, "Gấp đôi bánh và thưởng thức", "Gấp đôi bánh lại, vớt ra đĩa có lót giấy thấm dầu. Dùng kéo cắt miếng vừa ăn, cuộn bánh tráng và rau sống chấm nước mắm chua ngọt.", 5));

                banhXeo.Images.Add(RecipeImage.Create(
                    banhXeo.Id,
                    "https://images.unsplash.com/photo-1559847844-5315695dadae?auto=format&fit=crop&w=1200&q=80",
                    "Bánh xèo miền Tây vàng giòn rụm",
                    isPrimary: true,
                    orderIndex: 0
                ));
                banhXeo.Publish();
                recipes.Add(banhXeo);

                // Recipe 4: Gỏi Cuốn Tôm Thịt Thanh Mát
                var goiCuon = Recipe.Create(
                    "Gỏi Cuốn Tôm Thịt Thanh Mát",
                    "Món khai vị trứ danh tươi mát với tôm đỏ au, thịt luộc thái mỏng, bún tươi và các loại rau thơm cuộn tròn trong lớp bánh tráng dẻo mềm.",
                    "Luộc tôm thịt chín tới, cuốn khéo tay cùng bún và rau sống, chấm sốt tương đen bơ đậu phộng béo bùi.",
                    catKhaiVi.Id,
                    author.Id,
                    prepTime: 25,
                    cookTime: 15,
                    servings: 4,
                    difficulty: RecipeDifficulty.Easy
                );
                goiCuon.SetNutrition(new RecipeNutrition(260, 18, 38, 5, 3, 520));
                goiCuon.Ingredients.Add(RecipeIngredient.Create(goiCuon.Id, "Tôm sú tươi", 300, "g", "luộc chín bóc vỏ chẻ đôi", 1));
                goiCuon.Ingredients.Add(RecipeIngredient.Create(goiCuon.Id, "Thịt ba chỉ rút sườn", 250, "g", "luộc chín thái mỏng", 2));
                goiCuon.Ingredients.Add(RecipeIngredient.Create(goiCuon.Id, "Bánh tráng dẻo", 1, "xấp", "loại cuốn gỏi không rách", 3));
                goiCuon.Ingredients.Add(RecipeIngredient.Create(goiCuon.Id, "Bún tươi sợi nhỏ", 300, "g", "rửa sạch để ráo", 4));
                goiCuon.Ingredients.Add(RecipeIngredient.Create(goiCuon.Id, "Xà lách, rau thơm, hẹ lá", null, null, "rửa sạch", 5));
                goiCuon.Ingredients.Add(RecipeIngredient.Create(goiCuon.Id, "Tương đen chấm gỏi cuốn", 1, "bát", "nấu cùng bơ đậu phộng", 6));

                goiCuon.Steps.Add(RecipeStep.Create(goiCuon.Id, 1, "Sơ chế và luộc tôm thịt", "Luộc thịt ba chỉ với chút hành tím cho thơm rồi thái mỏng. Luộc tôm chín đỏ, bóc vỏ bỏ chỉ lưng và chẻ đôi.", 15));
                goiCuon.Steps.Add(RecipeStep.Create(goiCuon.Id, 2, "Nấu sốt tương chấm", "Phi thơm tỏi, cho tương hột xay và bơ đậu phộng vào đảo đều, nêm đường cho vừa khẩu vị sánh mịn.", 10));
                goiCuon.Steps.Add(RecipeStep.Create(goiCuon.Id, 3, "Cuốn gỏi cuốn", "Làm ẩm bánh tráng, xếp xà lách, rau thơm, bún rồi đến thịt luộc. Gấp hai mép bánh lại, xếp tôm và cọng hẹ thò ra ngoài rồi cuộn chặt tay.", 10));

                goiCuon.Images.Add(RecipeImage.Create(
                    goiCuon.Id,
                    "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=1200&q=80",
                    "Gỏi cuốn tôm thịt thanh mát chấm tương bùi béo",
                    isPrimary: true,
                    orderIndex: 0
                ));
                goiCuon.Publish();
                recipes.Add(goiCuon);

                // Recipe 5: Chè Bưởi An Giang Không Đắng
                var cheBuoi = Recipe.Create(
                    "Chè Bưởi An Giang Không Đắng",
                    "Chè bưởi với cùi bưởi giòn sần sật, đượm vị ngọt ngào của đường thốt nốt, đậu xanh bùi bở hòa quyện cùng nước cốt dừa béo ngậy.",
                    "Khử đắng cùi bưởi bằng muối và phèn chua, áo bột năng luộc trong veo, nấu cùng đậu xanh hấp và nước cốt dừa.",
                    catTrangMieng.Id,
                    author.Id,
                    prepTime: 40,
                    cookTime: 45,
                    servings: 6,
                    difficulty: RecipeDifficulty.Medium
                );
                cheBuoi.SetNutrition(new RecipeNutrition(320, 6, 60, 8, 4, 120));
                cheBuoi.Ingredients.Add(RecipeIngredient.Create(cheBuoi.Id, "Cùi bưởi da xanh", 1, "quả", "chỉ lấy phần cùi trắng xốp", 1));
                cheBuoi.Ingredients.Add(RecipeIngredient.Create(cheBuoi.Id, "Đậu xanh cà vỏ", 200, "g", "ngâm nở hấp chín", 2));
                cheBuoi.Ingredients.Add(RecipeIngredient.Create(cheBuoi.Id, "Bột năng cao cấp", 150, "g", "làm giòn cùi bưởi", 3));
                cheBuoi.Ingredients.Add(RecipeIngredient.Create(cheBuoi.Id, "Đường thốt nốt An Giang", 250, "g", "nấu ngọt thanh", 4));
                cheBuoi.Ingredients.Add(RecipeIngredient.Create(cheBuoi.Id, "Nước cốt dừa đậm đặc", 300, "ml", "nấu riêng ăn kèm", 5));

                cheBuoi.Steps.Add(RecipeStep.Create(cheBuoi.Id, 1, "Sơ chế khử đắng cùi bưởi", "Thái cùi bưởi hạt lựu, bóp nhiều lần với muối hột rồi xả nước sạch cho hết tinh dầu đắng. Luộc sơ qua nước sôi và vắt ráo.", 30));
                cheBuoi.Steps.Add(RecipeStep.Create(cheBuoi.Id, 2, "Áo bột năng và luộc cùi bưởi", "Trộn cùi bưởi với chút đường thốt nốt cho ngấm, sau đó lăn đều qua bột năng. Luộc cùi bưởi đến khi nổi lên trong veo thì vớt ngay vào âu nước đá.", 15));
                cheBuoi.Steps.Add(RecipeStep.Create(cheBuoi.Id, 3, "Nấu nước đường đậu xanh", "Đun sôi 1 lít nước với đường thốt nốt. Cho đậu xanh đã hấp chín vào khuấy đều. Xuống bột năng hòa nước cho sánh lại.", 15));
                cheBuoi.Steps.Add(RecipeStep.Create(cheBuoi.Id, 4, "Hoàn thiện chè bưởi", "Trút cùi bưởi giòn vào nồi chè, khuấy nhẹ tay rồi tắt bếp. Múc chè ra ly, rưới nước cốt dừa béo ngậy lên trên.", 5));

                cheBuoi.Images.Add(RecipeImage.Create(
                    cheBuoi.Id,
                    "https://images.unsplash.com/photo-1551024709-8f23befc6f87?auto=format&fit=crop&w=1200&q=80",
                    "Ly chè bưởi cốt dừa ngọt lành mát lịm",
                    isPrimary: true,
                    orderIndex: 0
                ));
                cheBuoi.Publish();
                recipes.Add(cheBuoi);

                // Recipe 6: Nấm Đùi Gà Kho Tiêu Chay
                var namKho = Recipe.Create(
                    "Nấm Đùi Gà Kho Tiêu Chay Đậm Đà",
                    "Món nấm kho tiêu chay thơm nồng tiêu xanh, nấm dai ngọt thấm đẫm nước sốt đậm đà, cực kỳ đưa cơm trong những ngày ăn chay thanh tịnh.",
                    "Kho nấm đùi gà với nước tương, tiêu xanh, dầu hào chay trong nồi đất đến khi nước sốt keo lại óng ánh.",
                    catMonChay.Id,
                    author.Id,
                    prepTime: 15,
                    cookTime: 20,
                    servings: 3,
                    difficulty: RecipeDifficulty.Easy
                );
                namKho.SetNutrition(new RecipeNutrition(195, 8, 24, 7, 5, 780));
                namKho.Ingredients.Add(RecipeIngredient.Create(namKho.Id, "Nấm đùi gà tươi", 400, "g", "rửa sạch khứa ca-rô", 1));
                namKho.Ingredients.Add(RecipeIngredient.Create(namKho.Id, "Tiêu xanh tươi & tiêu sọ", 3, "nhánh", "đập dập nhẹ", 2));
                namKho.Ingredients.Add(RecipeIngredient.Create(namKho.Id, "Hành boa-rô", 1, "cây", "thái lát mỏng", 3));
                namKho.Ingredients.Add(RecipeIngredient.Create(namKho.Id, "Nước tương ngon & dầu hào chay", 3, "muỗng canh", "pha sốt kho", 4));
                namKho.Ingredients.Add(RecipeIngredient.Create(namKho.Id, "Ớt hiểm đỏ", 2, "trái", "để nguyên cuống", 5));

                namKho.Steps.Add(RecipeStep.Create(namKho.Id, 1, "Sơ chế nấm đùi gà", "Rửa sạch nấm đùi gà, cắt khúc dày 2cm rồi dùng dao khứa vát hình caro lên mặt để nấm ngấm đều gia vị.", 10));
                namKho.Steps.Add(RecipeStep.Create(namKho.Id, 2, "Áp chảo nấm", "Cho nấm vào chảo với chút xíu dầu ăn, áp chảo vàng đều hai mặt cho thơm và dai hơn.", 5));
                namKho.Steps.Add(RecipeStep.Create(namKho.Id, 3, "Kho tiêu đậm vị", "Cho hành boa-rô và tiêu xanh vào nồi đất phi thơm. Thêm nước tương, dầu hào chay, đường và 1/2 chén nước. Cho nấm vào đun nhỏ lửa đến khi nước sốt sệt lại.", 15));

                namKho.Images.Add(RecipeImage.Create(
                    namKho.Id,
                    "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=1200&q=80",
                    "Nồi nấm đùi gà kho tiêu chay thơm phức đậm đà",
                    isPrimary: true,
                    orderIndex: 0
                ));
                namKho.Publish();
                recipes.Add(namKho);

                await db.Recipes.AddRangeAsync(recipes);
                await db.SaveChangesAsync();
                logger.LogInformation("Đã nạp {Count} công thức nấu ăn mẫu hoàn chỉnh kèm nguyên liệu, các bước và ảnh.", recipes.Count);
            }

            logger.LogInformation("Hoàn tất nạp dữ liệu mẫu thành công!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Có lỗi xảy ra trong quá trình nạp dữ liệu mẫu.");
            throw;
        }
    }
}
