"use strict";

var fileValidation = () => {
    var file = $("#file")[0];

    if (file.files.length > 0) {
        if ((file.files.item(0).size / 1024) > 6000) {
            alert("File is too big! Limit is 6 MB");
        }
        else {
            var formdata = new FormData();
            formdata.append("file", file.files.item(0));

            var requestOptions = {
                method: 'POST',
                body: formdata,
            };

            fetch("/api/files/upload", requestOptions)
                .then(response => response.json())
                .then(result => window.location.replace("/home/show/" + (result.link.split("/").pop())))
                .catch(error => alert(`error: ${error}`));
        }
    }
    else {
        alert("Error. File length is 0!");
    }
}

dropContainer.ondragover = dropContainer.ondragenter = function (evt)
{
    evt.preventDefault();
};

dropContainer.ondrop = function (evt)
{
    file.files = evt.dataTransfer.files;
    evt.preventDefault();
    fileValidation();
};