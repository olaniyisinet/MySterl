// JScript File

function doLookUp(entity,element_id)
{
    //alert(entity);
    //var txt = "ctl00$ContentPlaceHolder1$";
    var fragment_url = "../Sync/lookup.aspx?entity=" + entity + "&element=" + element_id;
    window.open (fragment_url,"mywindow");
}
