namespace SmartRecruitmentMatchingPlatform.Models.Entities;

public class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; }
        = new List<JobSeekerSkill>();
}