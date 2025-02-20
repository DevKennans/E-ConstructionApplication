namespace EConstructionApp.Domain.Entities.Cross
{
    public class MaterialTask
    {
        public Guid MaterialId { get; set; }
        public Material Material { get; set; }

        public Guid TaskId { get; set; }
        public Task Task { get; set; }
    }
}
