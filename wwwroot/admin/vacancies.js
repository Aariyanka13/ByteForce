let allVacancies = [];

document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.ADMINISTRATOR);
  Navigation.renderShell(CONSTANTS.ROLES.ADMINISTRATOR, 'vacancies');

  const searchInput = document.getElementById('adminVacancySearch');
  searchInput.addEventListener('input', (e) => {
    const q = e.target.value.toLowerCase().trim();
    const filtered = allVacancies.filter(v => 
      (v.title && v.title.toLowerCase().includes(q)) || 
      (v.location && v.location.toLowerCase().includes(q))
    );
    renderTable(filtered);
  });

  await loadAdminVacancies();
});

async function loadAdminVacancies() {
  const tbody = document.getElementById('adminVacanciesTableBody');
  tbody.innerHTML = Loader.renderTableSkeleton(5, 4);

  try {
    // Admin uses job search endpoint or employer vacancies list
    const res = await Api.get(CONSTANTS.API_ENDPOINTS.JOBS.SEARCH, { page: 1, pageSize: 50 });
    allVacancies = res && res.items ? res.items : [];
    renderTable(allVacancies);
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-danger text-center py-4">Failed to load system vacancies.</td></tr>`;
  }
}

function renderTable(list) {
  const tbody = document.getElementById('adminVacanciesTableBody');
  if (!list || list.length === 0) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-center py-6 text-muted">No vacancies found.</td></tr>`;
    return;
  }

  tbody.innerHTML = list.map(v => `
    <tr>
      <td class="font-semibold text-main">${UI.escapeHtml(v.title)}</td>
      <td>${UI.icons.location} ${UI.escapeHtml(v.location || 'Remote')}</td>
      <td>${v.requiredExperienceYears || 0}+ Years</td>
      <td>${UI.escapeHtml(v.requiredEducationLevel || 'No Requirement')}</td>
      <td><span class="badge badge-selected">Active</span></td>
    </tr>
  `).join('');
}
