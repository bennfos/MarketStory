
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established

connection.on("RemoveApproval", function (storyBoardId) {
    var approvalCheck = document.getElementById(`approvalCheck--${storyBoardId}`);
    approvalCheck.remove();
});

connection.on("ReceiveApproval", function (storyBoardId) {
    var img = document.createElement("img");
    img.src = "/images/checkmark.png"
    img.style.width = "30px";
    img.style.height = "30px";
    img.id = `approvalCheck--${storyBoardId}`

    document.getElementById(`approvalBox--${storyBoardId}`).appendChild(img);
    alert("Approved!");
});

connection.on("CallerReceiveMessage", function (storyBoardId, userId, message) {
   var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
   var encodedMsg = msg;
    var div = document.createElement("div");
    var pMessage = document.createElement("p")
    div.style.backgroundColor = "#1D9EF1";
    div.style.color = "#FFFFFF";
    div.style.maxWidth = "350px";
    div.style.minWidth = "15px";
    div.style.display = "flex";
    div.style.flexDirection = "row";
    div.style.alignSelf = "flex-end";
    div.style.borderRadius = "8px";
    div.style.margin = "10px";
    div.style.padding = "8px";
    pMessage.style.margin = "5px";
    pMessage.textContent = encodedMsg;  
    div.appendChild(pMessage);
    document.getElementById(`messagesList--${storyBoardId}`).appendChild(div);
    var input = document.getElementById(`messageInput--${storyBoardId}`);
    input.value = ""
});

connection.on("OthersReceiveMessage", function (storyBoardId, userId, userName, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");   
    var encodedMsg = msg;
    var div = document.createElement("div");
    var pUser = document.createElement("p");
    var pMessage = document.createElement("p")
    div.style.backgroundColor = "#EEEEEE";   
    div.style.maxWidth = "350px";
    div.style.minWidth = "15px";
    div.style.display = "flex";
    div.style.flexDirection = "row";
    div.style.alignSelf = "flex-start";
    div.style.borderRadius = "8px";
    div.style.margin = "10px";
    div.style.padding = "8px";
    pUser.style.fontWeight = "bold";
    pUser.style.margin = "5px";
    pMessage.style.margin = "5px";
    pUser.textContent = userName;
    pMessage.textContent = encodedMsg;
    div.appendChild(pUser);
    div.appendChild(pMessage);
    document.getElementById(`messagesList--${storyBoardId}`).appendChild(div);
    var input = document.getElementById(`messageInput--${storyBoardId}`);
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


document.getElementById("storyBoardsList").addEventListener("click", function (event) {
    if (event.target.id.startsWith("messageInput")) {
        var storyBoardId = event.target.id.split("--")[1];
        connection.invoke("TypingIndicator", storyBoardId);
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
    



