let buttons = document.querySelectorAll(".links button");
    for (let btn of buttons) {
      btn.onclick = function() {
        let active_btn = document.querySelector('.active-description');
        active_btn.classList.remove('active-description');
        this.classList.add("active-description");

        let index = this.getAttribute('id');
        let contents = document.querySelectorAll('.content-box')
        for(let div of contents) {
            if(div.getAttribute('id') == index){
                div.classList.remove('d-none')
            }
            else{
                div.classList.add('d-none')
            }
        }
      }
    };

var searchButtons=document.getElementsByClassName('phone-search-btn')
for(let searchButton of searchButtons){
  searchButton.addEventListener('click', function(){
let searchSections=document.getElementsByClassName('right-header-bottom')
for(let searchSection of searchSections){
  searchSection.classList.toggle('active-for-search')
}
  })
}


$(document).on('click', '.gender-selector-btn', (function () {
    let genderid = this.getAttribute('data-id');
    let producttypeid = this.getAttribute('data-producttype-id');
    fetch("/shop/getfilteredproducts?genderid=" + genderid + "&producttypeid=" + producttypeid)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.sale-full').html(data)
        })

}))

$(document).on('click', '.producttype-selector-btn', (function () {
    let producttypeid = this.getAttribute('data-id');
    let genderid = this.getAttribute('data-gender-id');
    fetch("/shop/getfilteredproducts?producttypeid=" + producttypeid + "&genderid=" + genderid)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.sale-full').html(data)
        })
}))






$(document).on('click', '.pagination-list', (function () {
    let pageindex = this.getAttribute('data-id');
    let producttypeid = this.getAttribute('data-producttype-id');
    let genderid = this.getAttribute('data-gender-id');
    fetch("shop/GetFilteredProducts?pageindex=" + pageindex + "&producttypeid=" + producttypeid + "&genderid=" + genderid)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.sale-full').html(data)
        })

}))


$(document).ready(function (){
    $('#searchInput').keyup(function () {
        let search = $(this).val();
        console.log(search)

        if (search.length >= 3) {
            fetch('shop/search?search=' + search)
                .then(res => {
                    return res.text()
                }).then(data => {
                    $('#searchbody').html(data)
                })
        } else
        {
            $('#searchbody').html('')
        }
     
    })

    $(document).on('click', '.addAdress', function (e) {
        e.preventDefault();
        $('.addressContainer').addClass('d-none');
        $('.addressForm').removeClass('d-none');
    })



    $('.addToBasket').click(function myfunction(e) {
        e.preventDefault();
        let productId = $(this).data('id');
        fetch('basket/AddBasket?id=' + productId)
            .then(res => {
                return res.text();
            }).then(data => {
                console.log(data)
                if (data != null){
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'bottom-start',
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: false,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    })

                    Toast.fire({
                        icon: 'success',
                        title: 'Product Added from Cart',
                        customClass: {
                            container: 'my-sweet-alert'
                        }
                    })
                    fetch('basket/BasketCount')
                        .then(res => {
                            return res.json();
                        })
                        .then(data => {
                            $('.count-basket').text(data);
                        })
                }
            })
   
    })

    $(document).on('click', '.pilus-btn', function () {
        let productId = this.getAttribute('data-id');
        console.log(productId)
        fetch('basket/PlusCount?id=' + productId)
            .then(res => {
                return res.text();
            }).then(data => {
                $('#basket-full').html(data)
            });
    });
    $(document).on('click', '.minus-btn', function () {
        let productId = this.getAttribute('data-id');
        fetch('basket/MinusCount?id=' + productId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('#basket-full').html(data)
            });
            
    });

    $(document).on('click', '.deleteCart', function () {
        const removeId = $(this).attr('data-id');
        fetch('basket/DeleteBasket?id=' + removeId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('#basket-full').html(data);
                fetch('basket/BasketCount')
                    .then(res => {
                        return res.json();
                    })
                    .then(data => {
                        $('.count-basket').text(data);
                    })
            });

    });
    
    $('.wishlist-button').click(function myfunction(e) {
        e.preventDefault();
        let productId = $(this).data('id');
        fetch('/wishlist/AddWishlist?id=' + productId)
            .then(res => {
                return res.text();
            }).then(data => {
                console.log(data)
                if (data != null) {
                    const Toast = Swal.mixin({
                        toast: true,
                        position: 'bottom-start',
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: false,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    })

                    Toast.fire({
                        icon: 'success',
                        title: 'Product Added from Wishlist',
                        customClass: {
                            container: 'my-sweet-alert'
                        }
                    })
                }
            })

    })


    $(document).on('click', '.romovewishlist', function () {
        const removeId = $(this).attr('data-id');
        fetch('/wishlist/DeleteWishlist?id=' + removeId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('#basket-full').html(data);
            });

    });

})

$('.accordion-collapse').on('show.bs.collapse', function () {
    $(this).closest("table")
        .find(".accordion-collapse.show")
        .not(this)
        .collapse('toggle');
})

