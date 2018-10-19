// JScript File
function printHTML(itm)
{
    var htm = document.getElementById(itm).innerHTML;
    var styl = "<style type='text/css'>* {font-family:Verdana;}</style>";
    newwin = window.open('../sync/printpage.htm','printwin');
    newwin.document.write(styl);
    newwin.document.write("<input type='button' value='PRINT' onclick='window.print()' /><br />");
    newwin.document.write(htm);
    newwin.document.close();
}


function closeInfo()
{
    $(".infoFalse").fadeOut(500);
    $(".infoTrue").fadeOut(500);
}

function rando() {
var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
var string_length = 8;
var randomstring = '';
for (var i=0; i<string_length; i++) {
                var rnum = Math.floor(Math.random() * chars.length);
                randomstring += chars.substring(rnum,rnum+1);
}

return randomstring;
}

//transactions recall
function loadDetails(itm)
{
    var url = "../sync/getRecords.aspx?trx=" + itm;    
    $.get("../sync/getRecords.aspx",{trx:itm,r:rando()},
	function(data){
		$("#bxtrnxDetails").html(data);
	});	
}

function doNameRequest(itm)
{   
    $.get("../sync/doNameRequest.aspx",{trx:itm,r:rando()},
	function(data){
		alert(data);
		//location.reload();
	});	
}

function trnxRequery(itm)
{
    $.get("../sync/doRequery.aspx",{trx:itm,r:rando()},
	function(data){
		alert(data);
		//location.reload();
	});
}

function mailHOP(itm)
{
    $.get("../sync/doMailHOP.aspx",{trx:itm,r:rando()},
	function(data){
		alert(data);
		//location.reload();
	});
}

function trnxAuthorize(itm)
{
    var r = confirm("Are you sure you want to authorize this transaction?");
    if(!r)
    {
        return;
    }
    $.get("../sync/doAuthorize.aspx",{trx:itm,r:rando()},
	function(data){
		alert(data);
		//location.reload();
	});
}


function trnxRetransfer(itm)
{
    $.get("../sync/doRetransfer.aspx",{trx:itm,r:rando()},
	function(data){
		alert(data);
		//location.reload();
	});
}



function trnxReverse(itm)
{
    $.get("../sync/doReverse.aspx",{trx:itm,r:rando()},
	function(data){
		alert(data);
		//location.reload();
	});
}

function trnxTransStatus(itm)
{
    $.get("../sync/doSingleStatusEnquiry.aspx",{trx:itm,r:rando()},
	function(data){
		alert(data);
		//location.reload();
	});
}


function trnxReject(itm)
{
    $.get("../sync/doReject.aspx",{trx:itm,r:rando()},
	function(data){
		alert(data);
		//location.reload();
	});
}