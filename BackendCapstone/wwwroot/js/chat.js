
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (storyBoardId, user, message) {
   var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
   var encodedMsg = msg;
   var div = document.createElement("div");  
   div.textContent = encodedMsg;
   document.getElementById(`messagesList--${storyBoardId}`).appendChild(div);
});



connection.start().then(function () {
console.log("connected to hub")}).catch (function (err) {
return console.error(err.toString());
});



document.getElementById("storyBoardsList").addEventListener("click", function (event) {
    if (event.target.id.startsWith("sendButton")) {
        var storyBoardId = event.target.id.split("--")[1];
        console.log(`${storyBoardId} button clicked`);
        var user = document.getElementById(`userInput--${storyBoardId}`).value;
        var message = document.getElementById(`messageInput--${storyBoardId}`).value;
        connection.invoke("SendMessage", storyBoardId, user, message);   
    }
});