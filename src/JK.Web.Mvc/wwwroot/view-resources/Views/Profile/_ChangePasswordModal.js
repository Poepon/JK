(function () {
    app.modals.ChangePasswordModal = function () {
        var _profileService = abp.services.app.profile;

        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form[name=ChangePasswordModalForm]');
            _$form.validate();
        };

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }
            abp.ui.setBusy(_$form);
            _profileService.changePassword(_$form.serializeFormToObject())
                .done(function () {
                    abp.notify.info('密码修改成功。');
                    _modalManager.close();
                }).always(function () {
                    abp.ui.clearBusy(_$form);
                });
        };
    };
})();