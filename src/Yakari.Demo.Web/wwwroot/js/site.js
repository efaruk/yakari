// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
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