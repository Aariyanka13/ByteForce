/**
 * Storage Manager for JWT tokens and user session state
 */
const Storage = {
  getToken() {
    return localStorage.getItem(CONSTANTS.STORAGE_KEYS.TOKEN);
  },
  setToken(token) {
    if (token) {
      localStorage.setItem(CONSTANTS.STORAGE_KEYS.TOKEN, token);
    }
  },
  removeToken() {
    localStorage.removeItem(CONSTANTS.STORAGE_KEYS.TOKEN);
  },
  getCurrentUser() {
    const raw = localStorage.getItem(CONSTANTS.STORAGE_KEYS.USER);
    if (!raw) return null;
    try {
      return JSON.parse(raw);
    } catch {
      return null;
    }
  },
  setCurrentUser(user) {
    if (user) {
      localStorage.setItem(CONSTANTS.STORAGE_KEYS.USER, JSON.stringify(user));
    }
  },
  removeCurrentUser() {
    localStorage.removeItem(CONSTANTS.STORAGE_KEYS.USER);
  },
  clearSession() {
    this.removeToken();
    this.removeCurrentUser();
  }
};
