let vacancyId = null;

document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'jobs');

  const params = new URLSearchParams(window.location.search);
  vacancyId = params.get('id');

  if (!vacancyId) {
    window.location.href = CONSTANTS.ROUTES.JOB_SEEKER.JOBS;
    return;
  }

  await loadJobDetails();
});

async function loadJobDetails() {
  const container = document.getElementById('jobDetailsContainer');

  try {
    const job = await Api.get(CONSTANTS.API_ENDPOINTS.JOBS.DETAILS(vacancyId));
    const breakdown = job.match ? job.match.breakdown : null;

    const skillsHtml = job.requiredSkills && job.requiredSkills.length > 0
      ? job.requiredSkills.map(s => `<span class="badge badge-shortlisted">${UI.escapeHtml(typeof s === 'object' ? s.name : s)}</span>`).join(' ')
      : '<span class="text-muted text-sm">No specific skills listed</span>';

    const matchScore = job.match ? (job.match.breakdown ? job.match.breakdown.totalScore : (job.match.overallScore || 0)) : null;

    container.innerHTML = `
      <div class="grid grid-cols-3 gap-6 md:grid-cols-1 mb-6">
        <!-- Main Job Body -->
        <div class="card" style="grid-column: span 2;">
          <div class="flex items-center justify-between mb-4">
            <div>
              <div class="font-semibold text-sm text-primary mb-1">${UI.icons.building} ${UI.escapeHtml(job.companyName)}</div>
              <h1 class="h2 mb-1">${UI.escapeHtml(job.title)}</h1>
              <div class="text-sm text-muted">${UI.icons.location} ${UI.escapeHtml(job.companyLocation || job.jobLocation || 'Location not specified')} • Posted ${UI.formatDate(job.postedAt)}</div>
            </div>
            ${UI.getMatchScoreChip(matchScore)}
          </div>

          <div class="mb-6 pt-4 border-top">
            <h3 class="h4 mb-2">Job Description</h3>
            <div class="text-muted" style="white-space: pre-line; line-height: 1.6;">${UI.escapeHtml(job.description)}</div>
          </div>

          <div class="mb-6">
            <h3 class="h4 mb-2">Required Skills</h3>
            <div class="flex flex-wrap gap-2">${skillsHtml}</div>
          </div>

          <div class="grid grid-cols-2 gap-4 pt-4 border-top text-sm">
            <div>
              <span class="font-semibold text-main">Experience Required:</span>
              <span class="text-muted">${job.requiredExperienceYears || 0}+ Years</span>
            </div>
            <div>
              <span class="font-semibold text-main">Education Level:</span>
              <span class="text-muted">${UI.escapeHtml(job.requiredEducationLevel || 'No Requirement')}</span>
            </div>
          </div>
        </div>

        <!-- Right Match Score Sidebar Card -->
        <div class="flex flex-col gap-6">
          <div class="card">
            <h3 class="h4 mb-3">Match Score Breakdown</h3>
            ${breakdown ? `
              <div class="flex flex-col gap-3 text-sm">
                <div>
                  <div class="flex justify-between font-medium mb-1">
                    <span>Skill Overlap Score</span>
                    <span>${Math.round(breakdown.skillScore || 0)}%</span>
                  </div>
                  <div class="progress-bar-container"><div class="progress-bar-fill" style="width: ${breakdown.skillScore}%;"></div></div>
                </div>

                <div>
                  <div class="flex justify-between font-medium mb-1">
                    <span>Experience Score</span>
                    <span>${Math.round(breakdown.experienceScore || 0)}%</span>
                  </div>
                  <div class="progress-bar-container"><div class="progress-bar-fill" style="width: ${breakdown.experienceScore}%;"></div></div>
                </div>

                <div>
                  <div class="flex justify-between font-medium mb-1">
                    <span>Education Level Score</span>
                    <span>${Math.round(breakdown.educationScore || 0)}%</span>
                  </div>
                  <div class="progress-bar-container"><div class="progress-bar-fill" style="width: ${breakdown.educationScore}%;"></div></div>
                </div>
              </div>
            ` : '<div class="text-muted text-sm">Match calculation details unavailable.</div>'}

            <div class="mt-6">
              <button id="applyNowBtn" class="btn btn-primary btn-block btn-lg" onclick="submitApplication()">
                Submit Application Now
              </button>
            </div>
          </div>
        </div>
      </div>
    `;
  } catch (err) {
    container.innerHTML = `<div class="card text-danger text-center py-6">Failed to load vacancy details. ${UI.escapeHtml(err.message)}</div>`;
  }
}

async function submitApplication() {
  const btn = document.getElementById('applyNowBtn');
  if (btn) btn.disabled = true;

  try {
    await Api.post(CONSTANTS.API_ENDPOINTS.JOBS.APPLY(vacancyId));
    Toast.success('Application submitted successfully!');
    setTimeout(() => {
      window.location.href = CONSTANTS.ROUTES.JOB_SEEKER.APPLICATIONS;
    }, 1000);
  } catch (err) {
    Toast.error(err.message || 'Failed to submit application.');
    if (btn) btn.disabled = false;
  }
}
