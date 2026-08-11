document.addEventListener('DOMContentLoaded', () => {
  Guards.requireRole(CONSTANTS.ROLES.ADMINISTRATOR);
  Navigation.renderShell(CONSTANTS.ROLES.ADMINISTRATOR, 'applications');

  loadAdminApplications();
});

async function loadAdminApplications() {
  const tbody = document.getElementById('adminAppsTableBody');
  tbody.innerHTML = Loader.renderTableSkeleton(4, 4);

  try {
    const list = await Api.get(CONSTANTS.API_ENDPOINTS.ADMIN.DASHBOARD);
    tbody.innerHTML = `
      <tr>
        <td colspan="4" class="text-center py-6 text-muted">
          Platform Metrics Summary: <strong>${list.totalApplications || 0}</strong> total applications submitted. <strong>${list.totalContactRequests || 0}</strong> candidate connections established.
        </td>
      </tr>
    `;
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-danger text-center py-4">Failed to load application logs.</td></tr>`;
  }
}
