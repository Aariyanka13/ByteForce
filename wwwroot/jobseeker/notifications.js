document.addEventListener('DOMContentLoaded', () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'notifications');

  const markAllBtn = document.getElementById('markAllReadBtn');
  markAllBtn.addEventListener('click', async () => {
    try {
      await Api.put(CONSTANTS.API_ENDPOINTS.NOTIFICATIONS.MARK_ALL_READ);
      Toast.success('All notifications marked as read');
      await loadNotifications();
      NotificationsComponent.initHeaderBell();
    } catch {
      Toast.error('Failed to mark all as read');
    }
  });

  loadNotifications();
});

async function loadNotifications() {
  const container = document.getElementById('notificationsList');

  try {
    const list = await Api.get(CONSTANTS.API_ENDPOINTS.NOTIFICATIONS.MINE);

    if (!list || list.length === 0) {
      container.innerHTML = `
        <div class="empty-state py-8">
          <div class="empty-title">No Notifications</div>
          <div class="empty-text text-sm">You have no notification records at this time.</div>
        </div>
      `;
      return;
    }

    container.innerHTML = list.map(item => `
      <div class="p-3 border rounded flex items-start gap-3 ${item.isRead ? '' : 'bg-primary-light'}" style="transition: background-color var(--transition-fast);">
        <span>${UI.icons.bell}</span>
        <div style="flex: 1;">
          <div class="flex items-center justify-between mb-1">
            <span class="font-semibold text-main text-sm">${UI.escapeHtml(item.title)}</span>
            <span class="text-xs text-light">${UI.formatDateTime(item.createdAt)}</span>
          </div>
          <p class="text-sm mb-0">${UI.escapeHtml(item.message)}</p>
        </div>
        ${!item.isRead ? `
          <button class="btn btn-ghost btn-sm text-xs" onclick="markRead(${item.id})">Mark Read</button>
        ` : ''}
      </div>
    `).join('');
  } catch (err) {
    container.innerHTML = `<div class="text-danger text-center py-4">Failed to load notifications.</div>`;
  }
}

async function markRead(id) {
  try {
    await Api.put(CONSTANTS.API_ENDPOINTS.NOTIFICATIONS.MARK_READ(id));
    await loadNotifications();
    NotificationsComponent.initHeaderBell();
  } catch {
    Toast.error('Failed to mark notification as read');
  }
}
