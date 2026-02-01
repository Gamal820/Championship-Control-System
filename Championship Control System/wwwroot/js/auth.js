/* ===================================
   Authentication Pages JavaScript
   File: wwwroot/js/auth.js
   =================================== */

// Password Toggle Function
function togglePassword(inputId, iconElement) {
    const input = document.getElementById(inputId);
    const icon = iconElement.querySelector('i');

    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('bi-eye');
        icon.classList.add('bi-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('bi-eye-slash');
        icon.classList.add('bi-eye');
    }
}

// Password Match Validation
function initPasswordMatchValidation() {
    const password = document.getElementById('password');
    const confirmPassword = document.getElementById('confirmPassword');

    if (confirmPassword && password) {
        confirmPassword.addEventListener('input', function () {
            const passwordValue = password.value;
            const confirmPasswordValue = this.value;

            if (passwordValue === confirmPasswordValue && confirmPasswordValue !== '') {
                this.style.borderColor = '#28a745';
            } else if (confirmPasswordValue !== '') {
                this.style.borderColor = '#dc3545';
            } else {
                this.style.borderColor = '#e0e0e0';
            }
        });
    }
}

// OTP Input Validation (only numbers)
function initOTPValidation() {
    const otpInput = document.querySelector('input[name="OTP"]');

    if (otpInput) {
        // Auto-focus on OTP input
        otpInput.focus();

        // Only allow numbers
        otpInput.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '');
        });

        // Auto-submit on 4 digits (optional)
        otpInput.addEventListener('input', function (e) {
            if (this.value.length === 4) {
                // You can uncomment the line below to auto-submit
                // this.form.submit();
            }
        });
    }
}

// Email Format Validation Helper
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Real-time Email Validation
function initEmailValidation() {
    const emailInputs = document.querySelectorAll('input[type="email"]');

    emailInputs.forEach(input => {
        input.addEventListener('blur', function () {
            if (this.value && !isValidEmail(this.value)) {
                this.classList.add('is-invalid');
            } else {
                this.classList.remove('is-invalid');
            }
        });
    });
}

// Initialize all validations on page load
document.addEventListener('DOMContentLoaded', function () {
    initPasswordMatchValidation();
    initOTPValidation();
    initEmailValidation();

    // Add loading state to submit buttons
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function () {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="bi bi-hourglass-split"></i> جاري المعالجة...';
            }
        });
    });
});

// Password Strength Indicator (Optional Enhancement)
function checkPasswordStrength(password) {
    let strength = 0;

    if (password.length >= 8) strength++;
    if (password.length >= 12) strength++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[^a-zA-Z\d]/.test(password)) strength++;

    return strength;
}

function updatePasswordStrength(inputId, strengthBarId) {
    const input = document.getElementById(inputId);
    const strengthBar = document.getElementById(strengthBarId);

    if (input && strengthBar) {
        input.addEventListener('input', function () {
            const strength = checkPasswordStrength(this.value);
            const percentage = (strength / 5) * 100;

            strengthBar.style.width = percentage + '%';

            if (strength <= 2) {
                strengthBar.className = 'progress-bar bg-danger';
            } else if (strength <= 3) {
                strengthBar.className = 'progress-bar bg-warning';
            } else {
                strengthBar.className = 'progress-bar bg-success';
            }
        });
    }
}

// Prevent Double Submission
function preventDoubleSubmit() {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        let submitted = false;

        form.addEventListener('submit', function (e) {
            if (submitted) {
                e.preventDefault();
                return false;
            }

            submitted = true;
            return true;
        });
    });
}

// Call prevent double submit on load
document.addEventListener('DOMContentLoaded', preventDoubleSubmit);