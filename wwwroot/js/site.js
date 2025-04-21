$(document).ready(function () {

    //let isAnimating = false; // Flag để ngăn spam hiệu ứng

    //$('[data-toggle-modal="modal"]').on('click', function () {
    //    const target = $(this).data('target');
    //    const $modal = $(target);
    //    console.log(12312312)
    //    if (!$modal.hasClass('fade')) {
    //        $modal.addClass('fade');
    //    }

    //    $('body').addClass('modal-open');
    //    $modal.css('display', 'block');
    //    setTimeout(() => {
    //        $modal.addClass('show');
    //    }, 10);
    //});

    //// Đóng khi bấm nút close
    //$(document).on('click', '[data-action="close"]', function () {
    //    const $modal = $(this).closest('.modal');
    //    $modal.removeClass('show');
    //    setTimeout(() => {
    //        $modal.css('display', 'none');
    //        $('body').removeClass('modal-open');
    //    }, 150);
    //});

    //// Click nền ngoài
    //$(document).on('click', '.modal', function (e) {
    //    const $modal = $(this);
    //    const $dialog = $modal.find('.modal-dialog');

    //    if (!$(e.target).closest('.modal-dialog').length) {
    //        const isStatic = $modal.attr('data-modal-backdrop') === 'static';

    //        if (isStatic) {
    //            // Ngăn spam nếu đang animate
    //            if (isAnimating) return;

    //            isAnimating = true;
    //            $modal.addClass('modal-static');
    //            setTimeout(() => {
    //                $modal.removeClass('modal-static');
    //                isAnimating = false;
    //            }, 300);
    //        } else {
    //            $modal.removeClass('show');
    //            setTimeout(() => {
    //                $modal.css('display', 'none');
    //                $('body').removeClass('modal-open');
    //            }, 150);
    //        }
    //    }
    //});

  



    $('.currency-input').on('input', function () {
        formatCurrencyInput($(this));
    });

    $('.currency-input').each(function () {
        formatCurrencyInput($(this));
    });


    $('.input-validation-error').each(function () {
        $(this)
            .removeClass('input-validation-error')
            .addClass('is-invalid');
    });

    // Thay class message lỗi
    $('.field-validation-error').each(function () {
        $(this)
            .removeClass('field-validation-error')
            .addClass('invalid-feedback');
    });

})

function formatCurrencyInput($el) {
    let val = $el.val();
    val = val.replace(/\D/g, '');
    val = val.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    $el.val(val);
}
function initModalForm({
    modalSelector,
    triggerSelector,
    formUrl,
    onSuccess = () => window.location.reload,
    onFormLoaded = () => { }
}) {
    // let initialFormData = {};

    // Mở modal
    $(document).on('click', triggerSelector, function () {
        $.get(formUrl, function (data) {
            const $modal = $(modalSelector);
            $modal.find('.modal-dialog').html(data);
            const $form = $modal.find('form');

            $.validator.unobtrusive.parse($form);

            onFormLoaded($form, $modal);

            $modal.modal('show');

            // initialFormData = $form.serializeObject();
        });
    });

    // Submit
    $(document).off('submit', `${modalSelector} form`).on('submit', `${modalSelector} form`, function (e) {
        e.preventDefault();
        const $form = $(this);
        const $modal = $(modalSelector);

        if (!$form.valid()) return;
        $.ajax({
            url: $form.attr('action'),
            method: 'POST',
            data: $form.serialize(),
            success: function (result) { 
                if (result?.success) {
                    location.reload();
                    onSuccess();
                } else {
                    // Nếu vẫn là HTML (form chứa lỗi)
                    $modal.find('.modal-dialog').html(result);
                    const $newForm = $modal.find('form');

                    console.log($newForm)
                    $.validator.unobtrusive.parse($newForm);

                    // initialFormData = $newForm.serializeObject();
                }
             
            }
        });
    });

    // Cảnh báo khi có thay đổi
    // $(document).on('hide.bs.modal', modalSelector, function (e) {
    // 	const currentData = $(this).find('form').serializeObject();
    // 	if (!_.isEqual(currentData, initialFormData)) {
    // 		if (!confirm("Bạn có chắc muốn hủy? Dữ liệu đã thay đổi sẽ bị mất.")) {
    // 			e.preventDefault();
    // 		}
    // 	}
    // });
}