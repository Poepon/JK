(function () {

    app.modals.CreateOrEditChannelModal = function () {

        var _modalManager;
        var channelService = abp.services.app.channel;
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

            var paymentMethod = $informationForm.serializeFormToObject();

            _modalManager.setBusy(true);
            if (channel.Id > 0) {
                channelService.update(channel).done(function () {
                    abp.notify.info('保存成功。');
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditChannelModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                channelService.create(channel).done(function () {
                    abp.notify.info('保存成功。');
                    _modalManager.close();
                    abp.event.trigger('app.createOrEditChannelModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }
        };
    };
})();