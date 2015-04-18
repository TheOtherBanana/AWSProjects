var settings = {
    "Palette": ["Yellow", "Orange", "DarkGreen", "Blue", "DarkRed", "Brown"],
    "Font": "Segoe UI",
    "BackgroundColor": "Black",
    "ExcludeCommonWords": true,
    "Width": 1024,
    "Height": 768,
    "MaxFontSize": 60,
    "MinFontSize": 10,
    "WordsToExclude": null
};

chrome.runtime.onMessage.addListener(function (request, sender, response) {
    if (request.showContent == "show") {
        chrome.tabs.query({ active: true, currentWindow: true }, function (tabs) {
            chrome.pageAction.show(tabs[0].id);
            settings.BackgroundColor = $("#backColor").val();
            settings.Palette = $("#textColors").val();
            settings.Font = $("#font").val();
            settings.MaxFontSize = $("#maxFont").val();
            settings.MinFontSize = $("#minFont").val();
            settings.Height = $("#height").val();
            settings.Width = $("#width").val();
            settings.WordsToExclude = $("#custExclude").val();
            chrome.tabs.sendMessage(tabs[0].id, { action: "loadGenerateButtons" });
            chrome.tabs.sendMessage(tabs[0].id, { action:"pageUrl",url: tabs[0].url });
        });
    }
});


$(function () {

    var $selectMax = $(".1-100Max");
    for (i = 1; i <= 100; i++) {
        if (i == 70) {
            $selectMax.append($('<option selected></option>').val(i).html(i))

        }
        else {
            $selectMax.append($('<option></option>').val(i).html(i))
        }
    }


    var $selectMin = $(".1-100Min");
    for (i = 1; i <= 100; i++) {
        if (i == 5) {
            $selectMin.append($('<option selected></option>').val(i).html(i))

        }
        else {
            $selectMin.append($('<option></option>').val(i).html(i))
        }
    }

    $("#height").val(480);
    $("#width").val(640);

    Cookies.json = true;

    var cookieSettings = Cookies.get("settingsCookie");
    if (cookieSettings == null || cookieSettings == undefined) {
        console.log("No cookie settings");
        cookieSettings = settings;
    }

    console.log(cookieSettings);
    $("#backColor").val(cookieSettings.BackgroundColor);
    $("#textColors").val(cookieSettings.Palette);
    $("#font").val(cookieSettings.Font);
    $("#maxFont").val(cookieSettings.MaxFontSize);
    $("#minFont").val(cookieSettings.MinFontSize);
    $("#height").val(cookieSettings.Height);
    $("#width").val(cookieSettings.Width);
    $("#exclude").val(cookieSettings.ExcludeCommonWords);
    $("#custExclude").val(cookieSettings.WordsToExclude);

    $("#textColors").multiselect({

        header: "Select font colors",
        noneSelectedText: "Select font colors",
    });


    $("#backColor").multiselect({
        multiple: false,
        header: "Select background color",
        noneSelectedText: "Select background color",
        selectedList: 1
    });
   

    $("#font").multiselect({
        multiple: false,
        header: "Select font",
        noneSelectedText: "Select font",
        selectedList: 1
    });

    $("#maxFont").multiselect({
        multiple: false,
        header: "Select font",
        noneSelectedText: "Select maximum font size",
        selectedList: 1
    });

    $("#minFont").multiselect({
        multiple: false,
        header: "Select font",
        noneSelectedText: "Select minimum font size",
        selectedList: 1
    });


    $("#exclude").multiselect({
        multiple: false,
        header: "Select font",
        noneSelectedText: "Exclude common words",
        selectedList: 1
    });
    
    
    $("#generatorForm").on("submit", function () {
    
        settings.BackgroundColor = $("#backColor").val();
        settings.Palette = $("#textColors").val();
        settings.Font = $("#font").val();
        settings.MaxFontSize = $("#maxFont").val();
        settings.MinFontSize = $("#minFont").val();
        settings.Height = $("#height").val();
        settings.Width = $("#width").val();
        settings.WordsToExclude = $("#custExclude").val();

        chrome.tabs.query({ active: true, currentWindow: true }, function (tabs) {
            chrome.tabs.sendMessage(tabs[0].id, { action: "refreshSettings", settings: settings });

        });

        Cookies.set("settingsCookie", settings);
        cookieSettings = Cookies.get("settingsCookie");
        console.log(cookieSettings);

        window.close();
        return false;
        
    });

});




