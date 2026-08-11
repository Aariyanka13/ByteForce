document.addEventListener('DOMContentLoaded', () => {
  Guards.guestOnly();

  const registerForm = document.getElementById('registerForm');
  const fullNameInput = document.getElementById('fullName');
  const emailInput = document.getElementById('email');
  const passwordInput = document.getElementById('password');
  const confirmPasswordInput = document.getElementById('confirmPassword');
  const togglePasswordBtn = document.getElementById('togglePasswordBtn');
  const toggleConfirmPasswordBtn = document.getElementById('toggleConfirmPasswordBtn');
  const confirmFeedback = document.getElementById('confirmFeedback');
  const submitBtn = document.getElementById('submitBtn');
  const btnText = document.getElementById('btnText');
  const btnSpinner = document.getElementById('btnSpinner');
  const apiErrorBanner = document.getElementById('apiErrorBanner');

  UI.setupPasswordToggle(passwordInput, togglePasswordBtn);
  UI.setupPasswordToggle(confirmPasswordInput, toggleConfirmPasswordBtn);

  registerForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    apiErrorBanner.classList.add('hidden');

    let isValid = Validators.validateForm(registerForm);

    if (passwordInput.value !== confirmPasswordInput.value) {
      confirmPasswordInput.classList.add('is-invalid');
      confirmFeedback.textContent = 'Passwords do not match.';
      confirmFeedback.style.display = 'block';
      isValid = false;
    }

    if (!isValid) return;

    submitBtn.disabled = true;
    btnText.classList.add('hidden');
    btnSpinner.classList.remove('hidden');

    try {
      await Auth.registerJobSeeker({
        fullName: fullNameInput.value.trim(),
        email: emailInput.value.trim(),
        password: passwordInput.value,
        confirmPassword: confirmPasswordInput.value
      });

      Toast.success('Account created successfully! Logging you in...');

      // Auto login after registration
      const loginRes = await Auth.login(emailInput.value.trim(), passwordInput.value);
      setTimeout(() => {
        Auth.redirectAfterLogin(loginRes.user);
      }, 500);
    } catch (err) {
      apiErrorBanner.textContent = err.message || 'Registration failed. Please check your inputs.';
      apiErrorBanner.classList.remove('hidden');
      Toast.error(err.message || 'Registration failed');
    } finally {
      submitBtn.disabled = false;
      btnText.classList.remove('hidden');
      btnSpinner.classList.add('hidden');
    }
  });
});
