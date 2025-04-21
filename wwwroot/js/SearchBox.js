$(document).ready(function () {
    // Khi click vào dropdown control
    $('#city-ts-control').on('click', function (event) {
        event.stopPropagation(); // Ngăn chặn sự kiện lan ra ngoài
        let wrapper = $(this).closest('.ts-wrapper');
        let dropdown = wrapper.find('.ts-dropdown');

        // Toggle hiển thị dropdown
        if (dropdown.is(':visible')) {
            wrapper.removeClass('dropdown-active focus input-active');
            dropdown.hide();
        } else {
            $('.ts-wrapper').removeClass('dropdown-active focus input-active'); // Đóng dropdown khác
            $('.ts-dropdown').hide();
            wrapper.addClass('dropdown-active focus input-active');
            dropdown.show();
        }
    });

    // Khi chọn một option
    $('.ts-dropdown .option').on('click', function (event) {
        event.stopPropagation();
        let selectedValue = $(this).attr('data-value');
        let selectedText = $(this).text();
        let wrapper = $(this).closest('.ts-wrapper');
        // Cập nhật nội dung đã chọn
        wrapper.find('.item').text(selectedText).attr('data-value', selectedValue);

        // Thay đổi class "selected"
        $('.ts-dropdown .option').removeClass('selected');
        $(this).addClass('selected');
        $('#city option:selected').removeAttr('selected'); // Xóa tất cả selected
        $('#city option[value="' + selectedValue + '"]').attr('selected', 'selected');
        // Đóng dropdown
        wrapper.removeClass('dropdown-active focus input-active');
        wrapper.find('.ts-dropdown').hide();
    });

    // Đóng dropdown khi click ra ngoài
    $(document).on('click', function () {
        $('.ts-wrapper').removeClass('dropdown-active focus input-active');
        $('.ts-dropdown').hide();
    });

    let $input = $("#query");
    let $clearBtn = $(".clear-keyword");

    // Kiểm tra input có giá trị khi trang load
    toggleClearButton();

    // Khi nhập vào input, kiểm tra có hiển thị nút xóa không
    $input.on("input", function () {
        toggleClearButton();
    });

    // Khi click vào nút clear
    $clearBtn.on("click", function (e) {
        e.preventDefault();
        $input.val("").focus(); // Xóa input và focus lại
        toggleClearButton();
    });

    // Hàm hiển thị/ẩn nút xóa
    function toggleClearButton() {
        if ($input?.val()?.trim()?.length > 0) {
            $clearBtn.removeClass("d-none");
        } else {
            $clearBtn.addClass("d-none");
        }
    }
});