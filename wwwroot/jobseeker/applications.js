let currentPage = 1;

document.addEventListener('DOMContentLoaded', () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'applications');

  const filter = document.getElementById('statusFilter');
  filter.addEventListener('change', () => {
    currentPage = 1;
    loadApplications();
  });

  loadApplications();
});

async function loadApplications() {
  const tbody = document.getElementById('applicationsTableBody');
  const status = document.getElementById('statusFilter').value;

  tbody.innerHTML = Loader.renderTableSkeleton(5, 4);

  try {
    const res = await Api.get(CONSTANTS.API_ENDPOINTS.APPLICATIONS.MINE, {
      status: status || null,
      page: currentPage,
      pageSize: 10
    });

    if (!res || !res.items || res.items.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="5" class="text-center py-6 text-muted">
            No job applications found. <a href="/jobseeker/jobs.html" class="link">Browse jobs to apply</a>.
          </td>
        </tr>
      `;
      Pagination.render({ currentPage: 1, totalPages: 1, onPageChangeContainerId: 'paginationBar', onPageChangeCallback: () => {} });
      return;
    }

    tbody.innerHTML = res.items.map(app => {
      const title = app.jobTitle || app.vacancyTitle || 'Vacancy';
      const score = app.matchScore !== undefined ? app.matchScore : app.matchingScore;
      return `
        <tr>
          <td class="font-semibold text-main">
            <a href="/jobseeker/job-details.html?id=${app.vacancyId}" class="link">${UI.escapeHtml(title)}</a>
          </td>
          <td>${UI.icons.building} ${UI.escapeHtml(app.companyName)}</td>
          <td>${UI.formatDate(app.appliedAt)}</td>
          <td>${UI.getMatchScoreChip(score)}</td>
          <td>${UI.getStatusBadge(app.status)}</td>
        </tr>
      `;
    }).join('');

    Pagination.render({
      currentPage: res.page,
      totalPages: res.totalPages,
      onPageChangeContainerId: 'paginationBar',
      onPageChangeCallback: (newPage) => {
        currentPage = newPage;
        loadApplications();
      }
    });
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-danger text-center py-4">Failed to load applications.</td></tr>`;
  }
}
