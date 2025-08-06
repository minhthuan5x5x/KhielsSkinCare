document.addEventListener('DOMContentLoaded', function () {
    const dropdownMenus = document.querySelectorAll('.dropdownMenu');

    dropdownMenus.forEach(dropdownMenu => {
        const productId = dropdownMenu.getAttribute('data-product-id');
        const firstOption = dropdownMenu.options[dropdownMenu.selectedIndex];
        const initialPrice = firstOption.getAttribute('data-price');
        document.getElementById(`priceDisplay-${productId}`).textContent = initialPrice;

        dropdownMenu.addEventListener('change', function () {
            const selectedOption = this.options[this.selectedIndex];
            const price = selectedOption.getAttribute('data-price');
            const variantId = selectedOption.value;

            // Cập nhật giá hiển thị
            document.getElementById(`priceDisplay-${productId}`).textContent = price;

            // Cập nhật variantId cho nút "THÊM VÀO GIỎ HÀNG"
            const addToCartButton = this.closest('.product').querySelector('.add-to-cart');
            addToCartButton.setAttribute('data-variant-id', variantId);
        });
    });
});

// Thêm sản phẩm vào giỏ hàng
function addToCart(button) {
    var productId = $(button).data('product-id');
    var variantId = $(button).data('variant-id');

    $.ajax({
        url: '@Url.Action("Add", "Cart")',
        type: 'POST',
        data: { Id: productId, variantId: variantId },
        success: function (response) {
            alert("Sản phẩm đã được thêm vào giỏ hàng!");
        },
        error: function (xhr, status, error) {
            alert("Đã có lỗi xảy ra: " + xhr.responseText);
        }
    });
}