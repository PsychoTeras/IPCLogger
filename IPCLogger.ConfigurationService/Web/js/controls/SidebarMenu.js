$.sidebarMenu = function (menu) {
    var subMenuSelector = '.sidebar-submenu';

    var $menu = $(menu);
    $menu.on('click', 'li a', function (e) {
        var $this = $(this);
        var checkElement = $this.next();

        if (checkElement.is(subMenuSelector) && checkElement.is(':visible')) {
            checkElement.removeClass('menu-open');
            checkElement.parent("li").removeClass("active");
        }

        else if (checkElement.is(subMenuSelector) && !checkElement.is(':visible')) {
            var parent = $this.parents('ul').first();
            var ul = parent.find('ul:visible');
            ul.removeClass('menu-open');
            var parent_li = $this.parent("li");

            checkElement.addClass('menu-open');
            parent.find('li.active').removeClass('active');
            parent_li.addClass('active');
        }
        if (checkElement.is(subMenuSelector)) {
            e.preventDefault();
        }

        $menu.trigger("resized");
    });

    return $menu;
};
