$(function () {
    $(".new-topic").on('click', function () {
        if ($(this).hasClass("hidden")) {
            $(this).removeClass("hidden");
        } else {
            $(this).addClass("hidden");
        }

        var form = $(".new-topic-form");
        if (form.hasClass("hidden")) {
            form.removeClass("hidden");
        } else {
            form.addClass("hidden");
        }
    });
});