/**
 * Auth Manager Service
 */
const Auth = {
  async login(email, password) {
    const response = await Api.post(CONSTANTS.API_ENDPOINTS.AUTH.LOGIN, { email, password });
    if (response && response.token) {
      Storage.setToken(response.token);
      Storage.setCurrentUser(response.user);
    }
    return response;
  },

  async registerJobSeeker(data) {
    return await Api.post(CONSTANTS.API_ENDPOINTS.AUTH.REGISTER_JOBSEEKER, data);
  },

  async registerEmployer(data) {
    return await Api.post(CONSTANTS.API_ENDPOINTS.AUTH.REGISTER_EMPLOYER, data);
  },

  async fetchMe() {
    try {
      const user = await Api.get(CONSTANTS.API_ENDPOINTS.AUTH.ME);
      if (user) {
        Storage.setCurrentUser(user);
      }
      return user;
    } catch (err) {
      Storage.clearSession();
      return null;
    }
  },

  logout() {
    Storage.clearSession();
    window.location.href = CONSTANTS.ROUTES.PUBLIC.LOGIN;
  },

  redirectAfterLogin(user) {
    if (!user || !user.role) {
      window.location.href = CONSTANTS.ROUTES.PUBLIC.INDEX;
      return;
    }
    switch (user.role) {
      case CONSTANTS.ROLES.ADMINISTRATOR:
        window.location.href = CONSTANTS.ROUTES.ADMIN.DASHBOARD;
        break;
      case CONSTANTS.ROLES.EMPLOYER:
        window.location.href = CONSTANTS.ROUTES.EMPLOYER.DASHBOARD;
        break;
      case CONSTANTS.ROLES.JOB_SEEKER:
        window.location.href = CONSTANTS.ROUTES.JOB_SEEKER.DASHBOARD;
        break;
      default:
        window.location.href = CONSTANTS.ROUTES.PUBLIC.INDEX;
    }
  }
};
