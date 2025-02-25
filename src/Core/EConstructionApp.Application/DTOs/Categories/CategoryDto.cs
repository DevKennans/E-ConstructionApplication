namespace EConstructionApp.Application.DTOs.Categories
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public virtual DateTime InsertedDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}
