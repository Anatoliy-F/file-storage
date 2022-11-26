namespace DataAccessLayer.Entities
{
    public class AppFile : BaseEntity
    {

        public byte[] Content { get; set; } = Array.Empty<byte>();

        public AppFileData? AppFileDataNav { get; set; }
    }
}
