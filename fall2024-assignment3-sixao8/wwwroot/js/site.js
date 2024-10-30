// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function displayTweets(tweets) {
    var item = document.getElementById("tweets");

    console.log("1");

    for (var i = 0; i < tweets.length; i++) {
        console.log(tweets[i]);
    }

    return;
}