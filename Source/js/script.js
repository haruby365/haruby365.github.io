{
    var scrollTopButton = null;

    function updateScrollTopButtonState() {
        if (scrollTopButton === null) {
            return;
        }
        var scrollY = window.scrollY;
        scrollTopButton.disabled = scrollY <= 0;
    }

    function scrollToTopSmooth() {
        window.scrollTo({ top: 0, behavior: "smooth", });
    }

    window.addEventListener("load", function() {
        scrollTopButton = document.getElementById("scrollTopButton")
        if (null !== scrollTopButton) {
            scrollTopButton.addEventListener("click", scrollToTopSmooth);
        }
        updateScrollTopButtonState();
    });
    window.addEventListener("scroll", updateScrollTopButtonState);
}