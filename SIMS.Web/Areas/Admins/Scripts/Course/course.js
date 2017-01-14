
jQuery(document).ready(function ($) {
    InitTable();

    BindDDLDepartment('ddlDepartmentForCouse');
    BindDDLDepartment('ddlDepartmentForCourse2');

    $("#btnQuery").bind("click", function () {
        var param = {
            //name: $("#sCourseName").val()
            departmentId: $.trim($("#ddlDepartmentForCouse").val())
        };
        var tbTable = $("#_CourseTable").DataTable();
        tbTable.settings()[0].ajax.data = param;
        tbTable.ajax.reload();
    });
});

var table;

//加载表数据
function InitTable() {
    table = $("#_CourseTable").dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/Admins/Course/GetDataTable",
            "type": "POST",
            "data": {
                departmentId: $.trim($("#ddlDepartmentForCouse").val())
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
            { "data": "Score", "title": "学分" },
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

function ShowModal(flag, id, name, departmentId, score, description) {
    if (flag == 0) {
        //add
        $('#CourseId').val('');
        $('#CourseName').val('');
        $('#CourseScore').val('');
        $('#CourseDescription').val('');
        $('#coursemodal .modal-title').html('添加');
        $('#coursemodal').modal('show');
    } else {
        //update
        $('#CourseId').val(id);
        $('#ddlDepartmentForCourse2').val(departmentId);
        $('#CourseName').val(name);
        $('#CourseScore').val(score);
        $('#CourseDescription').val(description);
        $('#coursemodal .modal-title').html('修改');
        $('#coursemodal').modal('show');
    }
}

function AddOrUpdate() {

    var id = $('#CourseId').val();
    if (id == 0) {
        Add();
    } else {
        Update();
    }
}

//添加
function Add() {
    var departmentId = $.trim($("#ddlDepartmentForCourse2").val());
    var name = $.trim($("#CourseName").val());
    var score = $.trim($("#CourseScore").val())
    var description = $.trim($("#CourseDescription").val())

    if (departmentId == '' || departmentId == '0') {
        alert('院系不能为空!');
        return;
    }

    if (name == '') {
        alert('课程名称不能为空!');
        return;
    }

    if (score == '' || score == '0')
    {
        alert('课程学分不能为空!');
        return;
    }

    $.ajax({
        url: '/Admins/Course/Add',
        data: {
            departmentId: departmentId,
            name: name,
            score:score,
            description: description
        },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (result) {
            if (result.Bresult) {
                $('#coursemodal').modal('hide');
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
    var departmentId = $.trim($("#ddlDepartmentForCourse2").val());
    var name = $.trim($("#CourseName").val());
    var score = $.trim($("#CourseScore").val())
    var description = $.trim($("#CourseDescription").val())

    if (departmentId == '' || departmentId == '0') {
        alert('院系的编号和名称不能为空!');
        return;
    }

    if (name == '') {
        alert('课程名称不能为空!');
        return;
    }

    if (score == '' || score == '0') {
        alert('课程学分不能为空!');
        return;
    }

    $.ajax({
        url: '/Admins/Course/Update',
        data: {
            id: $.trim($("#CourseId").val()),
            departmentId: departmentId,
            name: name,
            score:score,
            description: description
        },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (result) {
            if (result.Bresult) {
                $('#coursemodal').modal('hide');
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