// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function ScrollToBottom(){
    console.log("You're in the Scroll To Bottom Function")
    // window.scrollTo(0,document.querySelector(".scrollingContainer").scrollHeight);
    var myDiv = document.getElementById("messagesList");
    myDiv.scrollTop = myDiv.scrollHeight;
}