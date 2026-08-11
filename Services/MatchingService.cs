using SmartRecruitmentMatchingPlatform.DTOs.Jobs;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Services;

public class MatchingService : IMatchingService
{
    public MatchResultDto Calculate(
        JobSeekerProfile? profile,
        Vacancy vacancy)
    {
        var candidateSkillIds = profile?.JobSeekerSkills
            .Select(x => x.SkillId)
            .ToHashSet() ?? new HashSet<int>();

        var requiredSkills = vacancy.VacancySkills
            .Where(x => x.Skill != null)
            .Select(x => x.Skill)
            .ToList();

        var matchedCount = requiredSkills.Count(skill =>
            candidateSkillIds.Contains(skill.Id));

        decimal skillScore;

        if (requiredSkills.Count == 0)
        {
            skillScore = 60m;
        }
        else
        {
            skillScore =
                ((decimal)matchedCount / requiredSkills.Count)
                * 60m;
        }

        var candidateExp = profile?.TotalExperienceYears ?? 0m;
        var experienceScore = CalculateExperienceScore(
            candidateExp,
            vacancy.RequiredExperienceYears);

        decimal educationScore;

        if (vacancy.RequiredEducationLevel ==
            Models.Enums.EducationLevel.NoRequirement)
        {
            educationScore = 10m;
        }
        else if (profile != null &&
                 profile.EducationLevel.HasValue &&
                 profile.EducationLevel.Value >=
                 vacancy.RequiredEducationLevel)
        {
            educationScore = 10m;
        }
        else
        {
            educationScore = 0m;
        }

        decimal locationScore = 0m;

        if (profile != null &&
            !string.IsNullOrWhiteSpace(profile.Location) &&
            !string.IsNullOrWhiteSpace(vacancy.Location) &&
            string.Equals(
                profile.Location.Trim(),
                vacancy.Location.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            locationScore = 10m;
        }

        var totalScore =
            skillScore +
            experienceScore +
            educationScore +
            locationScore;

        var missingSkills = requiredSkills
            .Where(skill =>
                !candidateSkillIds.Contains(skill.Id))
            .Select(skill => new MissingSkillDto
            {
                Id = skill.Id,
                Name = skill.Name
            })
            .OrderBy(skill => skill.Name)
            .ToList();

        return new MatchResultDto
        {
            MatchedSkillCount = matchedCount,

            RequiredSkillCount = requiredSkills.Count,

            MissingSkills = missingSkills,

            Breakdown = new MatchBreakdownDto
            {
                SkillScore = Round(skillScore),

                ExperienceScore =
                    Round(experienceScore),

                EducationScore =
                    Round(educationScore),

                LocationScore =
                    Round(locationScore),

                TotalScore =
                    Round(totalScore)
            }
        };
    }

    private static decimal CalculateExperienceScore(
        decimal candidateExperience,
        decimal requiredExperience)
    {
        if (requiredExperience <= 0)
        {
            return 20m;
        }

        if (candidateExperience >= requiredExperience)
        {
            return 20m;
        }

        if (candidateExperience <= 0)
        {
            return 0m;
        }

        return (candidateExperience / requiredExperience)
               * 20m;
    }

    private static decimal Round(decimal value)
    {
        return Math.Round(
            value,
            2,
            MidpointRounding.AwayFromZero);
    }
}