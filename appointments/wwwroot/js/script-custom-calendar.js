var routeURL = location.protocol + "//" + location.host;

document.addEventListener('DOMContentLoaded', function () {

    var calendar;
    try {


        var calendarEl = document.getElementById('calendar');
        if (calendarEl != null) {
            calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                firstDay: 1,
                locale: 'pl',
                buttonText: {
                    month: 'Miesiąc',
                    today: 'Dzisiaj'
                },
                headerToolbar: {
                    left: 'prev,next,today',
                    center: 'title',
                    right: 'dayGridMonth'
                },
                selectable: true,
                editable: false,
                select: function (event) {
                    onShowModal(event, null);
                },

            });
            calendar.render();
        } else
            alert(calendarEl);

    }
    catch (e) {
        alert(e);
    }


});

function onShowModal(obj, isEventDetails) {
    var x = new moment(obj.startStr);
    var y = new moment(obj.endStr);
    var startDate = moment(obj.startStr).format("YYYY-MM-DD");
    var endDate = moment(obj.endStr).subtract(1, 'day').format("YYYY-MM-DD");
    var duration = y.diff(x, 'days');

    $('#dateFrom').val(startDate);
    $('#dateEnd').val(endDate);
    $('#duration').val(duration);
    $("#appointmentInput").modal("show");
}

function onCloseModal() {
    $("#appointmentInput").modal("hide");
}

function onSubmitForm() {
    if (checkValidation()) {

        var requestData = {
            Id: parseInt($("#id")),
            Title: $("#title").val(),
            Description: $("#description").val(),
            StartDate: $("#dateFrom").val(),
            EndDate: $("#dateEnd").val(),
            AppWorkerId: $("#appWorkerId").val(),
            Duration: $("#duration").val(),
        }

        $.ajax({
            url: routeURL + '/api/Vacation/SaveCalendarData',
            type: 'POST',
            data: JSON.stringify(requestData),
            contentType: 'application/json',
            success: function (response) {
                if (response.status === 1 || response.status === 2) {
                    $.notify(response.message, "success");
                    onCloseModal();
                }
                else {
                    $.notify(response.message, "error");
                }
            },
            error: function (xhr) {
                $.notify("Error", "error");
            }
        });
    }
}

function onDataUpdate(value) {
    var x = new moment(document.getElementById('dateFrom').value);
    var y = new moment(document.getElementById('dateEnd').value);
    var duration = y.diff(x, 'days');
    $('#duration').val(duration + 1);
}

function checkValidation() {
    var isValid = true;
    if ($("#title").val() === undefined || $("#title").val() === "") {
        isValid = false;
        $("#title").addClass('error');
    }
    else
        $("#title").removeClass('error');

    if ($("#dateFrom").val() === undefined || $("#dateFrom").val() === "") {
        isValid = false;
        $("#dateFrom").addClass('error');
    }
    else
        $("#dateFrom").removeClass('error');

    if ($("#dateEnd").val() === undefined || $("#dateEnd").val() === "") {
        isValid = false;
        $("#dateEnd").addClass('error');
    }
    else
        $("#dateEnd").removeClass('error');

    return isValid;
}
