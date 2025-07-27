namespace E_Government.Application.DTO.AdminDashboard
{
    // DTOs for Chart.js
    public class ChartDataDto
    {
        public List<string> Labels { get; set; }
        public List<ChartDatasetDto> Datasets { get; set; }
    }
}
