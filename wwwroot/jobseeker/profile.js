document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'profile');

  const profileForm = document.getElementById('profileForm');
  const fullNameInput = document.getElementById('fullName');
  const emailInput = document.getElementById('email');
  const phoneInput = document.getElementById('phone');
  const locationInput = document.getElementById('location');
  const experienceInput = document.getElementById('experience');
  const educationInput = document.getElementById('education');
  const summaryInput = document.getElementById('summary');
  const saveBtn = document.getElementById('saveBtn');

  // Load profile
  try {
    const profile = await Api.get(CONSTANTS.API_ENDPOINTS.JOBSEEKER.PROFILE);
    const currentUser = Storage.getCurrentUser() || {};
    fullNameInput.value = profile.fullName || currentUser.fullName || '';
    emailInput.value = profile.email || currentUser.email || '';
    phoneInput.value = profile.phone || '';
    locationInput.value = profile.location || '';
    experienceInput.value = profile.totalExperienceYears !== null ? profile.totalExperienceYears : '';
    if (profile.educationLevel) {
      educationInput.value = profile.educationLevel;
    }
    summaryInput.value = profile.profileSummary || '';
  } catch (err) {
    Toast.error('Failed to load profile details');
  }

  // Update profile
  profileForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (!Validators.validateForm(profileForm)) return;

    saveBtn.disabled = true;

    try {
      await Api.put(CONSTANTS.API_ENDPOINTS.JOBSEEKER.PROFILE, {
        phone: phoneInput.value.trim(),
        location: locationInput.value.trim(),
        totalExperienceYears: parseFloat(experienceInput.value) || 0,
        educationLevel: educationInput.value,
        profileSummary: summaryInput.value.trim()
      });

      Toast.success('Profile updated successfully!');
    } catch (err) {
      Toast.error(err.message || 'Failed to update profile');
    } finally {
      saveBtn.disabled = false;
    }
  });
});
