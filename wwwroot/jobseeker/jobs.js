let currentPage = 1;

document.addEventListener('DOMContentLoaded', () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'jobs');

  const searchForm = document.getElementById('searchForm');
  searchForm.addEventListener('submit', (e) => {
    e.preventDefault();
    currentPage = 1;
    loadJobs();
  });

  loadJobs();
});

async function loadJobs() {
  const grid = document.getElementById('jobsGridContainer');
  const search = document.getElementById('searchInput').value.trim();
  const location = document.getElementById('locationInput').value.trim();
  const exp = document.getElementById('expInput').value;

  grid.innerHTML = Loader.renderCardSkeleton(4);

  try {
    const res = await Api.get(CONSTANTS.API_ENDPOINTS.JOBS.SEARCH, {
      search,
      location,
      minExperienceYears: exp ? parseFloat(exp) : null,
      page: currentPage,
      pageSize: 6
    });

    if (!res || !res.items || res.items.length === 0) {
      grid.innerHTML = `
        <div class="card grid-cols-1 text-center py-8" style="grid-column: 1 / -1;">
          <div class="empty-title">No Matching Job Vacancies Found</div>
          <div class="empty-text text-sm">Try broadening your search keywords or location filters.</div>
        </div>
      `;
      Pagination.render({ currentPage: 1, totalPages: 1, onPageChangeContainerId: 'paginationBar', onPageChangeCallback: () => {} });
      return;
    }

    grid.innerHTML = res.items.map(job => {
      const vId = job.vacancyId || job.id;
      const score = job.matchScore !== undefined ? job.matchScore : job.matchPercentage;
      return `
        <div class="card job-card">
          <div>
            <div class="flex items-center justify-between mb-2">
              <div class="font-semibold text-xs text-muted">${UI.icons.building} ${UI.escapeHtml(job.companyName)}</div>
              ${UI.getMatchScoreChip(score)}
            </div>
            <h3 class="h3 mb-2">${UI.escapeHtml(job.title)}</h3>
            <div class="flex flex-wrap gap-3 text-sm text-muted mb-4">
              <span>${UI.icons.location} ${UI.escapeHtml(job.location || 'Not specified')}</span>
              <span>${UI.icons.clock} ${job.requiredExperienceYears || 0}+ Yrs Exp</span>
              <span>${UI.icons.education} ${UI.escapeHtml(job.requiredEducationLevel || 'No requirement')}</span>
            </div>
          </div>
          <div class="flex items-center justify-between pt-3 border-top mt-2">
            <span class="text-xs text-light">Posted ${UI.formatDate(job.postedAt)}</span>
            <div class="flex items-center gap-2">
              ${job.hasApplied 
                ? `<span class="badge badge-selected">Applied</span>`
                : `<a href="/jobseeker/job-details.html?id=${vId}" class="btn btn-primary btn-sm">View & Apply</a>`}
            </div>
          </div>
        </div>
      `;
    }).join('');

    Pagination.render({
      currentPage: res.page,
      totalPages: res.totalPages,
      onPageChangeContainerId: 'paginationBar',
      onPageChangeCallback: (newPage) => {
        currentPage = newPage;
        loadJobs();
      }
    });

  } catch (err) {
    grid.innerHTML = `<div class="text-danger text-center py-6" style="grid-column: 1 / -1;">Failed to load job listings.</div>`;
  }
}
