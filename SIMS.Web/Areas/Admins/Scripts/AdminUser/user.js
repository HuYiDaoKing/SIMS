
jQuery(document).ready(function ($) {

    $('#Grade').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-MM',
        showButtonPanel: true,
        onClose: function(dateText, inst) {
            var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
            var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
            $(this).datepicker('setDate', year+'-'+month);
        }
    });      

    InitTable();
    $("#btnQuery").bind("click", function () {
        var param = {
            code: $("#Code").val()
        };
        var tbTable = $("#_AdminUserTable").DataTable();
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
            "url": "/Admins/AdminUser/GetDataTable",
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
            { "data": "Id", "title": "ID" },
            { "data": "Code", "title": "编号" },
            { "data": "Name", "title": "姓名" },
            { "data": "Sex", "title": "性别" },
            { "data": "DepartmentName", "title": "系" },
            { "data": "MajorName", "title": "专业" },
            { "data": "Profession", "title": "职业" },
            /*{
                "data": "Id",
                "title": "操作",
                "render": function (data, type, full) {
                    console.log(full);
                    var actions = '';
                    var role = $("#hfrole").val();
                    if (role == 1) {
                        actions += "<a class=\"btn btn-secondary btn-sm btn-icon icon-left\" href=\"javascript:;\" onclick=\"Modify('" + full.Id + "','" + full.ActivityName + "','" + full.Discount + "','" + full.BeginTime + "','" + full.EndTime + "','" + full.Remark + "');\">编辑</a>";
                        actions += "<a class=\"btn btn-danger btn-sm btn-icon icon-left\" href=\"javascript:;\"  onclick=\"Execute('" + data + "')\">删除</a>";
                    }
                    return actions;
                }
            }*/
            { "data": "Actions", "title": "操作", "sWidth": "15%", "sClass": "text-center" }
        ]
    });
}

function ShowModal(flag, name, grade, sex, nation, department, major, profession) {
    if (flag == 0) {
        //add
        $('#usermodal .modal-title').html('添加');
        $('#Name').val('');
        //$('#dp_Grade').val('');
        //$('#dp_sex').val('');
        $('#Nation').val('');
        //$('#dp_department').val('');
        //$('#dp_major').val('');
        //$('#dp_profession').val('');

        $('#usermodal').modal('show');
    } else {
        //update
        $('#usermodal .modal-title').html('修改');
        $('#Name').val(name);
        $('#dp_Grade').val(grade);
        $('#dp_sex').val(sex);
        $('#Nation').val(nation);
        $('#dp_department').val(department);
        $('#dp_major').val(major);
        $('#dp_profession').val(profession);

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
    var name= $('#Name').val();
    var grade=$('#dp_Grade').val();
    var sex=$('#dp_sex').val();
    var nation=$('#Nation').val();
    var department=$('#dp_department').val();
    var major=$('#dp_major').val();
    var profession = $('#dp_profession').val();

    if (name == '')
        return '姓名不能为空 !';
    if (grade == '')
        return '年级不能为空!';
    if (sex == '')
        return '性别不能为空 !';
    if (nation == '')
        return '民族不能为空 !';
    if (department == '')
        return '民族不能为空 !';
    if (major == '')
        return '专业不能为空 !';
    if (profession == '')
        return '职业类型不能为空 !';

    $.ajax({
        url: '/Admins/AdminUser/Add',
        data: {
            name: name,
            grade: grade,
            sex: sex,
            nation: nation,
            department: department,
            major: major,
            profession: profession
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