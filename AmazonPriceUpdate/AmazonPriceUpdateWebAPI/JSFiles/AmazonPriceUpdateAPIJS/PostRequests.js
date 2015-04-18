/*
    PostRequests.js
*/
$(document).ready(function () {

    "use strict";

    var urlCountryCode = {
        "in": "IN",
        "com": "US",
        "co.uk": "UK",
        "br": "BR",
        "ca": "CA",
        "cn": "CN",
        "de": "DE",
        "es": "ES",
        "fr": "FR",
        "it": "IT",
        "co.jp":"JP"
    };
    var productASIN = "";
    var productEmailId = "";
    var serviceApiBaseUrl = "http://theotherbananaapis.elasticbeanstalk.com//api/amazonproductapi";
    //var serviceApiBaseUrl = "http://localhost:59132/api/amazonproductapi";

    var productDetails = $("#productUpdateDetails");
    productDetails.text("Your product details will be updated here");
    $("#productPriceUpdateForm").on("submit", function () {
        var url = $("#productUrl").val();
        if (url) {
            productDetails.text("Processing the request...");
            var asinRegex = new RegExp('/([A-Z0-9]{10})(?:[/?]|$)');
            var regionRegex = new RegExp('.amazon.[a-z.]+')
            var asin = url.match(asinRegex);
            var region = url.match(regionRegex);
            asin = asin[0].replace('/', "").replace('/', "");
            asin = asin.substr(0, 10);
            region = region[0].replace(".amazon.", '');
            region = urlCountryCode[region];

            var emailId = $("#emailId").val();
            var duration = $("#duration").val();
            var price = $("#priceToEmail").val();

            var jsonRequestObj = {
                "EmailId": emailId,
                "ProductASIN": asin,
                "ProductRegion": region,
                "ToEmailOnPrice": "true",
                "EmailOnPriceDuration": duration,
                "PriceToEmail": price,
                "ProductPriceType": "All"
            }
            console.log(jsonRequestObj);
            $.ajax({
                url: serviceApiBaseUrl,
                type: "PUT",
                cache: false,
                data: JSON.stringify(jsonRequestObj),
                contentType: "application/json",
                success: function (result) {
                    console.log(result);
                    productDetails.empty();
                    var newResult = $("<br/><br/><form action='' method='POST' id='productConfirmForm' class='productConfirmForm'>" +
                        "<div class='result'>" +
                        "<div class='title'>" + result.ProductName + "</div>" +
                        "<div>Current Price: " + result.CurrentPrice + " " + result.CurrencyCode + "</div>" +
                        "<div>From: Amazon " + result.AmazonUrl + "</div>" +
                        "<div>EmailId: " + result.EmailId + "</div>" +
                        "</div><br/>"+
                        "<input type='submit' value='Click To Confirm' />" +
                        "</form>");
                    var confirmationResult = $("<div id='confirmationResult'>" +
                        "<div id='confirmationDetails'></div>" +
                        "</div>")
                    productDetails.append(newResult);
                    productDetails.append(confirmationResult);
                    //Set the ASIN and EmailId
                    //productASIN = result.ProductASIN;
                    //productEmailId = result.EmailId;
                    $("#productConfirmForm").on("submit", function () {
                        console.log("Inside confirmation bloc");
                        var confirmationResult = $("#confirmationResult");
                        confirmationResult.text("Confirmation request is being processed\n");
                        var confirmJsonRequestObj = {
                            "EmailId": result.EmailId,
                            "ASIN": result.ProductASIN
                        }
                        $.ajax({
                            url: serviceApiBaseUrl+ "?confirmed=true",
                            type: "PUT",
                            cache: false,
                            data: JSON.stringify(confirmJsonRequestObj),
                            contentType: "application/json",
                            success: function (result) {
                                confirmationResult.empty();
                                confirmationResult.append(result);
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                var errorResultMessage = "Confirmation failed with error " + xhr.status + "<br/>" + thrownError;
                                confirmationResult.empty();
                                confirmationResult.append(errorResultMessage);
                            }
                        });
                        return false;
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    var errorMessage = "Get product details failed " + xhr.status + "<br/>" + thrownError;
                    var asinRegInfo = "<br/>Ensure that your country code and ASIN are correct. " + "<br/>Country code: " + region + "<br/>ASIN: " + asin;
                    productDetails.empty();
                    productDetails.append(errorMessage);
                    productDetails.append(asinRegInfo);
                }
            });
        }

        return false;
    });

});