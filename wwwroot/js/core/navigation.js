/**
 * Dynamic Sidebar, Header Shell Renderer, and Active Navigation Highlighter
 */
const Navigation = {
  renderShell(role, activeRouteKey) {
    const user = Storage.getCurrentUser() || { fullName: 'User', email: '', role: role };
    const initials = UI.getInitials(user.fullName);

    // Sidebar items by role
    let navItems = [];
    if (role === CONSTANTS.ROLES.JOB_SEEKER) {
      navItems = [
        { key: 'dashboard', label: 'Dashboard', icon: UI.icons.dashboard, route: CONSTANTS.ROUTES.JOB_SEEKER.DASHBOARD },
        { key: 'jobs', label: 'Browse Jobs', icon: UI.icons.search, route: CONSTANTS.ROUTES.JOB_SEEKER.JOBS },
        { key: 'applications', label: 'My Applications', icon: UI.icons.inbox, route: CONSTANTS.ROUTES.JOB_SEEKER.APPLICATIONS },
        { key: 'contact-requests', label: 'Contact Requests', icon: UI.icons.mail, route: CONSTANTS.ROUTES.JOB_SEEKER.CONTACT_REQUESTS },
        { key: 'profile', label: 'My Profile', icon: UI.icons.user, route: CONSTANTS.ROUTES.JOB_SEEKER.PROFILE },
        { key: 'skills', label: 'My Skills', icon: UI.icons.skills, route: CONSTANTS.ROUTES.JOB_SEEKER.SKILLS },
        { key: 'cv', label: 'CV Document', icon: UI.icons.cv, route: CONSTANTS.ROUTES.JOB_SEEKER.CV },
        { key: 'notifications', label: 'Notifications', icon: UI.icons.bell, route: CONSTANTS.ROUTES.JOB_SEEKER.NOTIFICATIONS }
      ];
    } else if (role === CONSTANTS.ROLES.EMPLOYER) {
      navItems = [
        { key: 'dashboard', label: 'Dashboard', icon: UI.icons.dashboard, route: CONSTANTS.ROUTES.EMPLOYER.DASHBOARD },
        { key: 'vacancies', label: 'My Vacancies', icon: UI.icons.briefcase, route: CONSTANTS.ROUTES.EMPLOYER.VACANCIES },
        { key: 'vacancy-create', label: 'Post a Job', icon: UI.icons.plus, route: CONSTANTS.ROUTES.EMPLOYER.VACANCY_CREATE },
        { key: 'contact-requests', label: 'Contact Requests', icon: UI.icons.mail, route: CONSTANTS.ROUTES.EMPLOYER.CONTACT_REQUESTS },
        { key: 'profile', label: 'Company Profile', icon: UI.icons.building, route: CONSTANTS.ROUTES.EMPLOYER.PROFILE },
        { key: 'notifications', label: 'Notifications', icon: UI.icons.bell, route: CONSTANTS.ROUTES.EMPLOYER.NOTIFICATIONS }
      ];
    } else if (role === CONSTANTS.ROLES.ADMINISTRATOR) {
      navItems = [
        { key: 'dashboard', label: 'System Overview', icon: UI.icons.dashboard, route: CONSTANTS.ROUTES.ADMIN.DASHBOARD },
        { key: 'users', label: 'User Directory', icon: UI.icons.users, route: CONSTANTS.ROUTES.ADMIN.USERS },
        { key: 'vacancies', label: 'Vacancy Audit', icon: UI.icons.briefcase, route: CONSTANTS.ROUTES.ADMIN.VACANCIES },
        { key: 'applications', label: 'Applications Log', icon: UI.icons.inbox, route: CONSTANTS.ROUTES.ADMIN.APPLICATIONS },
        { key: 'notifications', label: 'System Logs', icon: UI.icons.bell, route: CONSTANTS.ROUTES.ADMIN.NOTIFICATIONS }
      ];
    }

    const sidebarNavHtml = navItems.map(item => `
      <a href="${item.route}" class="nav-item ${item.key === activeRouteKey ? 'active' : ''}">
        <span class="nav-icon">${item.icon}</span>
        <span>${item.label}</span>
      </a>
    `).join('');

    const sidebarHtml = `
      <aside class="sidebar" id="appSidebar">
        <div class="sidebar-header">
          <a href="${CONSTANTS.ROUTES.PUBLIC.INDEX}" class="brand-logo">
            <div class="brand-icon"><img src="/images/logo-icon.svg" alt="ByteForce Logo" width="36" height="36"></div>
            <span>Byte<span style="color: var(--primary);">Force</span></span>
          </a>
        </div>
        <div class="nav-section-title">${role} Portal</div>
        <nav class="sidebar-nav">
          ${sidebarNavHtml}
        </nav>
      </aside>
    `;

    const headerHtml = `
      <header class="app-header">
        <div class="header-left">
          <button class="sidebar-toggle" id="sidebarToggle" aria-label="Toggle Navigation">
            <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="18" x2="21" y2="18"/></svg>
          </button>
          <div class="header-title">${navItems.find(i => i.key === activeRouteKey)?.label || 'Portal'}</div>
        </div>
        <div class="header-right">
          <button class="notification-bell-btn" id="notifBellBtn" title="Notifications">
            ${UI.icons.bell}
            <span class="unread-badge-dot hidden" id="headerUnreadDot">0</span>
          </button>
          <div class="user-dropdown">
            <button class="user-menu-btn" id="userMenuBtn">
              <div class="avatar">${initials}</div>
              <div class="user-info">
                <div class="user-name">${UI.escapeHtml(user.fullName)}</div>
                <div class="user-role-badge">${user.role}</div>
              </div>
              <span style="font-size: 0.75rem;">▼</span>
            </button>
            <div class="dropdown-menu" id="userDropdownMenu">
              <div class="dropdown-item" style="font-weight: 600; color: var(--text-muted); pointer-events: none;">
                ${UI.escapeHtml(user.email)}
              </div>
              <div class="dropdown-divider"></div>
              <a href="#" class="dropdown-item" id="logoutBtn">
                <span>${UI.icons.logout}</span> Log Out
              </a>
            </div>
          </div>
        </div>
      </header>
    `;

    // Inject into container if placeholders exist or wrap existing page content
    const body = document.body;
    const existingContent = body.innerHTML;
    
    body.innerHTML = `
      <div class="app-container">
        ${sidebarHtml}
        <div class="main-wrapper">
          ${headerHtml}
          <main class="page-content">
            ${existingContent}
          </main>
        </div>
      </div>
    `;

    this.bindEvents();
    NotificationsComponent.initHeaderBell();
  },

  bindEvents() {
    const toggleBtn = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('appSidebar');
    if (toggleBtn && sidebar) {
      toggleBtn.addEventListener('click', () => {
        sidebar.classList.toggle('open');
      });
    }

    const userMenuBtn = document.getElementById('userMenuBtn');
    const userDropdownMenu = document.getElementById('userDropdownMenu');
    if (userMenuBtn && userDropdownMenu) {
      userMenuBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        userDropdownMenu.classList.toggle('show');
      });

      document.addEventListener('click', () => {
        userDropdownMenu.classList.remove('show');
      });
    }

    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) {
      logoutBtn.addEventListener('click', (e) => {
        e.preventDefault();
        Modal.confirm({
          title: 'Confirm Logout',
          message: 'Are you sure you want to log out of ByteForce?',
          confirmText: 'Log Out',
          onConfirm: () => Auth.logout()
        });
      });
    }
  }
};
