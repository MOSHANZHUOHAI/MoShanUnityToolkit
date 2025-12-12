#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace MoShan.Unity.EditorExpand
{
    using Path   = System.IO.Path;
    using Object = UnityEngine.Object;

    /// <summary>
    /// 编辑器窗口：脚本模板创建器
    /// </summary>
    /// <remarks>
    /// <b>注意：</b>该脚本需要放在 Editor 文件夹下
    /// </remarks>
    public sealed partial class ScriptTemplateCreator
    {
        /// <summary>
        /// 创建脚本结束名称编辑行为
        /// </summary>
        /// <remarks>
        /// 在结束【<see cref="UnityEditor.ProjectBrowser">项目窗口</see>】中的脚本名称编辑后执行
        /// </remarks>
        private sealed class CreateScriptEndNameEditorAction : EndNameEditAction
        {
            #region 静态私有函数
            /// <summary>
            /// 根据模板创建脚本资产
            /// </summary>
            /// <remarks>
            /// 根据模板文件创建新的脚本资源，并导入到项目中
            /// </remarks>
            /// <param name="pathName">路径名称</param>
            /// <param name="resourceFile">资源文件</param>
            /// <returns></returns>
            private static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
            {
                // 获取【完整路径】
                string fullPath = Path.GetFullPath(pathName);

                // 开启【文件读取流】并读取模板文件内容
                StreamReader streamReader = new StreamReader(resourceFile);

                // 读取【所有内容】为【字符串】
                string content = streamReader.ReadToEnd();

                // 关闭【文件读取流】
                streamReader.Close();

                // 获取不包含扩展名的文件名称
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);

                #region 替换【占位符】
                // 占位符：脚本名称
                content = Regex.Replace(content, "#SCRIPTNAME#", fileNameWithoutExtension);

                // 占位符：根名空间开始
                content = Regex.Replace(content, "#ROOTNAMESPACEBEGIN#", string.Empty);

                // 占位符：根名空间结束
                content = Regex.Replace(content, "#ROOTNAMESPACEEND#", string.Empty);

                // 占位符: 不进行格式化
                content = Regex.Replace(content, "#NOTRIM#", string.Empty);
                #endregion

                // 创建【UTF-8 编码】
                UTF8Encoding encoding = new UTF8Encoding(true, false);

                // 是否追加到文件末尾
                bool append = false;

                // 开启【文件写入流】
                StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);

                // 写入【内容】为【文件】
                streamWriter.Write(content);

                // 关闭【文件写入流】
                streamWriter.Close();

                // 根据路径导入资产
                AssetDatabase.ImportAsset(pathName);

                // 根据路径加载资产
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            }
            #endregion

            #region 公开函数
            /// <summary>
            /// 行为 当用户接受编辑后的名称时，调用此函数
            /// </summary>
            /// <param name="instanceId">已编辑资源的实例 ID</param>
            /// <param name="pathName">资源路径</param>
            /// <param name="resourceFile">资源文件，传递给 ProjectWindowUtil 的资源文件字符串参数；如果项目窗口存在，则开始名称编辑</param>
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                // 创建脚本资源并显示该脚本在项目窗口中
                ProjectWindowUtil.ShowCreatedAsset(CreateScriptAssetFromTemplate(pathName, resourceFile));
            }
            #endregion
        }
    }
}
#endif
