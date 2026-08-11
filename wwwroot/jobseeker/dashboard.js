document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'dashboard');

  try {
    const data = await Api.get(CONSTANTS.API_ENDPOINTS.JOBSEEKER.DASHBOARD);
    
    // Completeness
    const completenessObj = data.profileCompleteness || {};
    const pct = Math.round(completenessObj.percentage !== undefined ? completenessObj.percentage : (data.completenessPercentage || 0));
    const missingItems = completenessObj.missingItems || data.missingItems || [];
    const bar = document.getElementById('completenessBar');
    const badge = document.getElementById('completenessBadge');
    const text = document.getElementById('completenessText');
    const missingBox = document.getElementById('missingItemsBox');
    const missingList = document.getElementById('missingItemsList');

    bar.style.width = `${pct}%`;
    badge.textContent = `${pct}% Complete`;
    
    if (pct === 100) {
      text.textContent = 'Great job! Your candidate profile is 100% complete and ready for job applications.';
      badge.className = 'badge badge-selected';
    } else {
      text.textContent = 'Please complete missing profile information to qualify for matching.';
      badge.className = 'badge badge-underreview';
    }

    if (missingItems && missingItems.length > 0) {
      missingBox.classList.remove('hidden');
      missingList.innerHTML = missingItems.map(item => `
        <li class="badge badge-rejected"><svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg> ${UI.escapeHtml(item)}</li>
      `).join('');
    } else {
      missingBox.classList.add('hidden');
    }

    // Stats
    const skillsNum = data.skillCount !== undefined ? data.skillCount : (data.selectedSkillsCount || 0);
    document.getElementById('skillsCount').textContent = skillsNum;
    document.getElementById('appsCount').textContent = data.totalApplications || 0;
    
    const cvBadge = document.getElementById('cvStatusBadge');
    if (data.hasCv) {
      cvBadge.innerHTML = `<span class="badge badge-selected">Uploaded</span>`;
    } else {
      cvBadge.innerHTML = `<span class="badge badge-rejected">Missing</span>`;
    }

    // Load recent notifications
    loadRecentNotifications();
  } catch (err) {
    Toast.error('Failed to load dashboard metrics');
  }
});

async function loadRecentNotifications() {
  const notifsList = document.getElementById('recentNotifsList');
  try {
    const list = await Api.get(CONSTANTS.API_ENDPOINTS.NOTIFICATIONS.MINE);
    if (!list || list.length === 0) {
      notifsList.innerHTML = `<div class="text-muted text-sm py-4 text-center">No notifications yet.</div>`;
      return;
    }

    const recent = list.slice(0, 3);
    notifsList.innerHTML = recent.map(n => `
      <div class="flex items-start gap-3 p-2 border-bottom" style="font-size: var(--font-size-sm);">
        <span>${UI.icons.bell}</span>
        <div style="flex: 1;">
          <div class="font-semibold text-main">${UI.escapeHtml(n.title)}</div>
          <div class="text-muted">${UI.escapeHtml(n.message)}</div>
          <div class="text-xs text-light mt-1">${UI.formatDateTime(n.createdAt)}</div>
        </div>
      </div>
    `).join('');
  } catch {
    notifsList.innerHTML = `<div class="text-muted text-sm py-4 text-center">Unable to load notifications.</div>`;
  }
}
