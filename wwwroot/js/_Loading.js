$(document).ready(function () {
    // Khi trang tải hoàn tất, ẩn màn hình loading
    $(window).on("load", function () {
        $("#loading-screen").fadeOut(500); // Mờ dần trong 0.5s
    });
});