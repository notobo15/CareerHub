$(document).ready(function () {
    // Lấy URL hiện tại
    var currentUrl = window.location.pathname;

    // Duyệt qua tất cả các thẻ <a> trong menu
    $('.menu-items a').each(function () {
        // Lấy giá trị href của thẻ <a>
        var linkUrl = $(this).attr('href');

        // Kiểm tra nếu href chứa URL hiện tại
        if (currentUrl.indexOf(linkUrl) !== -1) {
            // Thêm class 'active' vào thẻ <a> nếu href chứa URL hiện tại
            $(this).addClass('active');
        } else {
            // Loại bỏ class 'active' khỏi các thẻ khác
            $(this).removeClass('active');
        }
    });
});