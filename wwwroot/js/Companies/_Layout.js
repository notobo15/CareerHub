//$(document).ready(function () {
//    $(".read-more-btn").click(function () {
//        let container = $(this).closest(".read-more-container");
//        container.toggleClass("show");

//        container.find("[data-read-more-target='moreTextLink']").addClass("d-none");
//        container.find("[data-read-more-target='lessTextLink']").removeClass("d-none");
//    });

//    $(".read-less-btn").click(function () {
//        let container = $(this).closest(".read-more-container");
//        container.toggleClass("show");

//        container.find("[data-read-more-target='moreTextLink']").removeClass("d-none"); s
//        container.find("[data-read-more-target='lessTextLink']").addClass("d-none");
//    });

//    $('.location').on('click', function () {
//        var newMapUrl = $(this).data('map-url');
//        $('#googleMap').attr('src', newMapUrl);

//        $('.location').each(function () {
//            $(this).removeClass('active');
//        });

//        $(this).addClass('active');
//    });

//    var swiper = new Swiper('.swiper-container', {
//        slidesPerView: 3,
//        spaceBetween: 10,
//        pagination: {
//            el: ".swiper-pagination",
//            clickable: true,
//        },
//    });


//});