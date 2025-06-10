namespace E_Government.Application.DTO.AdminDashboard
{
    public class DashboardStatisticsDto
    {
        public int TotalLicenseRequests { get; set; }
        public int PendingLicenseRequests { get; set; }
        public int ApprovedLicenseRequests { get; set; }
        public int RejectedLicenseRequests { get; set; }
        public int TotalCivilDocRequests { get; set; }
        public int PendingCivilDocRequests { get; set; }
        public int ApprovedCivilDocRequests { get; set; }
        public int RejectedCivilDocRequests { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; } // Added based on review
        public Dictionary<string, int> OtherStats { get; set; } // e.g., Bills paid, Traffic violations
    }
}
