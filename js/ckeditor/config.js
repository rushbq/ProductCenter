/**
* @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
* For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    //介面語言
    // config.language = 'en';

    //背景顏色
    //config.uiColor = '#AADC6E';

    //工具欄是否可以被收縮
    config.toolbarCanCollapse = true;
    //工具欄默認是否展開
    config.toolbarStartupExpanded = true;

    //圖片上傳功能
    //若開放此功能，會有多餘檔案的問題 (只有新增機制)
    //config.filebrowserImageUploadUrl = '../CKUpload.ashx?type=Images';

    //工具欄定義欄位
    //自訂完整模式
    config.toolbar = [
        ['Source', '-', 'NewPage', 'Preview', '-', 'Templates'],
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'],
        ['Find', 'Replace', '-', 'SelectAll'],
        ['Link', 'Unlink', 'Anchor'],
        '/',
        ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript', '-', 'RemoveFormat'],
        ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
        ['Image', 'Table', 'HorizontalRule', 'SpecialChar', 'PageBreak', 'Flash', 'youtube'],
        '/',
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor'],
        ['Maximize', 'ShowBlocks']
    ];
    /*
    //自訂簡易模式
    config.toolbar = [
    ['Source', '-', 'NewPage'],
    ['Bold', 'Italic', 'Underline', 'Strike'],
    ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
    ['Link', 'Unlink']
    ];
    */
};
