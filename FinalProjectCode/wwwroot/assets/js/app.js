

$('.slider').slick({
    infinite: true,
    speed: 100,
    slidesToShow: 5,
     slidesToScroll: 5,
    responsive: [
        {
          breakpoint: 1024,
          settings: {
          slidesToShow: 3,
         slidesToScroll: 3,
         infinite: true,
         dots: true
         }
        },
        {
          breakpoint: 600,
          settings: {
          slidesToShow: 2,
         slidesToScroll: 2
         } 
        },
        {
        breakpoint: 480,
         settings: {
          slidesToShow: 2,
           slidesToScroll: 2
          }
        }
      
     ]
    });


    $('.slider-about').slick({
        dots: true,
    infinite: true,
    speed: 300,
    slidesToShow: 3,
     slidesToScroll: 3,
    responsive: [
        {
          breakpoint: 1024,
          settings: {
          slidesToShow: 2,
         slidesToScroll: 2,
         infinite: true,
         dots: true
         }
        },
        {
          breakpoint: 600,
          settings: {
          slidesToShow: 1,
         slidesToScroll: 1
         } 
        },
        {
        breakpoint: 480,
         settings: {
          slidesToShow: 2,
           slidesToScroll: 2
          }
        }
      
     ]
    });


    $('.slider-brands').slick({
        dots: true,
    infinite: true,
    speed: 100,
    slidesToShow: 6,
     slidesToScroll: 6,
    responsive: [
        {
          breakpoint: 1024,
          settings: {
          slidesToShow: 4,
         slidesToScroll: 4,
         infinite: true,
         dots: true
         }
        },
        {
          breakpoint: 600,
          settings: {
          slidesToShow: 2,
         slidesToScroll: 2
         } 
        },
        {
        breakpoint: 480,
         settings: {
          slidesToShow: 2,
           slidesToScroll: 2
          }
        }
   
     ]
    });


    $('.slider-disc').slick(
        {
            dots:false
        }
    );


    const hamburger =document.querySelector(".hamburger");
    const centerNav =document.querySelector(".center-nav");


    hamburger.addEventListener("click", () => {
      hamburger.classList.toggle("active");
      centerNav.classList.toggle("active");
    })


    const catagorySale =document.querySelector(".catagory-sale")
    const refineBy =document.querySelector(".refine-by")

    refineBy.addEventListener("click", () =>{
      catagorySale.classList.toggle("activedd")
    })
    

    $(".jquerry-button").click(function(){
      $(".first-div").slideToggle();
    });




  







