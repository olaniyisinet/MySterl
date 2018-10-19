if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
    var xmlhttp = new XMLHttpRequest();
}
else {// code for IE6, IE5
    var xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
}
    

function loadResp(fragment_url, element_id) {
 

    var element = document.getElementById(element_id);
    element.innerHTML = '<p><em>Loading ...</em></p>';
    xmlhttp.open("GET", fragment_url);
    xmlhttp.onreadystatechange = function() {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            element.innerHTML = xmlhttp.responseText;
        }
    }
    xmlhttp.send(null);
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


