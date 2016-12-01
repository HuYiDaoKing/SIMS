
jQuery(document).ready(function ($) {
    InitTable();
    $("#btnQuery").bind("click", function () {
        var param = {
            ActivityName: $("#ActivityName").val(),
            BeginTime: $("#BeginTime").val(),
            EndTime: $("#EndTime").val()
        };
        var tbTable = $("#ActivityTable").DataTable();
        tbTable.settings()[0].ajax.data = param;
        tbTable.ajax.reload();
    });

});

var table;
//加载表数据
function InitTable() {
    table = $("#_AdminUserTable").dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "Admins/AdminUser/GetDataTable",
            "type": "POST",
            "data": {
                code: $("#Code").val()
            },
            error: function (e) {
                alert("异常:" + e.responseText);
            }
        },
        "dom": 'rtp',
        "language": {
            'emptyTable': '',
            'loadingRecords': '加载中...',
            'processing': '查询中...',
            'search': '检索:',
            'lengthMenu': '每页 _MENU_ 件',
            'zeroRecords': '没有数据',
            'paginate': {
                'first': '第一页',
                'last': '最后一页',
                'next': '下一页',
                'previous': '上一页'
            },
            'info': '第 _PAGE_ 页 / 总 _PAGES_ 页',
            'infoEmpty': '没有数据',
            'infoFiltered': '(过滤总件数 _MAX_ 条)'
        },
        "columns": [
            { "data": "ActivityName", "title": "活动名称" },//任务ID
            { "data": "Discount", "title": "折扣率" },
            { "data": "BeginTime", "title": "开始时间" },
            { "data": "EndTime", "title": "结束时间" },
            { "data": "UserNickName", "title": "创建人" },
            { "data": "CreateDate", "title": "创建时间" },
            { "data": "Remark", "title": "备注" },
            {
                "data": "ActivityId",
                "title": "操作",
                "render": function (data, type, full) {
                    console.log(full);
                    var actions = '';
                    var role = $("#hfrole").val();
                    if (role == 1) {
                        actions += "<a class=\"btn btn-secondary btn-sm btn-icon icon-left\" href=\"javascript:;\" onclick=\"Modify('" + full.ActivityId + "','" + full.ActivityName + "','" + full.Discount + "','" + full.BeginTime + "','" + full.EndTime + "','" + full.Remark + "');\">编辑</a>";
                        actions += "<a class=\"btn btn-danger btn-sm btn-icon icon-left\" href=\"javascript:;\"  onclick=\"Execute('" + data + "')\">删除</a>";
                    }
                    return actions;
                }
            }
        ]
    });
}