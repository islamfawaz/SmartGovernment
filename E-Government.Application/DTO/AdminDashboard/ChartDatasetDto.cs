namespace E_Government.Application.DTO.AdminDashboard
{
    public class ChartDatasetDto
    {
        public string Label { get; set; }
        public List<object> Data { get; set; }
        public string BackgroundColor { get; set; } // Optional: for styling
        public string BorderColor { get; set; } // Optional: for styling
        public bool Fill { get; set; } // Optional
    }
}
