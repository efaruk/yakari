// Write your Javascript code.

var generateCacheItem = function () {
    $.ajax({
        url: '/api/cache/',
        type: 'POST',
        async:false,
        success: location.reload(),
        error: location.reload()
    });
    return true;
};

var removeCacheItem = function (e) {
    $.ajax({
        url: '/api/cache/' + e,
        type: 'DELETE',
        async:false,
        success: location.reload(),
        error: location.reload()
    });
    return true;
};