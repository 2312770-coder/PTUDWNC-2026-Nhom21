namespace CulinaryBlog.Domain.Enums;

// SRS mục 7.2 - lưu dạng smallint trong DB: 0=Draft, 1=Published, 2=Archived.
public enum RecipeStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
}
