var dow = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];
function valButton(btn) {var cnt = -1;for (var i=btn.length-1; i > -1; i--) {if (btn[i].checked) {cnt = i; i = -1;}}if (cnt > -1) return btn[cnt].value;else return null;}
function monthLength(month,year) {var dd = new Date(year, month, 0);return dd.getDate();}
function setCell(f, day, col, dateVar, baseId) {
    var date = new Date(dateVar);
    date.setDate(day);
var c = [];
var t = '<td';
if (f == 0) { c.push('previous'); date.setDate(day); date.setMonth(dateVar.getMonth() - 1); }
if (col==0 || col==6) c.push('weekend');
if (f == 9) { c.push('next'); date.setDate(day); date.setMonth(dateVar.getMonth() + 1); }
var id = (date.getFullYear().toString() + date.getMonth().toString() + date.getDate().toString());
t += ' id="td-' + id + '" data-app-id="' + id + '" ' + '" data-app-date="' + date + '" '
if (c.length>0) t+=' class="'+c.join(' ')+'"';
t += '><div class="topContent" id="topContent-' + baseId + id + '"><span class="date">' + day + '<\/span></div><div id="' + baseId + id + '" class="day"><\/div><\/td>\n';
return t;
}
function setCalendar(y,m) {
    if (y < 1901 || y > 2100) { alert('year must be after 1900 and before 2101'); return false; }
    if (m < 0 || m > 11) { alert('Month must be in between 0 and 11'); return false; }
var c = new Date();
c.setDate(1);
c.setMonth(m);
c.setFullYear(y);
var x = parseInt("1",10);
var s = (c.getDay()-x)%7; if (s<0) s+=7;
var dm = monthLength(m, y);
var baseId = y.toString() + m.toString();
var h = '<table cellpadding="0" cellspacing="0" class="Calendar" id="' + baseId + '">\n<thead>\n<tr>\n';
for (var i=0;i<7;i++) {
h+= '<th';
if ((i+x)%7==0 || (i+x)%7==6) h+= ' class="weekend"';
h+= '>'+dow[(i+x)%7]+'<\/th>\n';}
h += '<\/tr>\n<\/thead>\n<tbody>\n<tr>\n';
for (var i=s;i>0;i--) {
    h += setCell(0, dm - i + 1, (s - i + x) % 7, c, baseId);
}
dm = monthLength(m + 1, y);
for(var i=1; i <= dm; i++) {
if((s%7)==0) {h += '<\/tr><tr>\n'; s = 0;}
h += setCell(1, i, (s + x) % 7, c, baseId);
s++;
}
var j=1;
for (var i = s; i < 7; i++) {
    h += setCell(9, j, (i + x) % 7, c, baseId);
j++;
}
h += '<\/tr>\n<\/tbody>\n<\/table>';
return h;
}