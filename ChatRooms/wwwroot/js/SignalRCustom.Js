$(document).ready(function () {
    if (Notification.permission !== "granted") {
        Notification.requestPermission();
    }
});
var connection = new signalR.HubConnectionBuilder().withUrl("/Chat").build();
var currentGroupId = 0;
var userId = 0;
connection.on("Welcome", function (id) {
    userId = id;
});
connection.on("ReciveMesage", Recive);
function Recive(chat) {

    $("#messageText").val('');
    if (userId === chat.userId) {
        $(".chats").append(`
                    <div class="chat-me">
                      <div class="chat">
                       <p>${chat.chatBody}</p>
                       <span>${chat.createDate}</span>
                      </div>
                    </div>
                  `);
    }
    else {
        $(".chats").append(`
                    <div class="chat-you">
                      <div class="chat">
                       <span>${chat.personName}</span>
                         <p>${chat.chatBody}</p>
                        <span>${chat.createDate}</span>
                      </div>
                    </div>
                  `);
    }

}

connection.on("joinGroup", joined)
connection.on("NewGroup", appenGroup);
connection.on("ReciveNotification", sendNotification)
connection.start();


function sendNotification(chat) {
    if (Notification.permission === "granted") {
        console.log(currentGroupId);
        console.log(chat.groupId);
        if (currentGroupId !== chat.groupId) {
            var notification = new Notification(chat.groupName, {
                body: chat.chatBody,
            });
        }
    }
}
function joined(group, chats) {
    $(".header").css("display", "block");
    $(".footer").css("display", "block");
    $(".header h2").html(group.groupTitle);
    $(".header img").attr("src", "/GroupImage/" + group.imageName);
    currentGroupId = group.id;
    $(".chats").html("");
    chats.forEach(function (element) {
        if (userId === element.userId) {
            $(".chats").append(`
                <div class="chat-me">
                  <div class="chat">
                    <p>${element.text}</p>
                    <span>${element.date}</span>
                  </div>
                </div>
              `);
        } else {
            $(".chats").append(`
                <div class="chat-you">
                  <div class="chat">
                    <span>${element.personName}</span>
                    <br>
                    <p>${element.text}</p>
                    <span>${element.date}</span>
                  </div>
                </div>
              `);
        }
    });
}

function sendmessage(event) {
    event.preventDefault();
    var text = $("#messageText").val();
    var file = event.target[1].files[0];
    var formData = new FormData();
    formData.append("groupId", currentGroupId);
    formData.append("FileAttach", file);
    formData.append("ChatBody", text);
    $.ajax({
        url: "/Home/SendMassage",
        type: "post",
        data: formData,
        enctype: "multipart/form-data",
        processData: false,
        contentType: false
    });
}
async function Start() {
    try {

        await connection.start();
        $(".disconnected").hide();
    }
    catch (e) {
        $(".disconnected").show();
        setTimeout(Start, 3000);
    }

}
connection.onclose(Start);

function joininGroup(token) {
    connection.invoke("joinGroup", token, currentGroupId);
}
function joinInnPrivateGroup(receiverId) {
    
    try {
        connection.invoke("JoinPrivateGroup", receiverId, currentGroupId);
    } catch (e) {
        console.log(e)
    }
    
}


function appenGroup(groupName, Token, imageName) {
    if (groupName === "Error") {
        alert("Error");
    } else {
        $(".rooms #User_Groups ul").append(`
                        <li onclick="joininGroup('${Token}')">
                            ${groupName}
                            <img src="/GroupImage/${imageName}" />
                            <span></span>
                        </li>
                    `);
        $("#exampleModalCenter").modal({show:false});
    }
}

function insertGroup(event) {
    event.preventDefault();
    console.log(event);
    var groupName = event.target[0].value;
    var ImageFile = event.target[1].files[0];
    var formData = new FormData();
    formData.append("GroupName", groupName);
    formData.append("ImageFile", ImageFile);
    $.ajax({
        url: "/Home/CreateGroup",
        type: "post",
        data: formData,
        enctype: "multipart/form-data",
        processData: false,
        contentType: false
    });
}

function search() {
    var text = $("#Search_input").val();
    if (text) {
        // اگر متن جستجو وجود داشت
        $("#Search_Result").show();
        $("#User_Groups").hide();
        $.ajax({
            url: "/Home/Search?title=" + text,
            type: "get",
        }).done(function (data) {
            $("#Search_Result ul").html("");
            data.forEach(function (element) {
                if (element.isUser) {
                    // اگر نتیجه یک کاربر است
                    
                    $("#Search_Result ul").append(`
                        <li onclick="joinInnPrivateGroup(${element.token})">
                            ${element.title}
                            <img src="/img/${element.imageName}" />
                            <span></span>
                        </li>
                    `);
                } else {
                    // اگر نتیجه یک گروه است
                    $("#Search_Result ul").append(`
                        <li onclick="joininGroup('${element.token}')">
                            ${element.title}
                            <img src="/GroupImage/${element.imageName}" />
                            <span></span>
                        </li>
                    `);
                }
            });
        });
    } else {
        // اگر متن جستجو خالی بود
        $("#Search_Result").hide();
        $("#User_Groups").show();
    }
}
