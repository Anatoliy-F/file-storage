namespace DataAccessLayer.Entities
{
    public class ShortLink : BaseEntity
    {
        public string Link { get; set; } = string.Empty;
        
        public Guid AppFileDataId { get; set; }

        public AppFileData? AppFileDataNav { get; set; }
    }
}
