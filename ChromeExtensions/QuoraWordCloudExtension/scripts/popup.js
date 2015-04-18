// JavaScript source code

chrome.tabs.query({ active: true, currentWindow: true }, function (tabs) {
    alert("sending message");
    chrome.tabs.sendMessage(tabs[0].id, { action: "loadGenerateButtons" });
});

$(document).ready(function () {

    "use strict";

    alert("Yo!");
    
});