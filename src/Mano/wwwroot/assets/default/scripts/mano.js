$(document).ready(function () {
    DropEnable();
});

function DropEnable() {
    $('.markdown-editor').unbind().each(function () {
        $(this).dragDropOrPaste();
    });
}