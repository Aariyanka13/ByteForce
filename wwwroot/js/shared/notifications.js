/**
 * Header Unread Notification Counter Poller
 */
const NotificationsComponent = {
  async initHeaderBell() {
    const dot = document.getElementById('headerUnreadDot');
    if (!dot) return;

    try {
      const res = await Api.get(CONSTANTS.API_ENDPOINTS.NOTIFICATIONS.UNREAD_COUNT);
      if (res && res.count > 0) {
        dot.textContent = res.count > 99 ? '99+' : res.count;
        dot.classList.remove('hidden');
      } else {
        dot.classList.add('hidden');
      }
    } catch {
      dot.classList.add('hidden');
    }

    const bell = document.getElementById('notifBellBtn');
    if (bell) {
      bell.onclick = () => {
        const user = Storage.getCurrentUser();
        if (user && user.role === CONSTANTS.ROLES.EMPLOYER) {
          window.location.href = CONSTANTS.ROUTES.EMPLOYER.NOTIFICATIONS;
        } else if (user && user.role === CONSTANTS.ROLES.ADMINISTRATOR) {
          window.location.href = CONSTANTS.ROUTES.ADMIN.NOTIFICATIONS;
        } else {
          window.location.href = CONSTANTS.ROUTES.JOB_SEEKER.NOTIFICATIONS;
        }
      };
    }
  }
};
