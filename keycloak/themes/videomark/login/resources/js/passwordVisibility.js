console.log('passwordVisibility.js loaded');

document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM loaded, searching for elements...');
    
    // Try multiple selectors to find password inputs and buttons
    const passwordInputs = document.querySelectorAll('input[type="password"]');
    const toggleButtons = document.querySelectorAll('[data-password-toggle], button[aria-controls*="password"]');
    
    console.log('Found password inputs:', passwordInputs.length);
    console.log('Found toggle buttons:', toggleButtons.length);
    
    passwordInputs.forEach((passwordInput, index) => {
        console.log(`Password input ${index}:`, passwordInput.id, passwordInput);
    });
    
    toggleButtons.forEach((button, index) => {
        console.log(`Toggle button ${index}:`, button);
        const targetId = button.getAttribute('aria-controls');
        const passwordInput = document.getElementById(targetId);
        const inputGroup = button.closest('.kcInputGroup, [class*="kcInputGroup"], div');
        
        console.log(`Button ${index} controls:`, targetId);
        console.log(`Found input:`, passwordInput);
        console.log(`Found input group:`, inputGroup);
        
        if (passwordInput && button) {
            setupPasswordToggle(passwordInput, button, inputGroup);
        }
    });
});

function setupPasswordToggle(passwordInput, toggleButton, inputGroup) {
    console.log('Setting up password toggle for:', passwordInput.id);
    
    // Password toggle functionality
    toggleButton.addEventListener('click', function(e) {
        e.preventDefault();
        console.log('Toggle button clicked for:', passwordInput.id);
        
        const isPassword = passwordInput.type === 'password';
        passwordInput.type = isPassword ? 'text' : 'password';
        
        console.log('Password type changed to:', passwordInput.type);
        
        // Update icon
        const icon = this.querySelector('i');
        console.log('Icon element:', icon);
        
        if (icon) {
            const hideClass = this.getAttribute('data-icon-hide') || 'fa fa-eye-slash';
            const showClass = this.getAttribute('data-icon-show') || 'fa fa-eye';
            
            if (isPassword) {
                icon.className = hideClass;
                this.setAttribute('aria-label', this.getAttribute('data-label-hide') || 'Hide password');
            } else {
                icon.className = showClass;
                this.setAttribute('aria-label', this.getAttribute('data-label-show') || 'Show password');
            }
            console.log('Icon class updated to:', icon.className);
        }
    });
    
    // Focus handling only if input group exists
    if (inputGroup) {
        console.log('Setting up focus handlers for:', passwordInput.id);
        
        passwordInput.addEventListener('focus', function() {
            console.log('Input focused:', passwordInput.id);
            inputGroup.classList.remove('button-focused');
            inputGroup.classList.add('input-focused');
        });
        
        passwordInput.addEventListener('blur', function() {
            console.log('Input blurred:', passwordInput.id);
            inputGroup.classList.remove('input-focused');
        });
        
        toggleButton.addEventListener('focus', function() {
            console.log('Button focused for:', passwordInput.id);
            inputGroup.classList.remove('input-focused');
            inputGroup.classList.add('button-focused');
        });
        
        toggleButton.addEventListener('blur', function() {
            console.log('Button blurred for:', passwordInput.id);
            inputGroup.classList.remove('button-focused');
        });
    } else {
        console.log('No input group found for:', passwordInput.id);
    }
}
