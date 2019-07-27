(function () {

    app.modals.CreateOrEditCompanyModal = function () {

        var _modalManager;
        var companyService = abp.services.app.company;
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

            var company = $informationForm.serializeFormToObject();
            company.channelIds = [];
            var _$channelCheckboxes = $("input[name='channel']:checked");
            if (_$channelCheckboxes) {
                for (var channelIndex = 0; channelIndex < _$channelCheckboxes.length; channelIndex++) {
                    var _$channelCheckbox = $(_$channelCheckboxes[channelIndex]);
                    company.channelIds.push(_$channelCheckbox.val());
                }
            }
            _modalManager.setBusy(true);
            if (company.Id > 0) {
                companyService.update(company).done(function () {
                    abp.notify.info('保存成功。');
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditCompanyModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                companyService.create(company).done(function () {
                    abp.notify.info('保存成功。');
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditCompanyModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }
        };
    };
})();