using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Queries.SearchRecipes;

public class SearchRecipesQueryValidator : AbstractValidator<SearchRecipesQuery>
{
    public SearchRecipesQueryValidator()
    {
        RuleFor(x => x.Keyword)
            .NotEmpty().WithMessage("Vui lòng nhập từ khóa tìm kiếm.")
            .MaximumLength(200).WithMessage("Từ khóa tìm kiếm quá dài.");
    }
}
