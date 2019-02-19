function filterGlobal() {
    $('#dataTables-example').DataTable().search(
        $('#global_filter').val(),
        $('#global_regex').prop('checked'),
        $('#global_smart').prop('checked')
    ).draw();
}

function filterColumn(i) {
    $('#dataTables-example').DataTable().column(i).search(
        $('#col' + i + '_filter').val(),
        $('#col' + i + '_regex').prop('checked'),
        $('#col' + i + '_smart').prop('checked')
    ).draw();
}

$(document).ready(function () {
    $('#dataTables-example').DataTable({
        "aaSorting": [[0, "asc"]],
        "sPaginationType": "full_numbers",
        "paging": false,
        "searching": false,
        //"bProcessing": true,
        "bDeferRender": true,

        "order": [[0, "asc"]],
        "responsive": true,
        "ordering": true,
        "iDisplayLength": 10,
        "aLengthMenu": [[1, 10, 25, 50, 100, 500, 1000, -1], [1, 10, 25, 50, 100, 500, 1000, "All"]],
        "columnDefs": [
            { "visible": false, "targets": 0 }
        ]
    });

    $('input.global_filter').on('keyup click',
        function () {
            filterGlobal();
        });

    $('input.column_filter').on('keyup click',
        function () {
            filterColumn($(this).parents('tr').attr('data-column'));
        });

    function effect(idLoan) {

        if (confirm('Bạn có thật sự muốn kết thúc dây nợ không ? ')) {
            var idcus = document.getElementById("IDCUS_" + idLoan).value;
            $.ajax({
                type: 'get',
                dataType: 'json',
                url: '/Home/Reset',
                data: { id: idcus },
                success: function (data) {
                    if (data.success) {
                        document.getElementById("effect_" + idcus).style.color = "green";
                        document.getElementById("item_" + idLoan).setAttribute("onclick", "");
                        location.reload();
                    } else
                        alert('có lỗi xảy ra');
                },
                error: function (textStatus, errorThrown) {
                    alert('Error - ' + errorThrown);
                }
            });
        }
    }

    function deleteCus(cusid) {
        $.ajax({
            cache: false,
            dataType: "json",
            type: "POST",
            url: '@Url.Action("DeleteCustomer", "Home")',
            data: { id: cusid },
            error: function (xhr, status, error) {
                alert(error);
            },
            success: function (result, status, xhr) {
                if (result != null) {
                    if (result.status == "success") {
                        window.location.reload(true);
                    } else if (result.status == "error") {
                        window.alert(result.message);
                        window.location.reload(true);
                    }
                }
            }
        });
    }

    $("#Name").autocomplete({
        source: "/Default/SearchName",
        select: function (event, ui) {

        }
    });

    $("#Code").autocomplete({
        source: "/Default/SearchCode",
        select: function (event, ui) {

        }
    });

    $("#Phone").autocomplete({
        source: "/Default/SearchPhone",
        select: function (event, ui) {
        }
    });

    $("#Address").autocomplete({
        source: "/Default/SearchAddress",
        select: function (event, ui) {
        }
    });

    $(function () {
        $("#datetime").datepicker({ dateFormat: "dd/mm/yy" }).val();
    });

});

$(function () {
    $('#pagesize').on('change',
        function () {
            $('#form1').submit();
        });
});

function SumMoneyByCode() {
    var type = $("input[name=type]").val();
    var finaldate = $("input[name=datetime]").val();

    $.ajax({
        url: '/History/History',
        type: 'POST',
        data: { id: null, type: type, datetime: finaldate },
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        },
        success: function (result) {
            window.location.href = '/Home/LoadCustomer?type=' + result;
        }
    });
}

function ResetDatetime() {
    var type = $("input[name=type]").val();
    var message = document.getElementById("message").innerText;
    var finaldate = $("input[name=datetime]").val();

    $.ajax({
        url: '/Home/ResetDatetime',
        type: 'POST',
        data: { type: type, message: message, datetime: finaldate },
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        },
        success: function (result) {
            window.location.href = '/Home/LoadCustomer?type=' + result;
        }
    });
}

function chuadongtien(idLoan, permission) {
    if (permission == 0) {
        return;
    }

    $("#divLoader").show();
    var idcus = document.getElementById("IDCUS_" + idLoan).value;
    var songaydong = document.getElementById("songaydong_" + idcus).value;
    var ct = document.getElementById('ct_' + idcus).value;
    if (ct == "") {
        ct = 0;
    } else {
        ct = parseInt(ct);
    }
    $.ajax({
        type: 'get',
        dataType: 'json',
        cache: false,
        url: '/Home/UpdateLoan',
        data: { loanid: idLoan, songaydong: songaydong, idcus: idcus },
        success: function (data) {
            
            if (data.success) {
                $("#divLoader").hide();
                $('#amount_' + idcus).val(data.amount);
                $('#remain_' + idcus).val(data.remainingamount);

                if (data.status == 1) {
                    ct = ct + data.ct;
                }
                $('#ct_' + idcus).val(ct);
                if (data.songay > 0) {
                    for (var i = 0; i < data.songay; i++) {
                        var a = idLoan + i;
                        document.getElementById("item_" + a).className = "btn btn-success";

                        document.getElementById("item_" + a)
                            .setAttribute("onclick", "return chuadongtien1(" + idLoan + "," + permission + ");");

                    }
                } else {
                    document.getElementById("item_" + idLoan).className = "btn btn-success";

                    document.getElementById("item_" + idLoan)
                        .setAttribute("onclick", "return chuadongtien1(" + idLoan + "," + permission + ");");

                }

            } else {
                $("#divLoader").hide();
                alert(data.message);
            }

        },
        error: function (textStatus, errorThrown) {
            alert('Error - ' + errorThrown);
        }
    });
}

function chuadongtien1(idLoan, permission) {
    if (permission != 1) {
        return;
    }

    $("#divLoader").show();
    var idcus = document.getElementById("IDCUS_" + idLoan).value;
    var songaydong = document.getElementById("songaydong_" + idcus).value;
    var ct = document.getElementById('ct_' + idcus).value;
    if (ct == "") {
        ct = 0;
    } else {
        ct = parseInt(ct);
    }

    $.ajax({
        type: 'get',
        dataType: 'json',
        url: '/Home/UpdateLoan',
        data: { loanid: idLoan, songaydong: songaydong, idcus: idcus },
        success: function (data) {
            
            if (data.success) {
                $("#divLoader").hide();
                $('#amount_' + idcus).val(data.amount);
                $('#remain_' + idcus).val(data.remainingamount);

                if (data.status == 0) {
                    ct = ct - data.ct;
                }
                $('#ct_' + idcus).val(ct);

                if (data.songay > 0) {
                    for (var i = 0; i < data.songay; i++) {
                        var a = idLoan + i;
                        document.getElementById("item_" + a).className = "btn btn-danger";
                        document.getElementById("item_" + a)
                            .setAttribute("onclick", "return chuadongtien(" + idLoan + "," + permission + ");");
                    }
                } else {
                    document.getElementById("item_" + idLoan).className = "btn btn-danger";
                    document.getElementById("item_" + idLoan)
                        .setAttribute("onclick", "return chuadongtien(" + idLoan + "," + permission + ");");
                }

            } else {
                $("#divLoader").hide();
                alert(data.message);
            }
        },
        error: function (textStatus, errorThrown) {
            alert('Error - ' + errorThrown);
        }
    });
}
