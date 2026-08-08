namespace SmartRecruitmentMatchingPlatform.Models.Entities;

public class JobSeekerSkill
{
    public int JobSeekerProfileId { get; set; }

    public int SkillId { get; set; }

    public JobSeekerProfile JobSeekerProfile { get; set; } = null!;

    public Skill Skill { get; set; } = null!;
}