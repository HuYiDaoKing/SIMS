
jQuery(document).ready(function ($) {
   
    var _table = '';
    $.ajax({
        url: '/Admins/SchoolTimeTable/GetSchoolTimeTable',
        data: null,
        type: 'post',
        cache: false,
        //dataType: 'json',
        async: false,
        success: function (result) {
            //$('#_SchoolTimeTable').empty();
            //$('#_SchoolTimeTable').append(result);
            //$('#_SchoolTimeTable').html(result);
            _table = result;
        },
        error: function (e) {
            alert("异常:" + e.responseText);
        }
    });

    $('#_SchoolTimeTable').html(_table);
})
