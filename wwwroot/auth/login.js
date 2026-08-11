document.addEventListener('DOMContentLoaded', () => {
  Guards.guestOnly();

  const loginForm = document.getElementById('loginForm');
  const emailInput = document.getElementById('email');
  const passwordInput = document.getElementById('password');
  const togglePasswordBtn = document.getElementById('togglePasswordBtn');
  const submitBtn = document.getElementById('submitBtn');
  const btnText = document.getElementById('btnText');
  const btnSpinner = document.getElementById('btnSpinner');
  const apiErrorBanner = document.getElementById('apiErrorBanner');

  UI.setupPasswordToggle(passwordInput, togglePasswordBtn);

  loginForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    apiErrorBanner.classList.add('hidden');

    if (!Validators.validateForm(loginForm)) {
      return;
    }

    // Submit state
    submitBtn.disabled = true;
    btnText.classList.add('hidden');
    btnSpinner.classList.remove('hidden');

    try {
      const result = await Auth.login(emailInput.value.trim(), passwordInput.value);
      Toast.success('Login successful! Redirecting...');
      setTimeout(() => {
        Auth.redirectAfterLogin(result.user);
      }, 500);
    } catch (err) {
      apiErrorBanner.textContent = err.message || 'Invalid email or password.';
      apiErrorBanner.classList.remove('hidden');
      Toast.error(err.message || 'Login failed');
    } finally {
      submitBtn.disabled = false;
      btnText.classList.remove('hidden');
      btnSpinner.classList.add('hidden');
    }
  });
});
