$(document).ready(function () {

    $(document).on('click', 'a[href]:not([target="_blank"]):not([href^="#"]):not([href^="javascript:"])', function (e) {
        const href = $(this).attr('href');
        if (href && !$(this).hasClass('no-loading')) {
            $('#loading-screen').fadeIn(200); // Hiện loading khi chuyển trang
        }
    });

    //$(document).on('submit', 'form', function () {
    //    $('#loading-screen').fadeIn(200); // Hiện loading khi submit form
    //});
    $("#loading-screen").fadeOut(500); // Mờ dần trong 0.5s
});