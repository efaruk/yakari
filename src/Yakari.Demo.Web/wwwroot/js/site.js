// Write your Javascript code.

var generateCacheItem = function (e) {
    $.ajax({
        url: '/api/cache/generate',
        type: 'GET',
    });
    return true;
};