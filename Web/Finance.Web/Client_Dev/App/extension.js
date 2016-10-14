jQuery.extend({
    loadHtmlEditor: function () {
        window.tinyMCEPreInit = { base: '/lib/tinymce/', suffix: '', query: '' };
        $.getScript("~/lib/tinymce/tinymce.min.js").done(function (script, textStatus) {
            window.tinymce.dom.Event.domLoaded = true;
            tinyMCE.init({ mode: "specific_textareas", editor_selector: "HtmlEditor", theme: "advanced", height: "100%", width: "100%", verify_html: false, plugins: "pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,wordcount,advlist", theme_advanced_buttons1: "fullscreen,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,cut,copy,paste,pastetext,pasteword,|,tablecontrols,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,forecolor,backcolor,|,print", theme_advanced_buttons2: "", theme_advanced_buttons3: "", theme_advanced_buttons4: "", theme_advanced_toolbar_location: "top", theme_advanced_toolbar_align: "left", theme_advanced_statusbar_location: "", theme_advanced_resizing: false, content_css: "/Content/css/bootstrap.min.css", convert_urls: false, template_external_list_url: "lists/template_list.js", external_link_list_url: "lists/link_list.js", external_image_list_url: "lists/image_list.js", media_external_list_url: "lists/media_list.js", });
        });
    }
});
$(document).ready(function () {
    $(".ui-icon-close").on("click", function (e) {
        e.preventDefault();
        var div = $(this).closest('.message-content');
        $(div).hide();
    });
});
Date.prototype.toLongMonthName = function () {
    locale = "en-uk";
    var value = this.valueOf();
    return new Date(value).toLocaleString(locale, { month: "long" });
}
Date.prototype.addDays = function (num) {
    var value = this.valueOf();
    value += 86400000 * num;
    return new Date(value);
}
Date.prototype.addMonths = function (num) {
    var value = new Date(this.valueOf()), mo = this.getMonth(),yr = this.getYear();
    mo = (mo + num) % 12;
    if (0 > mo) { yr += (this.getMonth() + num - mo - 12) / 12; mo += 12; }
    else { yr += ((this.getMonth() + num - mo) / 12); }
    value.setMonth(mo);value.setYear(yr);return value;
}
jQuery.extend({ isProperty: function (propertry) { return typeof (propertry) != 'undefined' } });
jQuery.fn.extend({ check: function () { return this.each(function () { this.checked = true; }); }, uncheck: function () { return this.each(function () { this.checked = false; }); } });
jQuery.extend({ formatZeros: function (s) { return s.replace(/ (\d$)/, ' 00$1').replace(/ (\d\d)$/, ' 0$1'); } });
jQuery.extend({ date: function (date, format) {/* Calculate date parts and replace instances in format string accordingly*/date = new Date(date); format = format.toUpperCase(); format = format.replace("DD", (date.getDate() < 10 ? '0' : '') + date.getDate()); /* Pad with '0' if needed*/format = format.replace("MM", (date.getMonth() < 9 ? '0' : '') + (date.getMonth() + 1)); /*Months are zero-based*/format = format.replace("YYYY", date.getFullYear()); return format; } });
jQuery.extend({ toJsonDate: function (date) { return new Date(date.getTime() - (date.getTimezoneOffset() * 60000)).toJSON(); } });
jQuery.extend({
    getMonthName: function (date) {
        date = new Date(date); if (isNaN(date.getMonth())) { return ""; } return ["January", "February", "March","April", "May", "June","July", "August", "September","October", "November", "December"][date.getMonth()]}});
jQuery.extend({ getWeekDayName: function (date) { date = new Date(date); if (isNaN(date.getDate())) { return ""; } return ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'][date.getDay()] } });
jQuery.extend({ isNullOrEmpty: function (data) { if (data === "null" || data === null || data === "" || typeof data === "undefined") { return true; } else { return false; } } });
jQuery.extend({clearForm: function (ele) {
        $(ele).find(':input').each(function () {
            switch (this.type) {
                case 'password':
                case 'select-multiple':
                case 'select-one':
                case 'text':
                case 'textarea':
                    $(this).val('');
                    break;
                case 'checkbox':
                case 'radio':
                    this.checked = false;
            }
        });
    }
});
jQuery.extend({
    getJsonObject: function (jsonData, keyValues) {
        var vars = jQuery.param(keyValues).split("&");
        var DATA = null;
        if (!$.isNullOrEmpty(jsonData)) {
            //alert(jsonData.length);
            for (var i = 0; i < jsonData.length; ++i) {
                var data = jsonData[i];
                var isExixts = false;
                for (var x = 0; x < vars.length; x++) {
                    var pair = vars[x].split("=");
                    //alert(pair[0]);
                    //alert(pair[1]);
                    if ($.isNullOrEmpty(data[pair[0]]) && $.isNullOrEmpty(pair[1])) {
                        isExixts = true;
                    }
                    else if (data[pair[0]] == pair[1]) {
                        isExixts = true;
                    }
                    else { isExixts = false; break; }
                }
                if (isExixts) return data;
            }
        }
        return {};
    }});
jQuery.extend({getJsonObjects: function (jsonData, keyValues) {
        var vars = jQuery.param(keyValues).split("&");
        var DATA = [];
        if (!$.isNullOrEmpty(jsonData)) {
            //alert(jsonData.length);
            for (var i = 0; i < jsonData.length; ++i) {
                var data = jsonData[i];
                var isExixts = false;
                for (var x = 0; x < vars.length; x++) {
                    var pair = vars[x].split("=");
                    //alert(pair[0]);
                    //alert(pair[1]);
                    if ($.isNullOrEmpty(data[pair[0]]) && $.isNullOrEmpty(pair[1])) {
                        isExixts = true;
                    }
                    else if (data[pair[0]] == pair[1]) {
                        isExixts = true;
                    }
                    else { isExixts = false; break; }
                }
                if (isExixts) DATA.push(data);
            }
        }
        return DATA;
    }});
jQuery.extend({removeJsonObject: function (obj, prop, val) { var c, found = false; for (c in obj) { if (obj[c][prop] == val) { found = true; break; } } if (found) { obj.splice(c, 1); } } });
jQuery.extend({
    formatCurrency: function (total, symbol) {
        if ($.isNullOrEmpty(total)) { total = 0; }
        total = $.toNumber(total);
        var neg = false;
        if (total < 0) {
            neg = true;
            total = Math.abs(total);
        }
        total = parseFloat(total, 10).toFixed(2);
        if (!isNaN(total)) { return (neg ? '-' : '') + (symbol ? symbol : '') + total.replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString(); }
        else { return '£' + "0.00"; }
}
});
jQuery.extend({ toNumber: function (number) { if ($.isNullOrEmpty(number)) { number = 0; } if (!isNaN(number)) { return parseFloat(number); } number = Number(number.replace(/[^0-9\.]+/g, "")); return parseFloat(number); } });
jQuery.extend({ currenciesOnly: function (e) { if (e.which != 46 && e.which != 163 && e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) { return false; } } });
jQuery.extend({ numbersOnly: function (e) { if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) { return false; } } });
jQuery.extend({Guid: {
        Set: function (val) {var value;if (arguments.length == 1) {if (this.IsValid(arguments[0])) {value = arguments[0];} else {value = this.Empty();}}$(this).data("value", value);return value;},
        Empty: function () {return "00000000-0000-0000-0000-000000000000";},
        IsEmpty: function (gid) { rGx = new RegExp("\\b(?:[A-F0-9]{8})(?:-[A-F0-9]{4}){3}-(?:[A-F0-9]{12})\\b"); gid = rGx.exec(gid.toString().toUpperCase()); return gid == this.Empty() || typeof (gid) == 'undefined' || gid == null || gid == ''; },
        IsValid: function (value) { rGx = new RegExp("\\b(?:[A-F0-9]{8})(?:-[A-F0-9]{4}){3}-(?:[A-F0-9]{12})\\b"); return rGx.exec(value.toString().toUpperCase()) != null; },
        New: function () {if (arguments.length == 1 && this.IsValid(arguments[0])) {$(this).data("value", arguments[0]);value = arguments[0];}
            var res = [], hv;
            var rgx = new RegExp("[2345]");
            for (var i = 0; i < 8; i++) {
                hv = (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
                if (rgx.exec(i.toString()) != null) {if (i == 3) { hv = "6" + hv.substr(1, 3); }res.push("-");}res.push(hv.toUpperCase());}value = res.join('');$(this).data("value", value);return value;},
        Value: function () { if ($(this).data("value")) { return $(this).data("value"); } var val = this.New(); $(this).data("value", val); return val; }
    }
})();
jQuery.extend({
    postJSON: function (url, data, callback) {
        if (jQuery.isFunction(data)) { callback = data; data = undefined; }
        return jQuery.ajax({
            url: url, type: "POST", xhrFields: { withCredentials: true }, crossDomain: true, contentType: "application/json",
            dataType: "json", data: JSON.stringify(data), success: callback,
            error: function (xhr) { if (xhr.status == 404 || xhr.status == 0 || xhr.status == 500) { alert("Please contact system Administrator"); return; } /*var jsonResponse = JSON.parse(xhr.responseText); alert(xhr.statusText + " : " + jsonResponse.exceptionMessage);*/ }
        });
    }
});
jQuery.extend({
    getJSON: function (url, callback, addHeader) {
        return jQuery.ajax({
            url: url, type: "GET",xhrFields: { withCredentials: true }, crossDomain: true, success: callback,
            error: function (xhr) { if (xhr.status == 404 || xhr.status == 500) { alert("Please contact system Administrator"); return; } if (xhr && xhr.responseText) { var jsonResponse = JSON.parse(xhr.responseText); alert(xhr.statusText + " : " + jsonResponse.exceptionMessage); } }
        });
    }
});
jQuery.extend({Storage:
function() {
    this.get = function (name) {return JSON.parse(window.localStorage.getItem(name));};
    this.set = function (name, value) { window.localStorage.setItem(name, JSON.stringify(value));};
    this.clear = function () { window.localStorage.clear();};}
});
jQuery.extend({
    deleteJSON: function (url, callback) { return jQuery.ajax({ url: url, type: "DELETE", dataType: "json", xhrFields: { withCredentials: true }, crossDomain: true, success: callback, error: function (xhr) { if (xhr.status == 404 || xhr.status == 0 || xhr.status == 500) { alert("Please contact system Administrator"); return; } var jsonResponse = JSON.parse(xhr.responseText); alert(xhr.statusText + " : " + jsonResponse.exceptionMessage); }, beforeSend: function (request) { } }); }
});
jQuery.extend({
    setCookie:function(cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires;
    }
});
jQuery.extend({
    getCookie: function (cname) {
        var name = cname + "=";
        matched = document.cookie.match(RegExp(cname + "=.[^;]*"));
        if (matched) {
            var cookie = matched[0].split('=');
            return cookie[1];
        }
        return "[]";
    }
});
jQuery.extend({
    toTimeString: function (Time) {
        if (!$.isNullOrEmpty(Time)) {
            var split = Time.split(".");
            if (split.length > 1) {
                Time = split[1].replace(/[:]/g, "");
            }
            else { Time = split[0].replace(/[:]/g, ""); }
            return Time.substr(0, 2) + ":" + Time.substr(2, 2);
        }
        return "00:00";
    }
});
jQuery.extend({
    toMilliseconds: function (TimeIn, TimeOut) {
        var StartDate = new Date();
        var EndDate = new Date();
        if (!$.isNullOrEmpty(TimeIn)) {
            var split = TimeIn.split(".");
            if (split.length > 1) {TimeIn = split[1].split(":");StartDate.setDate(StartDate.getDate() + 1);}
            else { TimeIn = split[0].split(":"); }
            StartDate.setHours(TimeIn.length > 0 ? TimeIn[0] : 0, TimeIn.length > 1 ? TimeIn[1] : 0, 0, 0);
        }
        if (!$.isNullOrEmpty(TimeOut)) {
            var split = TimeOut.split(".");
            if (split.length > 1) {TimeOut = split[1].split(":");EndDate.setDate(EndDate.getDate() + 1);}
            else { TimeOut = split[0].split(":"); }
            EndDate.setHours(TimeOut.length > 0 ? TimeOut[0] : 0, TimeOut.length > 1 ? TimeOut[1] : 0, 0, 0);
        }
        var hourDiff = EndDate - StartDate;
        return hourDiff; //in ms
    }
});
jQuery.extend({toMillisecondsByDate: function (StartDate, EndDate) {
        var StartDate = new Date(StartDate).getTime();
        var EndDate = new Date(EndDate).getTime();
        return EndDate - StartDate; //in ms
    }
});
jQuery.extend({
    setTimeToDate: function (Time) {
        var ToDay = new Date();
        if (!$.isNullOrEmpty(Time)) {
            var split = Time.split(".");
            if (split.length > 1) {
                Time = split[1].replace(/[:]/g, "");
                ToDay.setDate(ToDay.getDate() + 1);
            }
            else { Time = split[0].replace(/[:]/g, ""); }
            return new Date(ToDay.setHours(Time.substr(0, 2), Time.substr(2, 2), 0, 0));
        }
        return ToDay.setHours(0, 0, 0, 0);
    }
});
jQuery.extend({
    setTimeToDate: function (date, Time) {
        var ToDay = new Date(date);
        if (isNaN(ToDay.getDate())) { ToDay = new Date(); Time = date; }
        if (!$.isNullOrEmpty(Time)) {
            var split = Time.split(".");
            if (split.length > 1) {
                Time = split[1].replace(/[:]/g, "");
                ToDay.setDate(ToDay.getDate() + 1);
            }
            else { Time = split[0].replace(/[:]/g, ""); }
            return new Date(ToDay.setHours(Time.substr(0, 2), Time.substr(2, 2), 0, 0));
        }
        return new Date(ToDay.setHours(0, 0, 0, 0));
    }
});
jQuery.extend({
    zeroPad: function (num, places) {
        var zero = places - num.toString().length + 1;
        return Array(+(zero > 0 && zero)).join("0") + num;
    }
});
jQuery.extend({
    toHoursAndMinutes: function (milliSeconds) {
        var secDiff = milliSeconds / 1000; //in s
        var minDiff = milliSeconds / 60 / 1000; //in minutes
        var hDiff = milliSeconds / 3600 / 1000; //in hours
        var hours = Math.floor(hDiff);
        return $.zeroPad(Math.floor(hDiff),2) + ":" + $.zeroPad((minDiff - 60 * hours),2);
    }
});
jQuery.extend({
    toHoursAndMinutesByDate: function (timeStart,timeEnd) {
        var timeStart = new Date(timeStart).getTime();
        var timeEnd = new Date(timeEnd).getTime();
        var hourDiff = timeEnd - timeStart; //in ms
        var secDiff = hourDiff / 1000; //in s
        var minDiff = hourDiff / 60 / 1000; //in minutes
        var hDiff = hourDiff / 3600 / 1000; //in hours
        var hours = Math.floor(hDiff);
        return Math.floor(hDiff).toString() + ":" + (minDiff - 60 * hours).toString();
    }
});
jQuery.extend({
    loadData: function (controlId, dataUrl, requestType) {
        if (!requestType)
            requestType = "GET";
        $(controlId).empty();
        $(controlId).each(function (index, item) {
            if (dataUrl && dataUrl.length > 0) {
                var loader = '<img id="ScheduleLoader" class="ico-sm" src="/images/local-loading.gif"style="position:absolute;left:50%;top:50%;"/>';
                $(controlId).append(loader); var randomURL = dataUrl.split("?");
                var URL = dataUrl + (randomURL && randomURL.length > 1 ? "&" + (Math.random() * 1000000) : "?" + (Math.random() * 1000000));
                $.ajax({
                    url: URL,
                    type:requestType,
                    cache: false,
                    context: item,
                    success: function (result) {
                        $(item).html(result);
                    }
                });
                //$(item).load(dataUrl + (randomURL && randomURL.length > 1 ? "&" + (Math.random() * 1000000) : "?" + (Math.random() * 1000000)), function (response, status, xhr) { if (xhr.status == 404) { alert("Please contact system Administrator"); return; } if (status == "error") { var msg = "Sorry unable to process your request please try again later! "; $("#error").html(msg + xhr.status + " " + xhr.statusText); } });
            }
        });
    }
});
jQuery.extend({noofdays: function (month, year) {
        var daysofmonth;
        if ((month == 4) || (month == 6) || (month == 9) || (month == 11)) { daysofmonth = 30; } else { daysofmonth = 31; }
        if (month == 2) { if (year / 4 - parseInt(year / 4) != 0) { daysofmonth = 28; } else if (year / 100 - parseInt(year / 100) != 0) { daysofmonth = 29; } else if (year / 400 - parseInt(year / 400) != 0) { daysofmonth = 28; } else { daysofmonth = 29; } }
        return daysofmonth;}});
