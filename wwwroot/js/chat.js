"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

//*****// AJG Im trying to add in the BUILDING ID into the message so the receiving HTML knows which messages to display, as long as they match the sending Users's Building ID
connection.on("ReceiveNotification", function (user, message, bldgId) {
    var li = document.createElement("li");
    // var li = document.createElement("div");
    console.log("You've received some message data!")
    // var li = document.createElement(`<b style="color: #000"/>`);
    document.getElementById("messagesList").appendChild(li);
    // document.getElementById("messagesList").prependChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    console.log("You're about to receive a message!")
    li.textContent = `${user} says: ${message}`;
    ScrollToBottom();
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;

    // Here is where I declare each user's group
    console.log("You've reached the start area")
    var bldgIdFromHTML = document.getElementById("bldgId").value;
    console.log("Building Id in JS file is "+bldgIdFromHTML)
    connection.invoke("AddToGroup", bldgIdFromHTML)

}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var bldgId = document.getElementById("bldgId").value;

    document.getElementById('messageInput').focus();
    //TRYING TO SET THIS TO RESET THE INPUT FIELD TO EMPTY AFTER EACH SEND
    // var inputReset = document.getElementById("messageInput");
    // document.getElementById("messageInput").textContent = '';
    // inputReset.value('');
    // inputReset.textContent = '';
    // if (user.Group(groupName)==null) {
        // console.log("You're not part of a group")
        // connection.invoke("AddToGroup", bldgId)
    // }

    // connection.invoke("SendMessage", user, message, bldgId).catch(function (err) {
    connection.invoke("SendNotification", user, message, bldgId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

// set group at message send
// stored ID through messages being sent
// compare with building ID at ChatHub/backend OR at each individual HTML (last resort)