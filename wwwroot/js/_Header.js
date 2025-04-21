$(document).ready(function () {
    $(".menu-title").hover(
        function () {
            $(".menu-title").removeClass("active");
            $(this).addClass("active");
        },
        function () {
            // Nếu muốn giữ class active khi hover xong thì bỏ dòng này
            // $(this).removeClass("active");
        }
    );

    $('.hamburger-menu').on('click', function () {
        $('.left-nav').toggleClass('d-none'); // Thêm hoặc xóa class `d-none`
    });

    // Đóng menu khi nhấn vào nút Close
    $('.btnCloseLeftNav').on('click', function () {
        $('.left-nav').addClass('d-none'); // Thêm lại class `d-none` để ẩn
    });

    $('.btnShowSubMenu').on('click', function () {
        // Tìm menu cùng cấp có class `sub-menu` và thay đổi width
        $(this).siblings('.sub-menu').css('width', '280px');
    });
    $(".btnCloseSubMenu").on('click', function () {
        $('.left-nav').addClass('d-none'); // Thêm lại class `d-none` để ẩn
    })
    // Khi nhấn vào nút Back
    $('.btnBackSubMenu').on('click', function () {
        // Tìm menu cha có class `sub-menu` và đặt width = 0
        $(this).closest('.sub-menu').css('width', '0');
    });
    $(".user-menu-wrapper").click(function () {
        $(".right-nav").toggleClass("d-none");
    });

    $(".btnCloseRightNav").click(function () {
        $(".right-nav").addClass("d-none");
    })


    $(document).click(function (event) {
        if (!$(event.target).closest('.right-nav, .user-menu-wrapper').length) {
            $(".right-nav").addClass("d-none");
        }
        if (!$(event.target).closest('.left-nav, .hamburger-menu').length) {
            $(".left-nav").addClass("d-none");
        }
    });
});