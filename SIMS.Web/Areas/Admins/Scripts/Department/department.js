
jQuery(document).ready(function ($) {
    InitTable();
    $("#btnQuery").bind("click", function () {
        var param = {
            name: $("#sDepartmentName").val()
        };
        var tbTable = $("#_DepartmentTable").DataTable();
        tbTable.settings()[0].ajax.data = param;
        tbTable.ajax.reload();
    });
});

var table;
//加载表数据
function InitTable() {
    table = $("#_DepartmentTable").dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/Admins/Department/GetDataTable",
            "type": "POST",
            "data": {
                name: $("#sDepartmentName").val()
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
            { "data": "Name", "title": "名称" },
            { "data": "Description", "title": "描述" },
            { "data": "Actions", "title": "操作", "sWidth": "15%", "sClass": "text-center" }
        ]
    });
}

function ShowModal(flag, id, code, name,description) {
    if (flag == 0) {
        //add
        $('#Id').val('');
        //$('#Code').val('');
        $('#DepartmentName').val('');
        $('#DepartmentDescription').val('');
        $('#departmentmodal .modal-title').html('添加');
        $('#departmentmodal').modal('show');
    } else {
        //update
        $('#Id').val(id);
        //$('#Code').val(code);
        $('#DepartmentName').val(name);
        $('#DepartmentDescription').val(description);
        $('#departmentmodal .modal-title').html('修改');
        $('#departmentmodal').modal('show');
    }
}

function AddOrUpdate() {
    var id = $('#Id').val();
    if (id == 0) {
        Add();
    } else {
        Update();
    }
}

//添加
function Add() {

    //var code = $.trim($("#Code").val());
    var name = $.trim($("#DepartmentName").val());
    var description = $.trim($("#DepartmentDescription").val())

    if (name == '') {
        return '院系名称不能为空!';
    }

    $.ajax({
        url: '/Admins/Department/Add',
        data: {
            //code: code,
            name: name,
            description: description
        },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (result) {
            if (result.Bresult) {
                $('#departmentmodal').modal('hide');
                table.fnFilter();
            }
            else {
                alert(result.Notice);
            }
        },
        error: function (e) {
            alert("异常:" + e.responseText);
        }
    });

}

//修改
function Update() {

    //var code = $.trim($("#Code").val());
    var name = $.trim($("#DepartmentName").val());
    var description = $.trim($("#DepartmentDescription").val())

    if (name == '') {
        return '院系的编号和名称不能为空!';
    }

    $.ajax({
        url: '/Admins/Department/Update',
        data: {
            id: $.trim($("#Id").val()),
            //code: code,
            name: name,
            description: description
        },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (result) {
            if (result.Bresult) {
                $('#departmentmodal').modal('hide');
                table.fnFilter();
            }
            else {
                alert(result.Notice);
            }
        },
        error: function (e) {
            alert("异常:" + e.responseText);
        }
    });

}