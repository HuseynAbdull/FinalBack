/*    $(document).ready(function () {
        $(document).on('click', '.deleteImg', function (e) {
            e.preventDefault();
            let url = $(this).attr('href');
            let imageid = $(this).data('imageid');
            let productId = $(this).data('productId');
            console.log(url);
            console.log(imageid);
            console.log(productId);
            fetch(url + "?productId=" + productId + "&imageId=" + imageid)
                .then(res => res.text())
                .then(data => {
                    $('.productImage').html(data)
                })
        })
    })
*/


$(document).ready(function () {
    let productId = $('.deleteImg').data('productId'); // move the definition here
    $(document).on('click', '.deleteImg', function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        let imageid = $(this).data('imageid');
        console.log(url);
        console.log(imageid);
        console.log(productId);
        fetch(url + "?productId=" + productId + "&imageId=" + imageid)
            .then(res => res.text())
            .then(data => {
                $('.productImage').html(data)
            })
    })
}) 