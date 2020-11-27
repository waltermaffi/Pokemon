function endSearching() {
    $("#searchButton").prop('disabled', false);
    $("#searchButton").html("Search");
    $("#keyInput").prop('disabled', false);
}

function search() {
    var key = $("#keyInput").val();
    $("#searchButton").prop('disabled', true);
    $("#searchButton").html("Searching...");
    $("#keyInput").prop('disabled', true);

    $.ajax({
        type: 'GET',
        url: 'api/Pokemon/' + key,
        data: { key: key },
        success: function (data) {
            var jsonObj = JSON.parse(data);
            $("#resultText").val(jsonObj["description"]);
            endSearching()
        },
        error: function (ex) {
            endSearching()
        }  
    })
}

$('#keyInput').keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        search()
    }

});