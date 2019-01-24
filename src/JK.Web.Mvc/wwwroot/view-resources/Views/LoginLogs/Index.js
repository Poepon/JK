(function () {
    $(function () {
        //对应AppService API.
        var _userLoginService = abp.services.app.userLogin;

        //查询按钮事件
        $('#QueryButton').click(function () {
            dataTableReload();
        });

        $("#datetimeRange").daterangepicker(
            app.createDateRangePickerOptions(),
            function (start, end, label) {
                if (start.isValid() && end.isValid()) {
                    $("#input[name='StartTime']").val(start.format('YYYY-MM-DDT00:00:00Z'));
                    $("#input[name='EndTime']").val(end.format('YYYY-MM-DDT23:59:59.999Z'));
                } else {
                    $("#input[name='StartTime']").val('');
                    $("#input[name='EndTime']").val('');
                }
            });

        var _$dataTable = $('#DataTable');
        //列表
        var dataTable = _$dataTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                //查询数据的方法
                ajaxFunction: _userLoginService.getUserLoginAttempts,
                inputFilter: function () {
                    var prms = {};
                    $("#LoginLogsFilter").serializeArray().map(function (x) { prms[abp.utils.toCamelCase(x.name)] = x.value; });
                    return prms;
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "userNameOrEmailAddress"
                },
                {
                    targets: 1,
                    data: "clientIpAddress"
                },
                {
                    targets: 2,
                    data: "browserInfo"
                },
                {
                    targets: 3,
                    data: "result"
                }, 
                {
                    targets: 4,
                    data: "creationTime",
                    render: function (creationTime) {
                        return moment(creationTime).format('YYYY-MM-DD HH:mm');
                    }
                }
            ]
        });

        function dataTableReload() {
            dataTable.ajax.reload();
        }

    });
})();
