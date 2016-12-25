jQuery(document).ready(function ($) {
    InitTable();
    BindDDLDepartment('ddlDepartment');
    BindDDLDepartment('ddlDepartment1');

    $("#btnQuery").bind("click", function () {
        var param = {
            departmentId: $.trim($("#ddlDepartment").val())
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
                departmentId: $.trim($("#ddlDepartment").val())
            },
            error: function (e) {
                alert("加载table异常:" + e.responseText);
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
            //{ "data": "Id", "title": "ID" },
            { "data": "Code", "title": "编号" },
            { "data": "DepartmentName", "title": "学院名称" },
            { "data": "Name", "title": "名称" },
            { "data": "Description", "title": "描述" },
            { "data": "Actions", "title": "操作", "sWidth": "15%", "sClass": "text-center" }
        ]
    });
}

function BindDDLDepartment(id) {
    $.ajax({
        url: '/Admins/Department/GetDepartment',
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
                $("#" + id).html(htmlstr);
            }
        },
        error: function (e) {
            alert("绑定BindDDLDepartment异常:" + e.responseText);
        }
    });
}

function ShowModal(flag) {
    if (flag == 0) {
        //add
        $('#majormodal .modal-title').html('添加');
        $('#majormodal').modal('show');
    } else {
        //update
        $('#majormodal .modal-title').html('修改');
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
    var departmentId = $.trim($("#ddlDepartment1").val());
    var name = $.trim($("#MajorName").val());
    var description = $.trim($("#MajorDescription").val())

    if (departmentId == '' || departmentId == '0')
    {
        alert('院系的编号和名称不能为空!');
        return;
    }

    if (name == '')
    {
        alert('专业名称不能为空!');
        return;
    }

    $.ajax({
        url: '/Admins/Major/Add',
        data: {
            departmemtId: departmentId,
            name: name,
            description: description
        },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (result) {
            if (result.Bresult) {
                $('#majormodal').modal('hide');
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

    var code = $.trim($("#Code").val());
    var name = $.trim($("#Name").val());
    var description = $.trim($("#Description").val())

    if (code == '' || name == '') {
        return '院系的编号和名称不能为空!';
    }

    $.ajax({
        url: '/Admins/Major/Update',
        data: {
            id: $.trim($("#Id").val()),
            code: code,
            name: name,
            description: description
        },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (result) {
            if (result.Bresult) {
                $('#majormodal').modal('hide');
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