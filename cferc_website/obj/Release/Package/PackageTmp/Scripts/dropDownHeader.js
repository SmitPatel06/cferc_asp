$(document).ready(function () {

    

    $('.headerMenu > li').bind('mouseover', openSubMenu);

    function openSubMenu() {

        $(this).find('ul').css('visibility', 'visible');
    };

    $('.headerMenu > li').bind('mouseout', closeSubMenu);

    function closeSubMenu() {

        $(this).find('ul').css('visibility', 'hidden');
    };



});