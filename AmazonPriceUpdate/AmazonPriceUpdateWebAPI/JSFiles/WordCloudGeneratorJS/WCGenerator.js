/*
    WCGenerator.js
*/
$(document).ready(function () {

    "use strict";
    var serviceApiBaseUrl = "http://theotherbananaapis.elasticbeanstalk.com//api/wordcloudapi";
    //var serviceApiBaseUrl = "http://localhost:59132/api/wordcloudapi";

    var colors = ["Red","Orange","Green","Yellow"];
    var background = "Black";
    var font = "Helvetica";
    var excludeCommonWords = true;
    var imageDetails = $("#imageDetails");
    imageDetails.text("Your image will come here");
    
    $("#generatorForm").on("submit", function () {
        var text = $("#textToGen").val();
        imageDetails.text("Processing");
        var jsonRequestObj = {
            "Text":text,
            "Palette":colors,
            "Font":font,
            "BackgroundColor":background,
            "ExcludeCommonWords":excludeCommonWords
        };

        console.log(jsonRequestObj);
        $.ajax({
            url: serviceApiBaseUrl,
            type: "POST",
            cache: false,
            data: JSON.stringify(jsonRequestObj),
            contentType: "application/json",
            success: function (result) {
                console.log(result);
                var result = "<img src=" + result.ImgurUrl  + "/>";
                imageDetails.empty();
                imageDetails.append(result);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                var errorResultMessage = "Generation failed with error " + xhr.status + "<br/>" + thrownError;
                confirmationResult.empty();
                confirmationResult.append(errorResultMessage);
            }
        });
        return false;
    });
});