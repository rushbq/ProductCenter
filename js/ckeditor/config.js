/**
* @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
* For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    //�����y��
    // config.language = 'en';

    //�I���C��
    //config.uiColor = '#AADC6E';

    //�u����O�_�i�H�Q���Y
    config.toolbarCanCollapse = true;
    //�u�����q�{�O�_�i�}
    config.toolbarStartupExpanded = true;

    //�Ϥ��W�ǥ\��
    //�Y�}�񦹥\��A�|���h�l�ɮת����D (�u���s�W����)
    //config.filebrowserImageUploadUrl = '../CKUpload.ashx?type=Images';

    //�u����w�q���
    //�ۭq����Ҧ�
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
    //�ۭq²���Ҧ�
    config.toolbar = [
    ['Source', '-', 'NewPage'],
    ['Bold', 'Italic', 'Underline', 'Strike'],
    ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
    ['Link', 'Unlink']
    ];
    */
};
