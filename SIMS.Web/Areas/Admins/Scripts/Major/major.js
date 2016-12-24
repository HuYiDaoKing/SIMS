jQuery(document).ready(function ($) {
    InitTable();

    $("#btnQuery").bind("click", function () {
        var param = {
            departmentId: 0
        };
        var tbTable = $("#_MajorTable").DataTable();
        tbTable.settings()[0].ajax.data = param;
        tbTable.ajax.reload();
    });
});

var table;
//加载表数据
function InitTable() {
    table = $("#_MajorTable").dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/Admins/Major/GetDataTable",
            "type": "POST",
            "data": {
                //name: $("#Name").val()
                departmentId:0
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
            { "data": "Id", "title": "ID" },
            { "data": "Code", "title": "编号" },
            { "data": "DepartmentName", "title": "学院名称" },
            { "data": "Name", "title": "名称" },
            { "data": "Description", "title": "描述" },
            { "data": "Actions", "title": "操作", "sWidth": "15%", "sClass": "text-center" }
        ]
    });
}

function BindDDLMajor() {
    $.ajax({
        url: '/Admins/ServiceArea/GetAllMajor',
        data: null,
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                var htmlstr = "<option value='0'>--请选择--</option>";
                for (var i = 0; i < data.length; i++) {
                    htmlstr += "<option value='" + data[i].Id + "'>" + data[i].Name + "</option>";
                }
                $("#" + Id).html(htmlstr);
            }
        },
        error: function (e) {
            alert("异常:" + e.responseText);
        }
    });
}
