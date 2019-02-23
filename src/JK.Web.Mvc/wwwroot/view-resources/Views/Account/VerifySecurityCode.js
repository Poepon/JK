$(function () {
    var $form = $('.verify-security-code-form');

    $form.validate();

    $form.find('input').keypress(function (e) {
        if (e.which === 13) {
            if ($('.forget-form').valid()) {
                $('.forget-form').submit();
            }
            return false;
        }
    });

    $form.submit(function (e) {
        e.preventDefault();

        if (!$form.valid()) {
            return;
        }

        abp.ui.setBusy(
            null,
            abp.ajax({
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                url: $form.attr('action'),
                data: $form.serialize()
            }).done(function () {
                //no need to handle result since redirects and errors are automatically handled

            })
        );
    });
});