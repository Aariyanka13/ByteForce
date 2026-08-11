/**
 * Input Validators for Client-side Forms
 */
const Validators = {
  isValidEmail(email) {
    if (!email) return false;
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(String(email).toLowerCase());
  },

  isStrongPassword(password) {
    if (!password) return false;
    return password.length >= 6;
  },

  validateForm(formElement) {
    let isValid = true;
    const inputs = formElement.querySelectorAll('input[required], select[required], textarea[required]');
    inputs.forEach(input => {
      const val = input.value ? input.value.trim() : '';
      if (!val) {
        input.classList.add('is-invalid');
        isValid = false;
      } else {
        input.classList.remove('is-invalid');
      }

      if (input.type === 'email' && val) {
        if (!this.isValidEmail(val)) {
          input.classList.add('is-invalid');
          isValid = false;
        }
      }
    });

    return isValid;
  }
};
