(function () {
    $(function () {
        //对应AppService API.
        var _bankService = abp.services.app.bank;

        //新建或修改模态窗口
        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Payments/Banks/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Payments/Views/Banks/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditBankModal'
        });
        //新建按钮事件
        $('#CreateNewButton').click(function () {
            _createOrEditModal.open();
        });
        //$('#btnQuery').click(function () {
        //    dataTableReload();
        //});
        //保存成功事件，重新刷新列表
        abp.event.on('app.createOrEditBankModalSaved', function () {
            dataTableReload();
        });

        //删除
        function deleteRecord(record) {
            abp.message.confirm(
                record.name + '将会被删除。',
                '您确定吗？',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _bankService.delete({
                            id: record.id
                        }).done(function () {
                            dataTableReload();
                            abp.notify.success('删除成功。');
                        });
                    }
                }
            );
        }

        var _$dataTable = $('#DataTable');
        //列表
        var dataTable = _$dataTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                //查询数据的方法
                ajaxFunction: _bankService.getAll
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "name"
                },
                {
                    targets: 1,
                    data: "bankCode"
                },
                {
                    targets: 2,
                    data: "orderNumber"
                },
                {
                    targets: 3,
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
