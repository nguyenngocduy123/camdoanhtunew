$(document).ready(function () {
    $("#abcd").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Default/SearchCode",
                type: "POST",
                data: "{ 'term': '" + request.term + "'}",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    console.log(response.responseText);
                },
                failure: function (response) {
                    console.log(response.responseText);
                }
            })
        },
        select: function (event, ui) {
            
            $.ajax({
                cache: false, async: false, type: "POST",
                url: "/Home/GetCusDetail",
                data: { "code": ui.item.id },
                success: function (data) {
                    var item = data[0];
                    $("#model_Name").val(item.Name);
                    $("#model_Phone").val(item.Phone);
                    $("#model_Address").val(item.Address);
                    $("#model_Loan").val(item.Loan);
                    $("#model_Price").val(item.Price);
                    $("#model_tiengoc").val(item.tiengoc);
                    $("#SelectedLoaiGiayTo").val(item.loaigiayto);
                    $("#model_Note").val(item.Note);
                    $("#model_DayBonus").val(item.songayduocphepnothem);
                    $("#model_OldCode").val(item.OldCode);

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve Item.');
                }
            });
        }
    });
})

jQuery(function ($) {
    $('.auto').autoNumeric('init');
});

$.validator.addMethod("imageOnly", function (value, element) {
    return this.optional(element) || /^.+\.(jpg|JPG|png|PNG)$/.test(value);
});

$.validator.addMethod("codeCheck", function (value, element) {
    var type = document.getElementById("model_type").value;

    if (value == null) {
        return true;
    }
    else {
        var s = value[0];

        if (type == 1 && s != 'B') {
            return false;
        }
        else if (type == 2 && s != 'C') {
            return false;
        }
        else if (type == 3 && s != 'M') {
            return false;
        }
        else if (type == 4 && s != 'Z') {
            return false;
        }

        return true;
    }

});

$("#frmaAddCustomer").validate({
    rules: {
        //'model.Code': {
        //    required: true,
        //    codeCheck: true
        //},
        'model.Name': {
            required: true
        },
        'model.fuMain': {
            imageOnly: true
        },
        'model.Phone': {
            required: true,
            number: true,
            minlength: 10,
            maxlength: 11
        },
        'model.Address': {
            required: true
        },
        'model.Price': {
            required: true,
            number: true
        },
        'model.Loan': {
            required: true,
            number: true
        }
    },
    messages: {
        //'model.Code': {
        //    required: 'Chưa nhập mã số',
        //    codeCheck: 'Mã số này không hợp lệ'
        //},
        'model.Name': {
            required: 'Chưa nhập tên khách hàng'
        },
        'model.fuMain': {
            imageOnly: 'Không đúng định dạng hình ảnh'
        },
        'model.Phone': {
            required: 'Chưa nhập số điện thoại',
            number: 'Nhập sđt không đúng định dạng',
            minlength: 'Ít nhất 10 số',
            maxlength: 'Nhiều nhất 11 số'
        },
        'model.Address': {
            required: 'Chưa nhập địa chỉ'
        },
        'model.Price': {
            required: 'Chưa nhập số tiền phải trả trong 1 ngày',
            number: 'Phải là số'
        },
        'model.Loan': {
            required: 'Chưa nhập tổng nợ',
            number: 'Phải là số'
        }

    },
    errorElement: 'span',
    errorClass: 'help-block',
    highlight: function (element) {
        $(element).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        //label.closest('.form-group').removeClass('has-success');

        //var name = label.attr('for');
        //$('[name=' + name + ']').closest('.form-group').removeClass('has-error');
        label.remove();
    }
});

$("#fuMain").fileinput({
    'allowedFileExtensions': ['jpg', 'png', 'gif']
});

$(function () {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //Month starts from 0
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd;
    }

    if (mm < 10) {
        mm = '0' + mm;
    }
    today = dd + '/' + mm + '/' + yyyy;

    $("#model_StartDate").val(today);
    $("#model_StartDate").datepicker({ dateFormat: "dd/mm/yy" }).val();
});

$('form').submit(function () {
    var form = $(this);
    $('input').each(function (i) {
        var self = $(this);
        try {
            var v = self.autoNumeric('get');
            self.autoNumeric('destroy');
            self.val(v);
        } catch (err) {
            console.log("Not an autonumeric field: " + self.attr("name"));
        }
    });
    return true;
});