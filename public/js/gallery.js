{
    var currentImageInfo = null;

    function showImageViewer(imageInfo) {
        if (typeof imageInfo !== "object") {
            return;
        }
        currentImageInfo = imageInfo;

        var imageViewerImage = document.getElementById("imageViewerImage");
        imageViewerImage.src = imageInfo.url;

        var imageViewer = document.getElementById("imageViewer");
        imageViewer.style.display = "flex";

        var pageContent = document.getElementById("pageContent");
        pageContent.style.display = "none";
    }
    function hideImageViewer() {
        var imageViewer = document.getElementById("imageViewer");
        imageViewer.style.display = "none";

        var pageContent = document.getElementById("pageContent");
        pageContent.style.display = "block";

        if (currentImageInfo !== null) {
            var mainImage = document.getElementById(currentImageInfo.imageElementId);
            mainImage.scrollIntoView();
        }
    }
}
{
    var imageNumberText = null;
    var imageElements = null;

    function updateImageNumberText() {
        if (imageElements === null || imageElements.length <= 0 || imageNumberText === null) {
            return;
        }
        var viewportHeight = window.visualViewport.height;
        var scrollIndex = 0;
        for (let i = 0; i < imageElements.length; i++) {
            const element = imageElements[i];
            var boundingRect = element.getBoundingClientRect();
            var centerY = (boundingRect.y + boundingRect.bottom) / 2;
            if (centerY > viewportHeight) {
                continue;
            }
            scrollIndex = i;
        }
        imageNumberText.innerHTML = String(scrollIndex + 1) + "/" + imageElements.length;
    }

    window.addEventListener("load", function(event) {
        imageElements = [];
        for (let i = 0; i < 999; i++) {
            const element = document.getElementById("image" + i);
            if (element === null) {
                break;
            }
            imageElements.push(element);
        }

        imageNumberText = document.getElementById("imageNumberText");
        updateImageNumberText();

        var thisPostInPostsElement = document.getElementById("thisPostInPostsElement");
        var otherPostsViewport = document.getElementById("otherPostsViewport");
        if (thisPostInPostsElement !== null && otherPostsViewport !== null) {
            var x = thisPostInPostsElement.offsetLeft;
            otherPostsViewport.scrollLeft = x;
        }
    });
    window.addEventListener("scroll", function(event) {
        updateImageNumberText();
    });
}