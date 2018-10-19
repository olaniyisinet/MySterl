$(document).ready(function () {
    //when the regular button is clicked
    //hide the content of the div slowly
    $('#nubanaccount').click(function () {
        $('#displaynuban').hide('slow');
        $('#processnuban').show('slow');
        $('#processnuban').html("Processing your request... Please wait");
        if ($('#bracode').val() == "") {
            alert("Oosp! Please input branch code");
            return;
        }
        if ($('#curcode').val() == "") {
            alert("Oosp! Please input currency code");
            return;
        }
        if ($('#glcode').val() == "") {
            alert("Oosp! Please input ledger code");
            return;
        }
        if ($('#cif').val() == "") {
            alert("Oosp! Please input customer number (CIF)");
            return;
        }
        if ($('#slno').val() == "") {
            alert("Oosp! Please input sl number");
            return;
        }
        
        
        

        $.ajax({
            url: "ProcessTransactions.aspx",
            type: "get",
            data: "bracode=" + $('#bracode').val() + "&curcode=" + $('#curcode').val() + "&glcode=" + $('#glcode').val() + "&cif=" + $('#cif').val() + "&slno=" + $('#slno').val() + "&action=nubanaccount",
            success: function (data) {
                //alert("success");
                $("#processnuban").html(data);
            },
            error: function () {
                //alert("failure");
                $("#processnuban").html('Sorry! Your request could not be completed' +
                    '<br/><p><button type="button" id="reloadregular2" class="btn btn-default btn-lg btn-block" runat="server"> Reload this screen</button></p>');
            }
        });
    });
    
    $('#dotrnx').click(function () {
        if ($('#nuban').val() == "") {
            alert("Oosp! Please input a 10 digit number");
            return;
        }
        if ($('#item').val() == "0") {
            alert("Oosp! Please select an action");
            return;
        }
        $('#processregular3').show('slow');
        $('#displayregular3').hide('slow');
        $('#processregular3').html("Processing your request... Please wait");

        $.ajax({
            url: "ProcessTransactions.aspx",
            type: "get",
            data: "nuban=" + $('#nuban').val() + "&action=" + $('#item').val()+"&username="+$('#username').val()+"&password="+$('#password').val(),
            success: function (data) {
                //alert("success");
                $("#processregular3").html(data);
            },
            error: function () {
                //alert("failure");
                $("#processregular3").html('Sorry! Your request could not be completed' +
                    '<br/><p><button type="button" id="reloadregular3" class="btn btn-default btn-lg btn-block" runat="server"> Reload this screen</button></p>');
            }
        });
    });
   
    $('#regularaccount').click(function () {
        if ($('#nuban').val() == "") {
            alert("Oosp! Please input a 10 digit number");
            return;
        }
        $('#processregular').show('slow');
        $('#displayregular').hide('slow');
        $('#processregular').html("Processing your request... Please wait");
        
        $.ajax({
            url: "ProcessTransactions.aspx",
            type: "get",
            data: "nuban=" + $('#nuban').val() + "&action=regularaccount",
            success: function (data) {
                //alert("success");
                $("#processregular").html(data);
            },
            error: function () {
                //alert("failure");
                $("#processregular").html('Sorry! Your request could not be completed' +
                    '<br/><p><button type="button" id="reloadregular" class="btn btn-default btn-lg btn-block" runat="server"> Reload this screen</button></p>');
            }
        });
    });
    //display processing transactions
    //connect to the service
    //get the details
    //display the details to the user
    
    $("#processregular").on('click', '#reloadregular',function () {
        //alert("I got here...");
        $('#processregular').hide('slow');
        $('#displayregular').show('slow');
        $('#nuban').val("");

    });
    $("#processregular3").on('click', '#reloadregular3', function () {
        //alert("I got here...");
        $('#processregular3').hide('slow');
        $('#displayregular3').show('slow');
        $('#nuban').val("");

    });
    $("#processnuban").on('click', '#reloadregular2', function () {
        //alert("I got here...");
        $('#processnuban').hide('slow');
        $('#displaynuban').show('slow');
        $('#cif').val("");

    });
   
    
});