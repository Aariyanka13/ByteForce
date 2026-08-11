document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.EMPLOYER);
  Navigation.renderShell(CONSTANTS.ROLES.EMPLOYER, 'profile');

  const form = document.getElementById('employerProfileForm');
  const companyNameInput = document.getElementById('companyName');
  const industryInput = document.getElementById('industry');
  const locationInput = document.getElementById('location');
  const websiteInput = document.getElementById('website');
  const descriptionInput = document.getElementById('description');
  const saveBtn = document.getElementById('saveEmployerProfileBtn');

  // Load existing profile
  try {
    const profile = await Api.get(CONSTANTS.API_ENDPOINTS.EMPLOYER.PROFILE);
    if (profile) {
      companyNameInput.value = profile.companyName || '';
      industryInput.value = profile.industry || '';
      locationInput.value = profile.location || '';
      websiteInput.value = profile.website || '';
      descriptionInput.value = profile.description || '';
    }
  } catch (err) {
    Toast.error('Failed to load company profile.');
  }

  // Update profile
  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (!Validators.validateForm(form)) return;

    saveBtn.disabled = true;

    try {
      await Api.put(CONSTANTS.API_ENDPOINTS.EMPLOYER.PROFILE, {
        companyName: companyNameInput.value.trim(),
        industry: industryInput.value.trim(),
        location: locationInput.value.trim(),
        website: websiteInput.value.trim(),
        description: descriptionInput.value.trim()
      });

      Toast.success('Company profile updated!');
    } catch (err) {
      Toast.error(err.message || 'Failed to update company profile');
    } finally {
      saveBtn.disabled = false;
    }
  });
});
