// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var message = $('#modalMessage').text().trim(); // Obtener el mensaje del TempData
    if (message.length > 0) {
        // Si hay un mensaje, mostrar el modal
        $('#errorModal').modal('show');
    }
});