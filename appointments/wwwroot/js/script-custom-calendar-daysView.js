var routeURL = location.protocol + "//" + location.host;
var calendar;

document.addEventListener('DOMContentLoaded', function () {
    try {
        var calendarEl = document.getElementById('calendar');
        if (calendarEl != null) {
            calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridWeek',
                firstDay: 1,
                locale: 'pl',

                buttonText: {
                    week: 'Tydzień',
                    today: 'Dzisiaj'
                },
                headerToolbar: {
                    left: 'prev,next,today',
                    center: 'title',
                    right: 'dayGridWeek'
                },
                showNonCurrentDates: false,
                fixedWeekCount: false,
                selectable: true,
                editable: false,
                select: function (event) {
                    onShowModal(event, null);
                },
                eventDisplay: 'block',
                events: function (fetchInfo, successCallback, failureCallback) {
                    GetCalendarData(successCallback, fetchInfo);
                },
                eventClick: function (info) {
                    getEventDetailsByEventId(info.event);
                }
            });
            calendar.render();
        }
    }
    catch (e) {
        alert(e);
    }
});

//#region modal events
function onShowModal(obj, isEventDetails) {
    if (isEventDetails != null) { ///show event details
        $("#id").val(obj.id);
        $('#title').val(obj.title);
        $('#duration').val(obj.duration);
        $("#workerId").val(obj.appWorkerId);
        $('#btnSend').html('Wyślij');

        $("#description").val(obj.description);
        var startDate = moment(obj.startDate).format("YYYY-MM-DD");
        $('#dateFrom').val(startDate);
        var endDate = moment(obj.endDate).format("YYYY-MM-DD");
        $('#dateEnd').val(endDate);

        if (obj.isRejected) {
            eventRejectedFooter();
            $('#lblStatus').html('Odrzucony');
        }
        else {
            if (obj.isApproved)
                eventApprovedFooter();

            else
                eventNotApprovedFooter();
        }
    }
    else {/// create new event
        $("#workerId").attr("disabled", false);
        $('#lblStatus').html('Brak');

        var x = new moment(obj.startStr);
        var y = new moment(obj.endStr);
        var startDate = moment(obj.startStr).format("YYYY-MM-DD");
        var endDate = moment(obj.endStr).subtract(1, 'day').format("YYYY-MM-DD");
        var duration = y.diff(x, 'days');
        if (duration == 0)
            duration = 1;

        $('#dateFrom').val(startDate);
        $('#dateEnd').val(endDate);

        $('#duration').val(duration);
        newEventFooter();
    }
    $("#appointmentInput").modal("show");
}

function onCloseModal() {
    $('#appointmentForm')[0].reset();
    $("#id").val(0);
    $("#title").val('');
    $("#description").val('');
    $("#dateFrom").val('');
    $("#dateEnd").val('');
    $("#duration").val('');
    $("#appointmentInput").modal("hide");
}

function onDataUpdate(value) {
    var x = new moment(document.getElementById('dateFrom').value);
    var y = new moment(document.getElementById('dateEnd').value);
    var duration = y.diff(x, 'days');
    $('#duration').val(duration + 1);
}

//#endregion

//#region event operations - API calls
function onSubmitForm() {

    if (checkValidation()) {
        var requestData = {
            Id: parseInt($("#id").val()),
            Title: $("#title").val(),
            Description: $("#description").val(),
            StartDate: $("#dateFrom").val(),
            EndDate: $("#dateEnd").val(),
            AppWorkerId: $("#workerId").val(),
        }

        $.ajax({
            url: routeURL + '/api/Vacation/SaveCalendarData',
            type: 'POST',
            data: JSON.stringify(requestData),
            contentType: 'application/json',
            success: function (response) {
                if (!isNaN(response.status) && response.status > 0) {
                    calendar.refetchEvents();
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

function onDeleteEvent() {
    var id = parseInt($("#id").val());
    $.ajax({
        url: routeURL + '/api/Vacation/DeleteVacation/' + id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {
            if (!isNaN(response.status) && response.status > 0) {
                calendar.refetchEvents();
                $.notify(response.message, 'success');
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

function onConfirmEvent() {
    var id = parseInt($("#id").val());
    $.ajax({
        url: routeURL + '/api/Vacation/ConfirmEvent/' + id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {
            if (!isNaN(response.status) && response.status > 0) {
                $.notify(response.message, 'success');
                calendar.refetchEvents();
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

function onRejectEvent() {
    var id = parseInt($("#id").val());
    $.ajax({
        url: routeURL + '/api/Vacation/RejectEvent/' + id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {
            if (!isNaN(response.status) && response.status > 0) {
                $.notify(response.message, 'success');
                calendar.refetchEvents();
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

function getEventDetailsByEventId(info) {
    $.ajax({
        url: routeURL + '/api/Vacation/GetCalendarDataById/' + info.id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {

            if (!isNaN(response.status) && response.status > 0 && response.dataenum != undefined) {
                onShowModal(response.dataenum, true);
            }
            //successCallback(events);
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });
}

function GetCalendarData(successCallback, fetchInfo) {
    var monthDt = new moment(fetchInfo.startStr).format("M");
    $.ajax({
        url: routeURL + '/api/Vacation/GetAllWorkersData',
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {
            var events = [];
            if (!isNaN(response.status) && response.status > 0) {
                $.each(response.dataenum, function (i, data) {
                    //workaround becouse end date was exclusively so we have to add 1 day
                    var endDate = moment(data.endDate).add(1, 'day').format("YYYY-MM-DD");
                    events.push({
                        allDay: true,
                        title: data.title,
                        description: data.description,
                        start: new Date(data.startDate),
                        end: endDate,
                        backgroundColor: getEventBgColor(data),
                        borderColor: "#162466",
                        textColor: "white",
                        id: data.id
                    });
                })
            }
            successCallback(events);
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });
    SetUserVacationDaysData($("#appWorkerId").val());
}

//#endregion

function SetUserVacationDaysData(id) {
    var url = '';
    if (id.length > 10) {
        url = '/' + id;
    }
    $.ajax({
        url: routeURL + '/api/Vacation/GetCurrentUser' + url,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {
            var user = [];
            if (!isNaN(response.status) && response.status > 0) {
                user = response.dataenum
                $('#current-vacation-days').html(user.vacationDays);
                return user;
            }
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });
}

function OnWorkerChange() {
    selectedWorker = $('#appWorkerId').val();
    SetUserVacationDaysData(selectedWorker);
    calendar.refetchEvents();
    $('#workerId').val(selectedWorker);
}

/// === Helpers === ///

//#region footer buttons
function eventRejectedFooter() {
    $('#btnDelete').hide();
    $('#btnSend').hide();
    $('#btnConfirm').hide();
    $('#btnReject').hide();
}

function eventApprovedFooter() {
    $("#lblStatus").html('Zaakceptowany');
    $("#workerId").attr("disabled", true);
    /// buttons
    $('#btnDelete').hide();
    $('#btnSend').hide();
    $('#btnConfirm').hide();
    $('#btnReject').hide();
}

function eventNotApprovedFooter() {
    $("#lblStatus").html('W trakcie akceptacji');
    $("#workerId").attr("disabled", false);

    /// buttons
    $('#btnDelete').show();
    $('#btnSend').show();
    $('#btnSend').html('Zaktualizuj');
    $('#btnConfirm').show();
    $('#btnReject').show();
}

function newEventFooter() {
    /// buttons
    $('#btnDelete').hide();
    $('#btnSend').show();
    $('#btnSend').html('Wyślij');
    $('#btnConfirm').hide();
    $('#btnReject').hide();
}

//#endregion

function getEventBgColor(data) {
    var bgColor;
    if (data.isRejected) {
        bgColor = '#CACACA';
    }
    else {
        data.isApproved ? bgColor = "#28a745" : bgColor = "#dc3545"
    }
    return bgColor;
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