// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {
    $('#btnConfirmDelete').click(function (e) {
        e.preventDefault();
        $('#confirmDeleteDialogue').dialog('open');
    });

    $('#confirmDeleteDialogue').dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        draggable: false,
        closeOnEscape: true,
        minWidth: 300,
        maxWidth: 500,
        width: 'auto',
        create: function () {
            $(this).next()
                .find('button')
                .removeClass('ui-button ui-corner-all ui-widget');
        },
        buttons: [
            {
                class: 'btn btn-danger',
                text: 'Confirm',
                click: function () {
                    $('#deleteForm').submit();
                }
            },
            {
                class: 'btn btn-outline-secondary',
                text: 'Cancel',
                click: function () {
                    $(this).dialog('close');
                }
            }
        ]
    });
});

(function ($) {
    var defaultOptions = {
        validClass: 'is-valid',
        errorClass: 'is-invalid',
        highlight: function (element, errorClass, validClass) {
            $(element).closest('.form-group')
                .removeClass(validClass)
                .addClass(errorClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).closest('.form-group')
                .removeClass(errorClass)
                .addClass(validClass);
        }
    };

    $.validator.setDefaults(defaultOptions);

    $.validator.unobtrusive.options = {
        errorClass: defaultOptions.errorClass,
        validClass: defaultOptions.validClass,
    };
})(jQuery);