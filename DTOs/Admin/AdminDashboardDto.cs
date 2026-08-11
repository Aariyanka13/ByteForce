namespace SmartRecruitmentMatchingPlatform.DTOs.Admin;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }

    public int TotalJobSeekers { get; set; }

    public int TotalEmployers { get; set; }

    public int ActiveUsers { get; set; }

    public int DisabledUsers { get; set; }

    public int TotalVacancies { get; set; }

    public int TotalApplications { get; set; }

    public int TotalContactRequests { get; set; }
}