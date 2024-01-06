function sendmessage(event) {
    event.preventDefault();
    var text = $("#messageText").val();
    if (text) {
        connection.invoke("SendMessage", text, currentGroupId);
    } else {
        alert("Error")
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
        $("#Search_Result").show();
        $("#User_Groups").hide();
        $.ajax({
            url: "/Home/Search?title=" + text,
            type: "get",

        }).done(function (data) {
            $("#Search_Result ul").html("");
            /*console.log(data);*/
            data.forEach(function (element) {
                if (element.isUser) {
                    $("#Search_Result ul").append(`
                                                <li onclick="joininGroup('${element.token}')">
                                                    ${element.title}
                                                    <img src="/img/${element.imageName}" />
                                            <span></span>
                                        </li>
                     `);
                } else {
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
        $("#Search_Result").hide();
        $("#User_Groups").show();
    }
}