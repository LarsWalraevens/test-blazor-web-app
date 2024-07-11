window.interop = {
    getBoundingClientRect: function (element) {
        if (element) {
            const rect = element.getBoundingClientRect();
            return {
                left: rect.left,
                top: rect.top,
                right: rect.right,
                bottom: rect.bottom,
                width: rect.width,
                height: rect.height
            };
        }
        return null;
    }
};
