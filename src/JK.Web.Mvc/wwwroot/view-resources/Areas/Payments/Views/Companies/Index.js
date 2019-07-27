(function () {
    $(function () {
        //对应AppService API.
        var _companyService = abp.services.app.company;

        //新建或修改模态窗口
        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Payments/Companies/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Payments/Views/Companies/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditCompanyModal'
        });

        //新建按钮事件
        $('#CreateNewButton').click(function () {
            _createOrEditModal.open();
        });

        $('#QueryButton').click(function () {
            dataTableReload();
        });
        //保存成功事件，重新刷新列表
        abp.event.on('app.createOrEditCompanyModalSaved', function () {
            dataTableReload();
        });

        //删除
        function deleteRecord(record) {
            abp.message.confirm(
                record.name + '将会被删除。',
                '您确定吗？',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _companyService.delete({
                            id: record.id
                        }).done(function () {
                            dataTableReload();
                            abp.notify.success('删除成功。');
                        });
                    }
                }
            );
        }

        function EnablePlatform(id, isAction) {
            _companyService.enable({
                id: id,
                isAction: isAction
            }).done(function () {
                dataTableReload();
            });
        }

        var _$dataTable = $('#DataTable');
        //列表
        var dataTable = _$dataTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                //查询数据的方法
                ajaxFunction: _companyService.getAll,
                inputFilter: function () {
                    var prms = {};
                    $("#companiesFilter").serializeArray().map(function (x) { prms[abp.utils.toCamelCase(x.name)] = x.value; });
                    return prms;
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "name"
                },
                {
                    targets: 1,
                    data: "isActive",
                    orderable: false,
                    render: function (isActive) {
                        if (isActive)
                            return '<i class="material-icons" style="color:green;">check_box</i>';
                        else
                            return '<i class="material-icons" style="color:red;">indeterminate_check_box</i>';
                    }
                },
                {
                    targets: 2,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        text: '<i class="fa fa-cog"></i>操作<span class="caret"></span>',
                        items: [
                            {
                                text: '修改',
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.id });
                                }
                            }, {
                                text: '删除',
                                action: function (data) {
                                    deleteRecord(data.record);
                                }
                            }, {
                                text: '启用',
                                visible: function (data) {
                                    if (data.record.isActive == false) { return true; }
                                    else
                                        return false;
                                },
                                action: function (data) {
                                    EnablePlatform(data.record.id, true);
                                }
                            }, {
                                text: '禁用',
                                visible: function (data) {
                                    if (data.record.isActive == true) { return true; }
                                    else
                                        return false;
                                },
                                action: function (data) {
                                    EnablePlatform(data.record.id, false);
                                }
                            }
                        ]
                    }
                }
            ]
        });

        function dataTableReload() {
            dataTable.ajax.reload();
        }

    });
})();
