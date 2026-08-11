document.addEventListener('DOMContentLoaded', () => {
  Guards.requireRole(CONSTANTS.ROLES.EMPLOYER);
  Navigation.renderShell(CONSTANTS.ROLES.EMPLOYER, 'vacancies');

  loadVacancies();
});

async function loadVacancies() {
  const tbody = document.getElementById('vacanciesTableBody');
  tbody.innerHTML = Loader.renderTableSkeleton(6, 4);

  try {
    const list = await Api.get(CONSTANTS.API_ENDPOINTS.VACANCIES.BASE);

    if (!list || list.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="6" class="text-center py-6 text-muted">
            No vacancies created yet. <a href="/employer/vacancy-create.html" class="link">Create your first job vacancy</a>.
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = list.map(v => {
      const skillsBadge = v.requiredSkills && v.requiredSkills.length > 0
        ? v.requiredSkills.slice(0, 3).map(s => `<span class="badge badge-applied" style="font-size: 10px;">${UI.escapeHtml(s.name)}</span>`).join(' ')
        : '<span class="text-xs text-muted">None</span>';

      return `
        <tr>
          <td class="font-semibold text-main">
            <a href="/employer/vacancy-details.html?id=${v.id}" class="link">${UI.escapeHtml(v.title)}</a>
          </td>
          <td>${UI.icons.location} ${UI.escapeHtml(v.location || 'Remote')}</td>
          <td>${v.requiredExperienceYears || 0}+ Years</td>
          <td>${skillsBadge}</td>
          <td>${v.isOpen ? '<span class="badge badge-selected">Open</span>' : '<span class="badge badge-rejected">Closed</span>'}</td>
          <td style="text-align: right;">
            <div class="flex items-center justify-end gap-2">
              <a href="/employer/applications.html?vacancyId=${v.id}" class="btn btn-primary btn-sm">Applicants</a>
              <a href="/employer/vacancy-edit.html?id=${v.id}" class="btn btn-secondary btn-sm">Edit</a>
              ${v.isOpen ? `<button class="btn btn-secondary btn-sm text-danger" onclick="closeVacancy(${v.id})">Close</button>` : ''}
            </div>
          </td>
        </tr>
      `;
    }).join('');
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="6" class="text-danger text-center py-4">Failed to load vacancies.</td></tr>`;
  }
}

function closeVacancy(id) {
  Modal.confirm({
    title: 'Close Vacancy',
    message: 'Are you sure you want to close this job vacancy? Job seekers will no longer be able to view or submit new applications for it.',
    confirmText: 'Close Vacancy',
    onConfirm: async () => {
      try {
        await Api.patch(CONSTANTS.API_ENDPOINTS.VACANCIES.CLOSE(id));
        Toast.success('Vacancy closed successfully.');
        await loadVacancies();
      } catch (err) {
        Toast.error(err.message || 'Failed to close vacancy');
      }
    }
  });
}
