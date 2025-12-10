;(function() {
  'use strict';


  $(activate);


  function activate() {

    $('.tab-Scroll')
      .scrollingTabs({
        enableSwiping: true
      })
      .on('ready.scrtabs', function() {
        $('.tab-content').show();
      });

  }
}());
