/**
 * Client-side Route Authorization Guards
 */
const Guards = {
  requireAuth() {
    const token = Storage.getToken();
    const user = Storage.getCurrentUser();
    if (!token || !user) {
      Storage.clearSession();
      window.location.href = CONSTANTS.ROUTES.PUBLIC.LOGIN;
      return false;
    }
    return true;
  },

  requireRole(requiredRole) {
    if (!this.requireAuth()) return false;

    const user = Storage.getCurrentUser();
    if (user.role !== requiredRole) {
      console.warn(`Access denied. Role '${user.role}' is not allowed to access '${requiredRole}' resource.`);
      Auth.redirectAfterLogin(user);
      return false;
    }
    return true;
  },

  guestOnly() {
    const token = Storage.getToken();
    const user = Storage.getCurrentUser();
    if (token && user) {
      Auth.redirectAfterLogin(user);
      return false;
    }
    return true;
  }
};
