/**
 * ByteForce System Constants & API Route Maps
 */
const CONSTANTS = {
  STORAGE_KEYS: {
    TOKEN: 'byteforce_auth_token',
    USER: 'byteforce_current_user'
  },
  ROLES: {
    JOB_SEEKER: 'JobSeeker',
    EMPLOYER: 'Employer',
    ADMINISTRATOR: 'Administrator'
  },
  ROUTES: {
    PUBLIC: {
      INDEX: '/index.html',
      LOGIN: '/auth/login.html',
      REGISTER_JOBSEEKER: '/auth/register-jobseeker.html',
      REGISTER_EMPLOYER: '/auth/register-employer.html'
    },
    JOB_SEEKER: {
      DASHBOARD: '/jobseeker/dashboard.html',
      PROFILE: '/jobseeker/profile.html',
      SKILLS: '/jobseeker/skills.html',
      CV: '/jobseeker/cv.html',
      JOBS: '/jobseeker/jobs.html',
      JOB_DETAILS: '/jobseeker/job-details.html',
      APPLICATIONS: '/jobseeker/applications.html',
      CONTACT_REQUESTS: '/jobseeker/contact-requests.html',
      NOTIFICATIONS: '/jobseeker/notifications.html'
    },
    EMPLOYER: {
      DASHBOARD: '/employer/dashboard.html',
      PROFILE: '/employer/profile.html',
      VACANCIES: '/employer/vacancies.html',
      VACANCY_CREATE: '/employer/vacancy-create.html',
      VACANCY_EDIT: '/employer/vacancy-edit.html',
      VACANCY_DETAILS: '/employer/vacancy-details.html',
      APPLICATIONS: '/employer/applications.html',
      CANDIDATE_DETAILS: '/employer/candidate-details.html',
      CONTACT_REQUESTS: '/employer/contact-requests.html',
      NOTIFICATIONS: '/employer/notifications.html'
    },
    ADMIN: {
      DASHBOARD: '/admin/dashboard.html',
      USERS: '/admin/users.html',
      VACANCIES: '/admin/vacancies.html',
      APPLICATIONS: '/admin/applications.html',
      NOTIFICATIONS: '/admin/notifications.html'
    }
  },
  API_ENDPOINTS: {
    AUTH: {
      REGISTER_JOBSEEKER: '/api/auth/register/jobseeker',
      REGISTER_EMPLOYER: '/api/auth/register/employer',
      LOGIN: '/api/auth/login',
      ME: '/api/auth/me'
    },
    JOBSEEKER: {
      PROFILE: '/api/jobseeker/profile',
      DASHBOARD: '/api/jobseeker/profile/dashboard',
      SKILLS: '/api/jobseeker/profile/skills'
    },
    CV: {
      BASE: '/api/cv',
      DOWNLOAD: '/api/cv/download'
    },
    EMPLOYER: {
      PROFILE: '/api/employer/profile'
    },
    VACANCIES: {
      BASE: '/api/vacancies',
      CLOSE: (id) => `/api/vacancies/${id}/close`,
      APPLICANTS: (id) => `/api/vacancies/${id}/applicants`
    },
    JOBS: {
      SEARCH: '/api/jobs',
      DETAILS: (id) => `/api/jobs/${id}`,
      MATCH: (id) => `/api/jobs/${id}/match`,
      APPLY: (id) => `/api/jobs/${id}/applications`
    },
    APPLICATIONS: {
      MINE: '/api/applications/mine',
      UPDATE_STATUS: (id) => `/api/applications/${id}/status`
    },
    CONTACT_REQUESTS: {
      BASE: '/api/contact-requests',
      EMPLOYER_LIST: '/api/contact-requests/employer',
      JOBSEEKER_LIST: '/api/contact-requests/jobseeker',
      RESPOND: (id) => `/api/contact-requests/${id}/respond`,
      CONTACT_DETAILS: (id) => `/api/contact-requests/${id}/contact-details`
    },
    NOTIFICATIONS: {
      MINE: '/api/notifications',
      UNREAD_COUNT: '/api/notifications/unread-count',
      MARK_READ: (id) => `/api/notifications/${id}/read`,
      MARK_ALL_READ: '/api/notifications/read-all'
    },
    SKILLS: {
      BASE: '/api/skills'
    },
    ADMIN: {
      DASHBOARD: '/api/admin/dashboard',
      USERS: '/api/admin/users',
      UPDATE_USER_STATUS: (id) => `/api/admin/users/${id}/status`
    }
  }
};
