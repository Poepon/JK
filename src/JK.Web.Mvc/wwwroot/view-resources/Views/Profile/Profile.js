(function () {
    $(function () {
        var setTwoFactorLoginModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Profile/SetTwoFactorLogin',
            scriptUrl: abp.appPath + 'view-resources/Views/Profile/_SetTwoFactorLoginModal.js',
            modalClass: 'SetTwoFactorLoginModal'
        });
        $('#btnTwoFactorLogin').click(function (e) {
            e.preventDefault();
            console.log("btnTwoFactorLogin");
            setTwoFactorLoginModal.open();
        });

        var changePasswordModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Profile/ChangePasswordModal',
            scriptUrl: abp.appPath + 'view-resources/Views/Profile/_ChangePasswordModal.js',
            modalClass: 'ChangePasswordModal'
        });
        $('#btnChangePassword').click(function (e) {
            e.preventDefault();
            console.log("btnChangePassword");
            changePasswordModal.open();
        });
    });
})();