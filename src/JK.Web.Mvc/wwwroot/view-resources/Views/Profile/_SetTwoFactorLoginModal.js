(function () {
    app.modals.SetTwoFactorLoginModal = function () {

        var _profileService = abp.services.app.profile;

        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            var $modal = _modalManager.getModal();

            $("#ckbEnable").change(function () {
                _profileService.setTwoFactorEnabled({ enable: this.checked });
            });

            var $btnEnableGoogleAuthenticator = $modal.find('#btnEnableGoogleAuthenticator');

            $btnEnableGoogleAuthenticator.click(function () {
                _profileService.updateGoogleAuthenticatorKey()
                    .done(function (result) {
                        $modal.find('.google-authenticator-enable').toggle();
                        $modal.find('img').attr('src', result.qrCodeSetupImageUrl);
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            });
        };
        this.save = function () {

        };
    };
})();