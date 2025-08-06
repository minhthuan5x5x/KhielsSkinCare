
$(document).ready(function () {
    var current_fs, next_fs, previous_fs; // fieldsets
    var opacity;

    $(".next").click(function () {
        current_fs = $(this).parent();
        next_fs = $(this).parent().next();

        // Add Class Active
        $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

        // Show the next fieldset
        next_fs.show();
        // Hide the current fieldset with style
        current_fs.animate({ opacity: 0 }, {
            step: function (now) {
                // For making fieldset appear animation
                opacity = 1 - now;

                current_fs.css({
                    'display': 'none',
                    'position': 'relative'
                });
                next_fs.css({ 'opacity': opacity });
            },
            duration: 600
        });
    });

    $(".previous").click(function () {
        current_fs = $(this).parent();
        previous_fs = $(this).parent().prev();

        // Remove class active
        $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

        // Show the previous fieldset
        previous_fs.show();

        // Hide the current fieldset with style
        current_fs.animate({ opacity: 0 }, {
            step: function (now) {
                // For making fieldset appear animation
                opacity = 1 - now;

                current_fs.css({
                    'display': 'none',
                    'position': 'relative'
                });
                previous_fs.css({ 'opacity': opacity });
            },
            duration: 600
        });
    });

    $('.radio-group .radio').click(function () {
        $(this).parent().find('.radio').removeClass('selected');
        $(this).addClass('selected');
    });




    $(document).ready(function () {
        $('#confirmPayment').on('click', function (e) {
            e.preventDefault();

            // Lấy phương thức thanh toán đã chọn
            const selectedPaymentMethod = $('#SelectedPaymentMethod').val();

            // Chuẩn bị dữ liệu để gửi tới server
            const formData = {
                email: $('input[name="email"]').val(),
                uname: $('input[name="uname"]').val(),
                FirstName: $('#FirstName').val(),
                LastName: $('#LastName').val(),
                PhoneNumber: $('#PhoneNumber').val(),
                AddressLine: $('#AddressLine').val(),
                City: $('#City').val(),
                paymentMethod: selectedPaymentMethod
            };

            // Kiểm tra phương thức thanh toán
            $.ajax({
                type: 'POST',
                url: '/CheckOut/ProcessCheckout',
                data: formData,
                success: function (response) {
                    if (selectedPaymentMethod === 'payOS' && response.redirectUrl) {
                        // Thanh toán qua PayOS, chuyển hướng đến trang thanh toán của PayOS
                        window.location.href = response.redirectUrl;
                    } else if (selectedPaymentMethod === 'cash' && response.success) {
                        // Thanh toán bằng tiền mặt, chuyển hướng đến trang ThankYou
                        alert('Thanh toán thành công!');
                        window.location.href = '/CheckOut/ThankYou';
                    } else {
                        var errorMessage = response.error || response.message || 'Vui lòng thử lại sau.';
                        alert('Có lỗi xảy ra: ' + errorMessage);
                    }
                },
                error: function () {
                    alert('Có lỗi xảy ra trong quá trình gửi dữ liệu.');
                }
            });
        });
    });
});
