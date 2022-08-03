'use strict';

function copyLink(event, elem){
    navigator.clipboard.writeText(elem.getAttribute("href"));

    $("#copiedAlert").css({
        left: event.pageX + 10,
        top: event.pageY + 10
    }).show(100);
}

function deleteFile(fileName) {
    var requestOptions = {
        method: 'DELETE',
    };

    fetch(`/api/files/${fileName}`, requestOptions)
        .then(result => {
            if (result.status != 200) {
                throw `status code is bad: ${result.status}`;
            }
            window.location.replace("/")
        })
        .catch(error => alert(`error: ${error}`));
}

function hideToolTip() {
    $("#copiedAlert").hide();
}

$(document).ready(function () {

    var fileId = document.URL.split("/").pop();

    var settings = {
        "url": `/api/files/${fileId}`,
        "method": "GET",
        "timeout": 0,
    };

    $.ajax(settings).done(function (response) {
        var element;

        switch (response.type) {
            case 0: // audio
                element = document.createElement("audio");
                break;
            case 1: // video
                element = document.createElement("video");
                break;
            case 2: // image
                element = document.createElement("img");
                break;
            case 3: // text
                element = document.createElement("div");
                element.className = "code text-break";

                var codeNote = document.createElement("pre");

                $.get(`/FileStorage/${fileId}`, function (data) {
                    var text = document.createElement("span");

                    text.innerText = data;
                    codeNote.appendChild(text);
                });

                element.appendChild(codeNote);
                break;
            case 4: // other
                element = document.createElement("h1");
                element.textContent = "Preview is unaccessible :(";
                break;
            default: // error
                alert("error! File type can not be " + response.fileType);
                return;
        }

        var hostAddr = window.location.protocol + "//" + window.location.host;

        element.autoplay = true;
        element.controls = true;
        element.style.cssText = "max-width:100%; max-height: 90vh;";
        element.src = response.link;

        $("#fileContent").append(element);

        var linkElement = $("#clickToDownload");

        linkElement.attr("href", `/FileStorage/${fileId}`);
        linkElement.attr("download", `${fileId}`);

        $("#downloadBtn").on("click", function () {
            window.location.href = `${hostAddr}/api/files/download/${fileId}`;
        });

        $("#copyBtn").attr("href", `${hostAddr}/api/files/download/${fileId}`);

        $("#directBtn").attr("href", `${hostAddr}/FileStorage/${fileId}`);
    });
});
