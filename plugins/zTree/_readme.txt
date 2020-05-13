[ref]
https://github.com/zTree/zTree_v3
�@�̺����w��.�u��git


[json���榡]
{
 id,
 parentID,
 label,
 selected,
 open
}


����:
 �������w�g�L�ק�,




-----------------------------------------------------------
[�ϥνd��]

  <link rel="stylesheet" href="<%=fn_Param.WebUrl %>plugins/zTree/css/zTreeStyle.css" />
        <script src="<%=fn_Param.WebUrl %>plugins/zTree/jquery.ztree.core-3.5.min.js"></script>
        <script src="<%=fn_Param.WebUrl %>plugins/zTree/jquery.ztree.excheck-3.5.min.js"></script>
        <script>
            //--- zTree �]�w Start ---
            var setting = {
                view: {
                    dblClickExpand: false
                },
                callback: {
                    onClick: MMonClick
                },
                check: {
                    enable: true
                },
                data: {
                    simpleData: {
                        enable: true
                    }
                }
            };

            //Event - onClick
            function MMonClick(e, treeId, treeNode) {
                var zTree = $.fn.zTree.getZTreeObj("authList");
                zTree.expandNode(treeNode);
            }
            //--- zTree �]�w End ---
        </script>
        <script>
            $(function () {
                /*
                    ���o�v��List
                    ref:http://api.jquery.com/jQuery.post/
                */
                var jqxhr = $.post("<%=fn_Param.WebUrl%>Ajax_Data/GetAuthList.ashx", {
                    id: '<%=Param_thisID %>',
                    db: '<%=Param_dbID %>'
                })
                  .done(function (data) {
                      //���J���
                      $.fn.zTree.init($("#authList"), setting, data)
                  })
                  .fail(function () {
                      alert("�v�������J����");
                  });


                /*
                    ���o�w�Ŀ諸����ID
                */
                $("#setAuth").on("click", function () {
                    var myTreeName = "authList";
                    var valAry = [];

                    //�ŧitree����
                    var treeObj = $.fn.zTree.getZTreeObj(myTreeName);

                    //���o�`�Iarray
                    var nodes = treeObj.getCheckedNodes(true);

                    //�Nid��J�}�C
                    for (var row = 0; row < nodes.length; row++) {
                        valAry.push(nodes[row].id);
                    }

                    //�N�}�C�զ��H','���j���r��A�ö�J���
                    $("#MainContent_tb_Values").val(valAry.join(","));

                    //Ĳ�o�]�w click
                    $("#MainContent_btn_Setting").trigger("click");

                });
            });
        </script>