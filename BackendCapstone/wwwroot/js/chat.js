
"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();




connection.on("RemoveApproval", function (storyBoardId) {
    var approvalCheck = document.getElementById(`approvalCheck--${storyBoardId}`);
    approvalCheck.remove();
});

connection.on("ReceiveApproval", function (storyBoardId) {
    var approvalBox = document.getElementById(`approvalBox--${storyBoardId}`)
    function approvalCheckHTML () {
        return `
        <img id = "approvalCheck--${storyBoardId}"
            src="/images/checkmark.png" 
            style="width: 30px; height: 30px;"
         />
        `
    }
    approvalBox.innerHTML += approvalCheckHTML();
});

connection.on("CallerReceiveMessage", function (storyBoardId, userId, message, timestamp) {
   var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
   var encodedMsg = msg;
    var input = document.getElementById(`messageInput--${storyBoardId}`);
    var messageList = document.getElementById(`messagesList--${storyBoardId}`)
    function callerMessageHTML() {
        return `        
            <div style="background-color:#1D9EF1; color:#FFFFFF; max-width:350px; 
                    min-width:15px; display:flex; flex-direction:row; 
                    align-self:flex-end; border-radius:8px; margin:10px; padding:8px;">
                <p style="margin: 8px">
                    ${encodedMsg}
                </p>
            </div>
        `
    }
    messageList.innerHTML += callerMessageHTML();
    messageList.scrollTop = messageList.scrollHeight;
    input.value = ""
});



connection.on("OthersReceiveMessage", function (storyBoardId, userId, userName, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");   
    var encodedMsg = msg;
    var input = document.getElementById(`messageInput--${storyBoardId}`);
    var messageList = document.getElementById(`messagesList--${storyBoardId}`)
    function otherMessageHTML() {
        return `        
            <div style="background-color:#EEEEEE; max-width:350px; 
                    min-width:15px; display:flex; flex-direction:row; 
                    align-self:flex-start; border-radius:8px; margin:10px; padding:8px;">
                <p style="font-weight:bold; margin:5px">
                    ${userName}
                </p>
                <p style="margin: 5px">
                    ${encodedMsg}
                </p>
            </div>
        `
    }
    messageList.innerHTML += otherMessageHTML();   
    messageList.scrollTop = messageList.scrollHeight;
    input.value = ""
});


connection.start().then(function () {
console.log("connected to hub")}).catch (function (err) {
return console.error(err.toString());
});




document.getElementById("storyBoardsList").addEventListener("click", function (event) {
    if (event.target.id.startsWith("sendButton")) {
        var storyBoardId = event.target.id.split("--")[1];
        console.log(`${storyBoardId} button clicked`);
        var userId = document.getElementById(`userId--${storyBoardId}`).value;
        var message = document.getElementById(`messageInput--${storyBoardId}`).value;        
        connection.invoke("SendMessage", storyBoardId, userId, message);   
    }
});


//write conditional so that event listener is present only for admin and client users
var userTypeId = document.getElementById("userTypeId").value;
if (userTypeId != "2") {
    document.getElementById("storyBoardsList").addEventListener("click", function (event) {
        if (event.target.id.startsWith("approval")) {       
            var storyBoardId = event.target.id.split("--")[1];
            console.log(`${storyBoardId} approval box clicked`);
            var approvalBox = document.getElementById(`approvalBox--${storyBoardId}`)
            console.log(approvalBox.childNodes);         
            connection.invoke("SendApproval", storyBoardId);
        };
    });
};
    



