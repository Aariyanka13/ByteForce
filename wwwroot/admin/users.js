document.addEventListener('DOMContentLoaded', () => {
  Guards.requireRole(CONSTANTS.ROLES.ADMINISTRATOR);
  Navigation.renderShell(CONSTANTS.ROLES.ADMINISTRATOR, 'users');

  const searchInput = document.getElementById('userSearchInput');
  const roleSelect = document.getElementById('roleFilter');
  const statusSelect = document.getElementById('statusFilter');

  let timer = null;
  const triggerSearch = () => {
    clearTimeout(timer);
    timer = setTimeout(() => loadUsers(), 300);
  };

  searchInput.addEventListener('input', triggerSearch);
  roleSelect.addEventListener('change', loadUsers);
  statusSelect.addEventListener('change', loadUsers);

  loadUsers();
});

async function loadUsers() {
  const tbody = document.getElementById('usersTableBody');
  const search = document.getElementById('userSearchInput').value.trim();
  const role = document.getElementById('roleFilter').value;
  const statusStr = document.getElementById('statusFilter').value;
  const isActive = statusStr !== '' ? (statusStr === 'true') : null;

  tbody.innerHTML = Loader.renderTableSkeleton(6, 5);

  try {
    const list = await Api.get(CONSTANTS.API_ENDPOINTS.ADMIN.USERS, {
      search: search || null,
      role: role || null,
      isActive: isActive
    });

    if (!list || list.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="6" class="text-center py-6 text-muted">
            No registered users found matching the selected filters.
          </td>
        </tr>
      `;
      return;
    }

    const currentAdmin = Storage.getCurrentUser();

    tbody.innerHTML = list.map(u => {
      const isSelf = currentAdmin && currentAdmin.id === u.id;
      return `
        <tr>
          <td class="font-semibold text-main">${UI.icons.user} ${UI.escapeHtml(u.fullName)} ${isSelf ? '<span class="text-xs text-primary">(You)</span>' : ''}</td>
          <td>${UI.escapeHtml(u.email)}</td>
          <td><span class="badge badge-applied">${UI.escapeHtml(u.role)}</span></td>
          <td>${UI.formatDate(u.createdAt)}</td>
          <td>${u.isActive ? '<span class="badge badge-selected">Active</span>' : '<span class="badge badge-rejected">Disabled</span>'}</td>
          <td style="text-align: right;">
            ${isSelf ? `
              <span class="text-xs text-muted">Self Account (Protected)</span>
            ` : `
              <button class="btn ${u.isActive ? 'btn-secondary text-danger' : 'btn-primary'} btn-sm" onclick="toggleUserStatus(${u.id}, '${UI.escapeHtml(u.fullName)}', ${!u.isActive})">
                ${u.isActive ? 'Disable Account' : 'Enable Account'}
              </button>
            `}
          </td>
        </tr>
      `;
    }).join('');
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="6" class="text-danger text-center py-4">Failed to load user directory.</td></tr>`;
  }
}

function toggleUserStatus(userId, fullName, newStatus) {
  const actionText = newStatus ? 'Enable' : 'Disable';
  Modal.confirm({
    title: `${actionText} User Account`,
    message: `Are you sure you want to ${actionText.toLowerCase()} access for ${fullName}? ${newStatus ? 'The user will regain login access.' : 'The user will be immediately blocked from logging in.'}`,
    confirmText: `${actionText} Account`,
    onConfirm: async () => {
      try {
        await Api.put(CONSTANTS.API_ENDPOINTS.ADMIN.UPDATE_USER_STATUS(userId), { isActive: newStatus });
        Toast.success(`User account ${actionText.toLowerCase()}d successfully.`);
        await loadUsers();
      } catch (err) {
        Toast.error(err.message || `Failed to ${actionText.toLowerCase()} user account`);
      }
    }
  });
}
