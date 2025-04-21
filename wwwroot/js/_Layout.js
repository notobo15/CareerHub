$(document).ready(function () {
    // Chỉ áp dụng khi màn hình nhỏ hơn 992px (responsive)
    if ($(window).width() < 992) {

        // Khi dropdown cha bị đóng, tự động đóng tất cả submenu bên trong
        $('.navbar .dropdown').on('hidden.bs.dropdown', function () {
            $(this).find('.submenu').hide(); // Ẩn tất cả submenu
        });

        // Xử lý khi click vào các link có submenu
        $('.dropdown-menu a').on('click', function (e) {
            var $nextEl = $(this).next('.submenu'); // Tìm submenu liền kề

            if ($nextEl.length) { // Nếu có submenu
                e.preventDefault(); // Ngăn không cho chuyển trang

                // Toggle mở/đóng submenu
                $nextEl.slideToggle(200); // Dùng hiệu ứng mượt mà
            }
        });
    }

    // Cập nhật feather icons (nếu có dùng thư viện này)
    if (typeof feather !== 'undefined') {
        feather.replace();
    }
});