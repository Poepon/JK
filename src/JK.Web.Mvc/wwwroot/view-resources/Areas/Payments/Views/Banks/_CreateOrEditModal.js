(function () {

    app.modals.CreateOrEditBankModal = function () {

        var _modalManager;
        var bankService = abp.services.app.bank;
        var $informationForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            var $modal = _modalManager.getModal();

            $('select').selectpicker();
            $informationForm = $modal.find('form[name=InformationsForm]');
            $informationForm.validate();
        };

        this.save = function () {
            if (!$informationForm.valid()) {
                return;
            }

            var banks = $informationForm.serializeFormToObject();
            _modalManager.setBusy(true);
            if (banks.Id > 0) {
                bankService.update(banks).done(function () {
                    abp.notify.info('保存成功。');
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditBankModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                bankService.create(banks).done(function () {
                    abp.notify.info('保存成功。');
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditBankModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }
        };
    };
})();