$("#register-btn").click(function() {
    $("#login").slideUp()
    $("#register").fadeIn()
})
$("#login-btn").click(function() {
    $("#register").slideUp()
    $("#login").slideDown()
})
$(document).ready(function () {
    var route = location.href.split("#")[1];
    if (route) {
        if (route=="Register") {
            $("#login").slideUp()
            $("#register").fadeIn()
        } else {
            $("#register").slideUp()
            $("#login").slideDown()
        }
    }
});