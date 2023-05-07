$(function () {

    $('.btn-addCart').off('click').on('click', function () {
        //var quantity = $(this).data('quantity');
        //if (quantity == null)
        //    quantity = $('#num_quantity').val();
        $.ajax({
            type: 'GET',
            url: '/Cart/AddCart',
            data: {
                product_ID: $(this).data('id'),
                quantity: 1
            },
            dataType: 'Json',
            contentType: "application/json; charset=utf-8",
            success: function (res) {
                if (res.status == true) {
                    var count = $('.ajax-cart-quantity').text();
                    if (count == "") {
                        $('.ajax-cart-quantity').text('1');
                    } else {
                        var Soluong = parseInt(count) + 1;
                        $('.ajax-cart-quantity').text(Soluong);
                    }
                    const notice = PNotify.success({
                        title: 'THÔNG BÁO!',
                        text: 'Thêm sản phẩm vào giỏ hàng thành công. Click để xem giỏ hàng.',
                        stack: new PNotify.Stack({
                            dir1: 'down',
                            dir2: 'right',
                            push: 'top',
                            maxStrategy: 'close'
                        })
                    });
                    notice.refs.elem.style.cursor = 'pointer';
                    notice.on('click', e => {
                        window.location.href = "/cart"
                    });


                }

            }
        });
    });


    $('.cart_quantity_down').off('click').on('click', function () {
        var id = $(this).data('id');
        var quantity = $('#quantity_' + id).val();
        if (quantity == 1)
            return;
        else {
            var count = Number(quantity) - 1;
            $('#quantity_' + id).val(count);
            $.ajax({
                url: '/Cart/Edit',
                data: {
                    product_ID: id,
                    quantity: count
                },
                dataType: 'Json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/cart";
                    }
                }
            });
        }
        
    });

    $('.cart_quantity_up').off('click').on('click', function () {
        var id = $(this).data('id');
        var numberOfProduct = $('#numberOfProduct_' + id).text();
        var quantity = $('#quantity_' + id).val();
        var count = (Number(numberOfProduct) != '' && Number(quantity) >= Number(numberOfProduct)) ? numberOfProduct : Number(quantity) + 1;
        $.ajax({
            url: '/Cart/Edit',
            data: {
                product_ID: id,
                quantity: count
            },
            dataType: 'Json',
            type: 'POST',
            success: function (res) {
                if (res.status == true) {
                    window.location.href = "/cart";
                }
            }
        });

    });


    $('.cart_quantity_delete').off('click').on('click', function () {
        $.ajax({
            data: { id: $(this).data('id') },
            url: '/Cart/Delete',
            dataType: 'Json',
            type: 'POST',
            success: function (res) {
                if (res.status == true) {
                    window.location.href = "/cart";
                }
            }
        });
    });

});


            //PNotify.error({
            //    title: 'THÔNG BÁO!!',
            //    text: 'Số lượng sản phẩm không thể vượt quá số lượng hiện có.',
            //    stack: new PNotify.Stack({
            //        dir1: 'down',
            //        dir2: 'right',
            //        firstpos1: 25,
            //        firstpos2: 25,
            //        push: 'top',
            //        maxStrategy: 'close'
            //    })
            //});