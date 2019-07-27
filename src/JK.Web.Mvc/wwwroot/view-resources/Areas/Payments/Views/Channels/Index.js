(function () {
    $(function () {
        //对应AppService API.
        var _channelService = abp.services.app.channel;

        //新建或修改模态窗口
        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Payments/Channels/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Payments/Views/Channels/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditChannelModal'
        });
        //新建按钮事件
        $('#CreateNewButton').click(function () {
            _createOrEditModal.open();
        });

        $('#QueryButton').click(function () {
            dataTableReload();
        });


        //保存成功事件，重新刷新列表
        abp.event.on('app.createOrEditChannelModalSaved', function () {
            dataTableReload();
        });

        //删除
        function deleteRecord(record) {
            abp.message.confirm(
                record.name + '将会被删除。',
                '您确定吗？',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _channelService.delete({
                            id: record.id
                        }).done(function () {
                            dataTableReload();
                            abp.notify.success('删除成功。');
                        });
                    }
                }
            );
        }


        function ChangeChannelStatus(tenantId, isAction) {
            _channelService.enable({
                id: tenantId,
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
                ajaxFunction: _channelService.getAll,
                inputFilter: function () {
                    var prms = {};
                    $("#channelsFilter").serializeArray().map(function (x) { prms[abp.utils.toCamelCase(x.name)] = x.value; });
                    console.log(prms);
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
                    data: "displayName"
                },
                {
                    targets: 2,
                    data: "channelCode"
                },
                {
                    targets: 3,
                    data: "isActive",
                    render: function (isActive) {
                        if (isActive)
                            return '<i class="material-icons" style="color:green;">check_box</i>';
                        else
                            return '<i class="material-icons" style="color:red;">indeterminate_check_box</i>';
                    }
                },
                {
                    targets: 4,
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
                                    return !data.record.isActive;
                                },
                                action: function (data) {
                                    ChangeChannelStatus(data.record.id, true);
                                }
                            }, {
                                text: '禁用',
                                visible: function (data) {
                                    return data.record.isActive;
                                },
                                action: function (data) {
                                    ChangeChannelStatus(data.record.id, false);
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
