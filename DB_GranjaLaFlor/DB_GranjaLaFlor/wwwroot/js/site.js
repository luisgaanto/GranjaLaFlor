// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Separation of Concerns (SoC), Single Responsibility Principle (SRP): best practice as it keeps reposabilities with each component. So .js goes here. 

document.addEventListener("DOMContentLoaded", function () {
    const alerts = document.querySelectorAll(".auto-close-alert");

    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        }, 4000);
    });
});