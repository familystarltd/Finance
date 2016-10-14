jQuery.extend({ loadHtmlEditor: function () { $.getScript("/lib/tinymce/tinymce.min.js").done(function (e, t) { window.tinymce.dom.Event.domLoaded = !0, tinyMCE.init({ selector: "textarea", height: "100%", width: "100%", verify_html: !1, plugins: "pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,wordcount,advlist", theme_advanced_buttons1: "fullscreen,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,cut,copy,paste,pastetext,pasteword,|,tablecontrols,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,forecolor,backcolor,|,print", theme_advanced_buttons2: "", theme_advanced_buttons3: "", theme_advanced_buttons4: "", theme_advanced_toolbar_location: "top", theme_advanced_toolbar_align: "left", theme_advanced_statusbar_location: "", theme_advanced_resizing: !1, content_css: "/Content/css/bootstrap.min.css", convert_urls: !1, template_external_list_url: "lists/template_list.js", external_link_list_url: "lists/link_list.js", external_image_list_url: "lists/image_list.js", media_external_list_url: "lists/media_list.js" }) }) } }), $(document).ready(function () { $(".ui-icon-close").on("click", function (e) { e.preventDefault(); var t = $(this).closest(".message-content"); $(t).hide() }) }), Date.prototype.toLongMonthName = function () { locale = "en-uk"; var e = this.valueOf(); return new Date(e).toLocaleString(locale, { month: "long" }) }, Date.prototype.addDays = function (e) { var t = this.valueOf(); return t += 864e5 * e, new Date(t) }, Date.prototype.addMonths = function (e) { var t = new Date(this.valueOf()), n = this.getMonth(), r = this.getYear(); return n = (n + e) % 12, 0 > n ? (r += (this.getMonth() + e - n - 12) / 12, n += 12) : r += (this.getMonth() + e - n) / 12, t.setMonth(n), t.setYear(r), t }, jQuery.extend({ isProperty: function (e) { return "undefined" != typeof e } }), jQuery.fn.extend({ check: function () { return this.each(function () { this.checked = !0 }) }, uncheck: function () { return this.each(function () { this.checked = !1 }) } }), jQuery.extend({ formatZeros: function (e) { return e.replace(/ (\d$)/, " 00$1").replace(/ (\d\d)$/, " 0$1") } }), jQuery.extend({ date: function (e, t) { return e = new Date(e), t = t.toUpperCase(), t = t.replace("DD", (e.getDate() < 10 ? "0" : "") + e.getDate()), t = t.replace("MM", (e.getMonth() < 9 ? "0" : "") + (e.getMonth() + 1)), t = t.replace("YYYY", e.getFullYear()) } }), jQuery.extend({ toJsonDate: function (e) { return new Date(e.getTime() - 6e4 * e.getTimezoneOffset()).toJSON() } }), jQuery.extend({ getMonthName: function (e) { return e = new Date(e), isNaN(e.getMonth()) ? "" : ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][e.getMonth()] } }), jQuery.extend({ getWeekDayName: function (e) { return e = new Date(e), isNaN(e.getDate()) ? "" : ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"][e.getDay()] } }), jQuery.extend({ isNullOrEmpty: function (e) { return "null" === e || null === e || "" === e || "undefined" == typeof e } }), jQuery.extend({ clearForm: function (e) { $(e).find(":input").each(function () { switch (this.type) { case "password": case "select-multiple": case "select-one": case "text": case "textarea": $(this).val(""); break; case "checkbox": case "radio": this.checked = !1 } }) } }), jQuery.extend({ getJsonObject: function (e, t) { var n = jQuery.param(t).split("&"); if (!$.isNullOrEmpty(e)) for (var r = 0; r < e.length; ++r) { for (var a = e[r], i = !1, s = 0; s < n.length; s++) { var o = n[s].split("="); if ($.isNullOrEmpty(a[o[0]]) && $.isNullOrEmpty(o[1])) i = !0; else { if (a[o[0]] != o[1]) { i = !1; break } i = !0 } } if (i) return a } return {} } }), jQuery.extend({ getJsonObjects: function (e, t) { var n = jQuery.param(t).split("&"), r = []; if (!$.isNullOrEmpty(e)) for (var a = 0; a < e.length; ++a) { for (var i = e[a], s = !1, o = 0; o < n.length; o++) { var u = n[o].split("="); if ($.isNullOrEmpty(i[u[0]]) && $.isNullOrEmpty(u[1])) s = !0; else { if (i[u[0]] != u[1]) { s = !1; break } s = !0 } } s && r.push(i) } return r } }), jQuery.extend({ removeJsonObject: function (e, t, n) { var r, a = !1; for (r in e) if (e[r][t] == n) { a = !0; break } a && e.splice(r, 1) } }), jQuery.extend({ formatCurrency: function (e, t) { $.isNullOrEmpty(e) && (e = 0); e = $.toNumber(e); var n = !1; return e < 0 && (n = !0, e = Math.abs(e)), e = parseFloat(e, 10).toFixed(2), isNaN(e) ? "£0.00" : (n ? "-" : "") + (t ? t : "") + e.replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() } }), jQuery.extend({ toNumber: function (e) { return $.isNullOrEmpty(e) && (e = 0), isNaN(e) ? (e = Number(e.replace(/[^0-9\.]+/g, "")), parseFloat(e)) : parseFloat(e) } }), jQuery.extend({ currenciesOnly: function (e) { if (46 != e.which && 163 != e.which && 8 != e.which && 0 != e.which && (e.which < 48 || e.which > 57)) return !1 } }), jQuery.extend({ numbersOnly: function (e) { if (8 != e.which && 0 != e.which && (e.which < 48 || e.which > 57)) return !1 } }), jQuery.extend({ Guid: { Set: function (e) { var t; return 1 == arguments.length && (t = this.IsValid(arguments[0]) ? arguments[0] : this.Empty()), $(this).data("value", t), t }, Empty: function () { return "00000000-0000-0000-0000-000000000000" }, IsEmpty: function (e) { return rGx = new RegExp("\\b(?:[A-F0-9]{8})(?:-[A-F0-9]{4}){3}-(?:[A-F0-9]{12})\\b"), e = rGx.exec(e.toString().toUpperCase()), e == this.Empty() || "undefined" == typeof e || null == e || "" == e }, IsValid: function (e) { return rGx = new RegExp("\\b(?:[A-F0-9]{8})(?:-[A-F0-9]{4}){3}-(?:[A-F0-9]{12})\\b"), null != rGx.exec(e.toString().toUpperCase()) }, New: function () { 1 == arguments.length && this.IsValid(arguments[0]) && ($(this).data("value", arguments[0]), value = arguments[0]); for (var e, t = [], n = new RegExp("[2345]"), r = 0; r < 8; r++) e = (65536 * (1 + Math.random()) | 0).toString(16).substring(1), null != n.exec(r.toString()) && (3 == r && (e = "6" + e.substr(1, 3)), t.push("-")), t.push(e.toUpperCase()); return value = t.join(""), $(this).data("value", value), value }, Value: function () { if ($(this).data("value")) return $(this).data("value"); var e = this.New(); return $(this).data("value", e), e } } })(), jQuery.extend({ postJSON: function (e, t, n) { return jQuery.isFunction(t) && (n = t, t = void 0), jQuery.ajax({ url: e, type: "POST", xhrFields: { withCredentials: !0 }, crossDomain: !0, contentType: "application/json", dataType: "json", data: JSON.stringify(t), success: n, error: function (e) { if (404 == e.status || 0 == e.status || 500 == e.status) return void alert("Please contact system Administrator") } }) } }), jQuery.extend({ getJSON: function (e, t, n) { return jQuery.ajax({cache:false, url: e, type: "GET", xhrFields: { withCredentials: !0 }, crossDomain: !0, success: t, error: function (e) { if (404 == e.status || 500 == e.status) return void alert("Please contact system Administrator"); if (e && e.responseText) { var t = JSON.parse(e.responseText); alert(e.statusText + " : " + t.exceptionMessage) } } }) } }), jQuery.extend({ Storage: function () { this.get = function (e) { return JSON.parse(window.localStorage.getItem(e)) }, this.set = function (e, t) { window.localStorage.setItem(e, JSON.stringify(t)) }, this.clear = function () { window.localStorage.clear() } } }), jQuery.extend({ deleteJSON: function (e, t) { return jQuery.ajax({ url: e, type: "DELETE", dataType: "json", xhrFields: { withCredentials: !0 }, crossDomain: !0, success: t, error: function (e) { if (404 == e.status || 0 == e.status || 500 == e.status) return void alert("Please contact system Administrator"); var t = JSON.parse(e.responseText); alert(e.statusText + " : " + t.exceptionMessage) }, beforeSend: function (e) { } }) } }), jQuery.extend({ setCookie: function (e, t, n) { var r = new Date; r.setTime(r.getTime() + 24 * n * 60 * 60 * 1e3); var a = "expires=" + r.toUTCString(); document.cookie = e + "=" + t + "; " + a } }), jQuery.extend({ getCookie: function (e) { if (matched = document.cookie.match(RegExp(e + "=.[^;]*")), matched) { var t = matched[0].split("="); return t[1] } return "[]" } }), jQuery.extend({ toTimeString: function (e) { if (!$.isNullOrEmpty(e)) { var t = e.split("."); return e = t.length > 1 ? t[1].replace(/[:]/g, "") : t[0].replace(/[:]/g, ""), e.substr(0, 2) + ":" + e.substr(2, 2) } return "00:00" } }), jQuery.extend({ toMilliseconds: function (e, t) { var n = new Date, r = new Date; if (!$.isNullOrEmpty(e)) { var a = e.split("."); a.length > 1 ? (e = a[1].split(":"), n.setDate(n.getDate() + 1)) : e = a[0].split(":"), n.setHours(e.length > 0 ? e[0] : 0, e.length > 1 ? e[1] : 0, 0, 0) } if (!$.isNullOrEmpty(t)) { var a = t.split("."); a.length > 1 ? (t = a[1].split(":"), r.setDate(r.getDate() + 1)) : t = a[0].split(":"), r.setHours(t.length > 0 ? t[0] : 0, t.length > 1 ? t[1] : 0, 0, 0) } var i = r - n; return i } }), jQuery.extend({ toMillisecondsByDate: function (e, t) { var e = new Date(e).getTime(), t = new Date(t).getTime(); return t - e } }), jQuery.extend({ setTimeToDate: function (e) { var t = new Date; if (!$.isNullOrEmpty(e)) { var n = e.split("."); return n.length > 1 ? (e = n[1].replace(/[:]/g, ""), t.setDate(t.getDate() + 1)) : e = n[0].replace(/[:]/g, ""), new Date(t.setHours(e.substr(0, 2), e.substr(2, 2), 0, 0)) } return t.setHours(0, 0, 0, 0) } }), jQuery.extend({ setTimeToDate: function (e, t) { var n = new Date(e); if (isNaN(n.getDate()) && (n = new Date, t = e), !$.isNullOrEmpty(t)) { var r = t.split("."); return r.length > 1 ? (t = r[1].replace(/[:]/g, ""), n.setDate(n.getDate() + 1)) : t = r[0].replace(/[:]/g, ""), new Date(n.setHours(t.substr(0, 2), t.substr(2, 2), 0, 0)) } return new Date(n.setHours(0, 0, 0, 0)) } }), jQuery.extend({ zeroPad: function (e, t) { var n = t - e.toString().length + 1; return Array(+(n > 0 && n)).join("0") + e } }), jQuery.extend({ toHoursAndMinutes: function (e) { var t = e / 60 / 1e3, n = e / 3600 / 1e3, r = Math.floor(n); return $.zeroPad(Math.floor(n), 2) + ":" + $.zeroPad(t - 60 * r, 2) } }), jQuery.extend({ toHoursAndMinutesByDate: function (e, t) { var e = new Date(e).getTime(), t = new Date(t).getTime(), n = t - e, r = n / 60 / 1e3, a = n / 3600 / 1e3, i = Math.floor(a); return Math.floor(a).toString() + ":" + (r - 60 * i).toString() } }), jQuery.extend({ loadData: function (e, t, n) { n || (n = "GET"), $(e).empty(), $(e).each(function (r, a) { if (t && t.length > 0) { var i = '<img id="ScheduleLoader" class="ico-sm" src="/images/local-loading.gif"style="position:absolute;left:50%;top:50%;"/>'; $(e).append(i); var s = t.split("?"), o = t + (s && s.length > 1 ? "&" + 1e6 * Math.random() : "?" + 1e6 * Math.random()); $.ajax({ url: o, type: n, cache: !1, context: a, success: function (e) { $(a).html(e) } }) } }) } }), jQuery.extend({ noofdays: function (e, t) { var n; return n = 4 == e || 6 == e || 9 == e || 11 == e ? 30 : 31, 2 == e && (n = t / 4 - parseInt(t / 4) != 0 ? 28 : t / 100 - parseInt(t / 100) != 0 ? 29 : t / 400 - parseInt(t / 400) != 0 ? 28 : 29), n } });