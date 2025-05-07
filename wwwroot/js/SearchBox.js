function toggleClearButton() {
    if ($input?.val()?.trim()?.length > 0) {
        $clearBtn.removeClass("d-none");
    } else {
        $clearBtn.addClass("d-none");
    }
}


$(document).ready(function () {

    if ($('#city').length > 0) {
        new TomSelect('#city', {
            create: false,
            controlInput: null,
            searchField: [],
            placeholder: "Chọn tỉnh/thành phố...",
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
    }

});