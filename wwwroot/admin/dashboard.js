document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.ADMINISTRATOR);
  Navigation.renderShell(CONSTANTS.ROLES.ADMINISTRATOR, 'dashboard');

  try {
    const data = await Api.get(CONSTANTS.API_ENDPOINTS.ADMIN.DASHBOARD);
    document.getElementById('statTotalUsers').textContent = data.totalUsers || 0;
    document.getElementById('statJobSeekers').textContent = data.totalJobSeekers || 0;
    document.getElementById('statEmployers').textContent = data.totalEmployers || 0;
    document.getElementById('statVacancies').textContent = data.totalVacancies || 0;
    document.getElementById('statApplications').textContent = data.totalApplications || 0;
    document.getElementById('statSelected').textContent = data.totalContactRequests || 0;
    document.getElementById('statActive').textContent = data.activeUsers || 0;
    document.getElementById('statDisabled').textContent = data.disabledUsers || 0;
  } catch (err) {
    Toast.error('Failed to load admin telemetry dashboard.');
  }
});
