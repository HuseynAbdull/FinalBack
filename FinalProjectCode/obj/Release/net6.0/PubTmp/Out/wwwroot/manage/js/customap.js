
$(document).ready(function (e) {
    $(document).on('click', '.deleteImg', function (e) {
        e.preventDefault();
        let imgid = $(this).attr('data-imageId')
        let prodid = $(this).attr('data-prodid')
        console.log(imgid)
        console.log(prodid)

        fetch('/manage/product/DeleteImg?productId=' + prodid + '&imageId=' + imgid)
            .then(res => res.text())
            .then(data => {
                $('.productImages').html(data)
            })
    });
});
