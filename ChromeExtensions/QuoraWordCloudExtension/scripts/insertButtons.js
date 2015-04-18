var requestWithSettings = {
    "Text": null,
    "Palette": ["Yellow","Orange","DarkGreen","Blue","DarkRed","Brown"],
    "Font": "Segoe UI",
    "BackgroundColor": "Black",
    "ExcludeCommonWords": true,
    "Width":1024,
    "Height": 768,
    "MaxFontSize": 70,
    "MinFontSize": 15,
    "WordsToExclude":null
};
var serviceApiBaseUrl = "http://localhost:59132/api/wordcloudapi";
//var serviceApiBaseUrl = "http://theotherbananaapis.elasticbeanstalk.com//api/wordcloudapi";

var pageUrl;
chrome.runtime.sendMessage({ showContent: "show" });

chrome.runtime.onMessage.addListener(function (request, sender, response) {
    if (request.action == "refreshSettings") {
        requestWithSettings.BackgroundColor = request.settings.BackgroundColor;
        requestWithSettings.Palette = request.settings.Palette;
        requestWithSettings.Font = request.settings.Font;
        requestWithSettings.ExcludeCommonWords = request.settings.ExcludeCommonWords;
        requestWithSettings.Width = request.settings.Width;
        requestWithSettings.Height = request.settings.Height;
        requestWithSettings.MinFontSize = request.settings.MinFontSize;
        requestWithSettings.MaxFontSize = request.settings.MaxFontSize;
        requestWithSettings.WordsToExclude = request.settings.WordsToExclude;
    }
});

chrome.runtime.onMessage.addListener(function (request, sender, response) {
    if (request.action == "pageUrl") {
        pageUrl = request.url;
    }
});

chrome.runtime.onMessage.addListener(function (request, sender, response) {
    if (request.action == "loadGenerateButtons") {
        (function ($) {
            $(document).on("click ready",function () {

                "use strict";
                var insertButton = "<div class='generateWordCloud'><button type='button'style = 'box-sizing: border-box;  transition: all ease-in-out 100ms; border-radius: 3px; box-shadow: 0 1px 1px 0 rgba(200,200,200,0.2); display: inline-block; font-weight: 500; padding: 3px 7px 4px 7px; text-align: center; text-decoration: none; cursor: pointer; background: #f1f8fb; color: #2b6dad; border: 1px solid #bbcadc;'>Get Word Cloud</button></div>";
                var insertButtonMedium = "<div class='generateWordCloudMedium' style='height:0px;width:0px' class='u-floatCenter'>" +
                    "<button class = 'button button--vertical' type='button' title='Get Word Cloud of this post' style='height:55px;vertical-align: top;line-height:1'><img src='https://email-template-files.s3-us-west-2.amazonaws.com/cloud-outline-hi.png' alt='Get Word Cloud' height='30' width='30' />"
                    + "<span style='height:0px;width:0px;display:block'></span><span class='button-label is-default'>Get Word Cloud</span></button></div>"
                var insertButtonMediumMetabar = "<div class='generateWordCloudMedium' style='height:0px;width:0px' class='metabar-block metabar-center u-floatCenter'>" +
                    "<button class = 'button button--vertical' type='button' title='Get Word Cloud of this post' style='height:55px;vertical-align: top;line-height:1'><img src='https://email-template-files.s3-us-west-2.amazonaws.com/cloud-outline-hi.png' alt='Get Word Cloud' height='30' width='30' />"
                    + "<span style='height:0px;width:0px;display:block'></span><span class='button-label is-default'>Get Word Cloud</span></button></div>"

                $(".pagedlist_item").each(function (i, pageobj) {
                    var jqpageObj = $(pageobj);
                    var expandedAnswer = $(jqpageObj).find(".ExpandedQText.QuotableExpandedAnswer.ExpandedAnswer");
                    var answerBase = $(jqpageObj).find(".Answer.AnswerBase");

                    if ($(expandedAnswer).length > 0 || $(answerBase).length > 0) {
                        var jobj = $(jqpageObj).find('.Answer.ActionBar');
                        var insertButtonClass = $(jqpageObj).find(".generateWordCloud");
                        if ($(insertButtonClass).length == 0) {
                            jobj.append(insertButton);
                        }
                    }
                });

                $(".more_link").on("click", function () {
                    var parentPagedList = $(this).parents(".pagedlist_item");
                    var jqParent = $(parentPagedList);
                    var jobj = $(jqParent).find('.Answer.ActionBar');
                    var insertButtonClass = $(jqParent).find(".generateWordCloud");
                    if ($(insertButtonClass).length == 0) {
                        jobj.append(insertButton);
                    }
                });

                $(".Answer.AnswerStandalone").on("ready click",function () {
                    var jobj = $(this).find('.Answer.ActionBar');
                    var insertButtonClass = $(this).find(".generateWordCloud");
                    if ($(insertButtonClass).length == 0) {
                        jobj.append(insertButton);
                    }
                });

                var bottomMeta = $(".metabar.u-clearfix.metabar--bottom.metabar--bordered.metabar--social.metabar--postSecondaryBar.js-postSecondaryBar");
                var insertButtonClass = $(bottomMeta).find(".generateWordCloudMedium");
                if ($(insertButtonClass).length == 0) {
                    console.log("Inside if");
                    //$(insertButtonMedium, bottomMeta).insertAfter(".metabar-block.metabar-left.u-floatLeft");
                    //$(".metabar-block.metabar-left.u-floatLeft").empty()
                    //$(".metabar-block.metabar-left.u-floatLeft").append(insertButton);
                    //$(bottomMeta).empty();
                   
                    $(".metabar-block.metabar-left.u-floatLeft", bottomMeta).after(insertButtonMediumMetabar);
                }

                var bottomStaticMeta = $(".u-clearfix.postFooter-actions--simple2")
                insertButtonClass = $(bottomStaticMeta).find(".generateWordCloudMedium");
                if ($(insertButtonClass).length == 0) {
                    $(".u-floatLeft", bottomStaticMeta).after(insertButtonMedium);
                }
                
                $(".generateWordCloud").unbind().on("click", function () {

                    answerDiv = $(this).parents(".pagedlist_item");
                    var html, answers = null;

                    if (answerDiv) {
                        var expandedAnswerdiv = $('.ExpandedQText.QuotableExpandedAnswer.ExpandedAnswer', answerDiv);
                        answers = $('div[id$="_container"]', answerDiv);
                        
                        if (answers.html() != undefined) {
                            answers = answers;
                        }
                        else if (expandedAnswerdiv.html() != undefined) {
                            //html = expandedAnswerdiv.html();
                            answers = expandedAnswerdiv;
                        }
                        else {
                            
                            var answerDiv = $(this).parents('.Answer.AnswerStandalone');
                            var expandedAnswerdiv = $('.inline_editor_value', answerDiv);
                            console.log(expandedAnswerdiv);
                            answers = $('div[id$="_container"]', answerDiv);
                            if (answers.html() != undefined) {
                                answers = answers;
                            }
                            else if (expandedAnswerdiv.html() != undefined) {
                                answers = expandedAnswerdiv;
                            }
                            
                        }
                    }
                    $("div.container_boundary", answers).remove();
                    html = answers.html();
                    console.log(html);
                    $.magnificPopup.open({
                        items: {
                            src: 'http://bit.ly/1FEcPSf'
                        },
                        type: 'image',
                        closeBtnInside: false
                    });
                    
                    requestWithSettings.Text = html;
                    $.ajax({
                        url: serviceApiBaseUrl,
                        type: "POST",
                        cache: false,
                        data: JSON.stringify(requestWithSettings),
                        contentType: "application/json",
                        success: function (result) {
                            $.magnificPopup.open({
                                items: {
                                    src: result.ImgurUrl
                                },
                                type: 'image'
                            });
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            var errorResultMessage = "Generation failed. Check your settings and try after sometime";

                            $.magnificPopup.open({
                                items: {
                                    src: result.ImgurUrl
                                },
                                type: 'image',
                                tError: 'The image could not be loaded. ' + errorResultMessage
                            });
                            console.log(errorResultMessage);
                        }
                    });
                });

                $(".generateWordCloudMedium").unbind().on("click", function () {
                    

                    console.log(pageUrl);

                    requestWithSettings.Text = pageUrl;
                    $.ajax({
                        url: serviceApiBaseUrl,
                        type: "POST",
                        cache: false,
                        data: JSON.stringify(requestWithSettings),
                        contentType: "application/json",
                        success: function (result) {
                            $.magnificPopup.open({
                                items: {
                                    src: result.ImgurUrl
                                },
                                type: 'image'
                            });
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            var errorResultMessage = "Generation failed. Check your settings and try after sometime";

                            $.magnificPopup.open({
                                items: {
                                    src: result.ImgurUrl
                                },
                                type: 'image',
                                tError: 'The image could not be loaded. ' + errorResultMessage
                            });
                            console.log(errorResultMessage);
                        }
                    });
                        
                    });
            });

        }(jQuery));

    }


});