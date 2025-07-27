namespace E_Government.Application.DTO.License
{
    public class LicenseRequestHistoryDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid RequestId { get; set; }

    }
}
