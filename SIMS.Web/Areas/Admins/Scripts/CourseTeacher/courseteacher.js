
jQuery(document).ready(function ($) {
    InitTable();

    BindDDLCourse('ddlCourseForCourseTeacher');
    BindDDLCourse('ddlCourseForCourseTeacher2');

    BindDDLTeacher('ddlTeacherForCourseTeacher');
    BindDDLTeacher('ddlTeacherForCourseTeacher2');

    $("#btnQuery").bind("click", function () {
        var param = {
            courseId: $.trim($("#ddlCourseForCourseTeacher").val()),
            teacherId: $.trim($("#ddlTeacherForCourseTeacher").val())
        };
        var tbTable = $("#_CourseTeacherTable").DataTable();
        tbTable.settings()[0].ajax.data = param;
        tbTable.ajax.reload();
    });
});

var table;

//加载表数据
function InitTable() {
    table = $("#_CourseTeacherTable").dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/Admins/CourseTeacher/GetDataTable",
            "type": "POST",
            "data": {
                courseId: $.trim($("#ddlCourseForCourseTeacher").val()),
                teacherId: $.trim($("#ddlTeacherForCourseTeacher").val())
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
            //{ "data": "Id", "title": "ID" },
            { "data": "Course", "title": "课程" },
            { "data": "Teacher", "title": "老师" },
            { "data": "Actions", "title": "操作", "sWidth": "15%", "sClass": "text-center" }
        ]
    });
}

function BindDDLCourse(id) {
    $.ajax({
        url: '/Admins/Course/GetCourses',
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
            alert("绑定BindDDLCourse异常:" + e.responseText);
        }
    });
}

//绑定教师
function BindDDLTeacher(id) {
    $.ajax({
        url: '/Admins/AdminUser/GetTeachers',
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
            alert("绑定BindDDLTeacher异常:" + e.responseText);
        }
    });
}

function ShowModal(flag, id, courseid, teacherid) {
    if (flag == 0) {
        //add
        $('#CourseTeacherId').val('');
        $('#courseteachermodal .modal-title').html('添加');
        $('#courseteachermodal').modal('show');
    } else {
        //update
        $('#CourseTeacherId').val(id);
        $('#ddlTeacherForCourseTeacher2').val(courseid);
        $('#ddlCourseForCourseTeacher2').val(teacherid);
        $('#courseteachermodal .modal-title').html('修改');
        $('#courseteachermodal').modal('show');
    }
}

function AddOrUpdate() {

    var id = $('#CourseTeacherId').val();
    if (id == 0) {
        Add();
    } else {
        Update();
    }
}

//添加
function Add() {
    var courseId = $.trim($("#ddlCourseForCourseTeacher2").val());
    var teacherId = $.trim($("#ddlTeacherForCourseTeacher2").val());

    if (courseId == '' || courseId == '0') {
        alert('课程不能为空!');
        return;
    }

    if (teacherId == '' || teacherId == '0') {
        alert('教师不能为空!');
        return;
    }

    $.ajax({
        url: '/Admins/CourseTeacher/Add',
        data: {
            courseId: courseId,
            teacherId: teacherId
        },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (result) {
            if (result.Bresult) {
                $('#courseteachermodal').modal('hide');
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
    var courseId = $.trim($("#ddlCourseForCourseTeacher2").val());
    var teacherId = $.trim($("#ddlTeacherForCourseTeacher2").val());

    if (courseId == '' || courseId == '0') {
        alert('课程不能为空!');
        return;
    }

    if (teacherId == '' || teacherId == '0') {
        alert('教师不能为空!');
        return;
    }

    $.ajax({
        url: '/Admins/Course/Update',
        data: {
            id: $.trim($("#CourseId").val()),
            courseId: courseId,
            teacherId: teacherId
        },
        type: 'post',
        cache: false,
        dataType: 'json',
        success: function (result) {
            if (result.Bresult) {
                $('#courseteachermodal').modal('hide');
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