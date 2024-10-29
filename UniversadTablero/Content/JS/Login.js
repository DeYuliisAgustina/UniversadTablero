document.addEventListener("DOMContentLoaded", () => {
    window.addEventListener('beforeunload', function (event) {
        console.log("La página se está recargando o cerrando.");
    });
    const container = document.querySelector(".container");
    const signupLink = document.querySelector(".signup-link");
    const loginLink = document.querySelector(".login-link");

    // Mover al formulario de registro
    signupLink.addEventListener("click", (e) => {
        e.preventDefault();
        container.classList.add("active");
    });

    // Volver al formulario de login
    loginLink.addEventListener("click", (e) => {
        e.preventDefault();
        container.classList.remove("active");
    });

    // Mostrar/Ocultar contraseña
    const pwFields = document.querySelectorAll(".password");
    const pwShowHide = document.querySelectorAll(".showHidePw");

    pwShowHide.forEach((eyeIcon) => {
        eyeIcon.addEventListener("click", () => {
            pwFields.forEach((pwField) => {
                if (pwField.type === "password") {
                    pwField.type = "text";
                    pwShowHide.forEach((icon) => {
                        icon.classList.replace("uil-eye-slash", "uil-eye");
                    });
                } else {
                    pwField.type = "password";
                    pwShowHide.forEach((icon) => {
                        icon.classList.replace("uil-eye", "uil-eye-slash");
                    });
                }
            });
        });
    });
});
