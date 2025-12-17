#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Debug = global::UnityEngine.Debug;

    /// <summary>
    /// 编辑器实用程序：脚本模板
    /// </summary>
    [Serializable]
    public static partial class ScriptTemplateEditorUtility
    {
        #region 常量
        /// <summary>
        /// 脚本模板目录名称
        /// </summary>
        private const string SCRIPT_TEMPLATE_DIRECTORY_NAME = "ScriptTemplates";
        #endregion

        #region 属性
        /// <summary>
        /// 资源脚本模板目录路径
        /// </summary>
        public static string AssetsScriptTemplatesDirectory
        {
            get
            {
                return Path.Combine(Application.dataPath, SCRIPT_TEMPLATE_DIRECTORY_NAME);
            }
        }

        /// <summary>
        /// 包脚本模板目录路径
        /// </summary>
        internal static string PackagesScriptTemplatesDirectory
        {
            get
            {
                // 获取【该类型所在的脚本源文件的路径】
                string scriptFilePath = new StackTrace(true).GetFrame(0).GetFileName();

                // 获取【最后一个斜杠【\Scrpits】的位置索引】
                int lastSlashIndex = scriptFilePath.LastIndexOf("\\Scripts");

                // 判断 <【位置索引】是否为【-1】>，即<是否未获取到正确的位置索引>
                if (lastSlashIndex == -1)
                {
                    return string.Empty;
                }

                // 截取字符串，移除最后一个【\Scrpits】及其后面的内容并进行拼接以获取【包脚本模板路径】
                return Path.Combine(scriptFilePath.Substring(0, lastSlashIndex), SCRIPT_TEMPLATE_DIRECTORY_NAME);
            }
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 获取【脚本模板名称】
        /// </summary>
        /// <remarks>
        /// 格式：{索引}-{菜单名称}__{子级菜单名称（可选）}-{文件名称}.{文件扩展名}
        /// </remarks>
        /// <param name="index">索引</param>
        /// <param name="menuName">菜单名称</param>
        /// <param name="submenuNames">子级菜单名称，可为【空】</param>
        /// <param name="fileName">文件名称，创建该模板为脚本时的默认脚本文件名称</param>
        /// <param name="extension">文件扩展名，创建该模板为脚本时的文件扩展名</param>
        /// <returns>若创建成功，返回由【输入参数】拼接的【脚本模板名称】；否则，返回【空字符串】。</returns>
        public static string GetScriptTemplateName(int index, string menuName, string[] submenuNames, string fileName, string extension)
        {
            // 判断 <【菜单名称】是否为【空】>或<【文件名称】是否为【空】>或<【文件扩展名】是否为【空】>
            if (string.IsNullOrWhiteSpace(menuName) || string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(extension))
            {
                return string.Empty;
            }

            // 判断 <【子级菜单名称】是否为【空】>
            foreach (string submenuName in submenuNames)
            {
                // 判断 <【子级菜单名称】是否为【空】>
                if (string.IsNullOrWhiteSpace(submenuName))
                {
                    return string.Empty;
                }

                menuName = string.Format("{0}__{1}", menuName, submenuName);
            }

            return string.Format("{0}-{1}-{2}.{3}", index, menuName, fileName, extension);
        }
        #endregion

        #region 私有方法

        #region 同步
        /// <summary>
        /// 同步【所有脚本模板】
        /// </summary>
        /// <remarks>
        /// 同步所有预设的脚本模板文件到【Assets/ScriptTemplates】文件夹下
        /// </remarks>
        [MenuItem("Tools/脚本模板/同步 脚本模板文件夹", false, 1)]
        private static void SynchronizeScriptTemplatesFolder()
        {
            // 显示对话框以获取【是否进行同步】
            bool isSynchronize = EditorUtility.DisplayDialog
            (
                "同步脚本模板",
                "同步所有预设的脚本模板文件到【Assets/ScriptTemplates】路径下？",
                "确定",
                "取消"
            );

            // 判断 <是否不同步>
            if (!isSynchronize)
            {
                return;
            }

            #region 获取并检查【源模板文件夹路径】
            // 获取【源模板文件夹路径】
            string sourceScriptTemplatesFolderPath = PackagesScriptTemplatesDirectory;

            // 判断 <【源模板文件夹路径】是否不存在>
            if (!Directory.Exists(sourceScriptTemplatesFolderPath))
            {
                Debug.Log($"源模板文件夹路径不存在。路径：{sourceScriptTemplatesFolderPath}");

                // 显示对话框
                EditorUtility.DisplayDialog
                (
                    "错误",
                    $"源模板文件夹路径不存在，无法进行同步。\n路径：\n{sourceScriptTemplatesFolderPath}",
                    "确定"
                );

                return;
            }
            #endregion

            #region 获取并检查【目标模板文件夹路径】
            // 获取【目标模板文件夹路径】
            string targetScriptTemplatesFolderPath = AssetsScriptTemplatesDirectory;

            // 判断 <【目标模板文件夹路径】是否不存在>
            if (!Directory.Exists(targetScriptTemplatesFolderPath))
            {
                try
                {
                    // 创建【目标模板文件夹路径】文件夹
                    Directory.CreateDirectory(targetScriptTemplatesFolderPath);

                    Debug.Log($"已创建【Assets/ScriptTemplates】文件夹。路径：{targetScriptTemplatesFolderPath}");

                    // 刷新【AssetDatabase】
                    AssetDatabase.Refresh();
                }
                // 捕获异常：其它
                catch (Exception otherException)
                {
                    Debug.LogError($"创建【Assets/ScriptTemplates】文件夹失败！\n {otherException.Message}");

                    return;
                }
            }
            #endregion

            SynchronizeFolder(sourceScriptTemplatesFolderPath, targetScriptTemplatesFolderPath, true);

            // 刷新【AssetDatabase】
            AssetDatabase.Refresh();

            // 显示对话框
            EditorUtility.DisplayDialog
            (
                "同步脚本模板完成",
                "已完成所有预设的脚本模板文件的同步，模板脚本的创建选项将于重启编辑器后同步到项目浏览器窗口的右键菜单。",
                "确定"
            );
        }

        /// <summary>
        /// 同步【文件夹】
        /// </summary>
        /// <param name="sourcePath">源路径</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="isOverwrite">是否覆写</param>
        private static void SynchronizeFolder(string sourcePath, string targetPath, bool isOverwrite = true)
        {
            // 判断 <【源文件夹】是否不存在>
            if (!Directory.Exists(sourcePath))
            {
                Debug.LogError($"源文件夹不存在！路径：{sourcePath}");

                return;
            }

            // 判断 <【目标文件夹】是否不存在>
            if (!Directory.Exists(targetPath))
            {
                // 创建【目标文件夹】
                Directory.CreateDirectory(targetPath);
            }

            // 复制【所有文件】
            CopyAllFiles(sourcePath, targetPath, isOverwrite);

            // 获取【源文件夹】下的【所有子级文件夹】
            string[] subDirectories = Directory.GetDirectories(sourcePath);

            // 遍历循环以处理【所有子级文件夹】
            foreach (string subDirectory in subDirectories)
            {
                // 获取【当前子级目录】的【名称】
                string directoryName = Path.GetFileName(subDirectory);

                // 获取【目标文件夹】下的对应【当前子级目录】的路径
                string targetSubDirectory = Path.Combine(targetPath, directoryName);

                SynchronizeFolder(subDirectory, targetSubDirectory, isOverwrite);
            }
        }

        /// <summary>
        /// 复制【所有文件】
        /// </summary>
        /// <param name="sourceDirectory">源目录</param>
        /// <param name="targetDirectory">目标目录</param>
        /// <param name="isOverwrite">是否覆写</param>
        private static void CopyAllFiles(string sourceDirectory, string targetDirectory, bool isOverwrite)
        {
            // 获取【源文件夹】下的【所有文件】
            string[] files = Directory.GetFiles(sourceDirectory);

            // 遍历循环以复制【所有文件】到目标目录
            foreach (string file in files)
            {
                // 判断 <【当前文件】是否为【.meta】文件>
                if (file.EndsWith(".meta"))
                {
                    continue;
                }

                // 获取【当前文件】的名称
                string fileName = Path.GetFileName(file);

                // 获取【目标文件】的路径
                string destinationFile = Path.Combine(targetDirectory, fileName);

                try
                {
                    // 确保目标目录存在
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));

                    // 复制【当前文件】
                    File.Copy(file, destinationFile, isOverwrite);

                    Debug.Log($"已完成复制：\n{file}\n->\n{destinationFile}");
                }
                // 抛出异常：其它
                catch (Exception otherException)
                {
                    Debug.LogError($"复制文件失败！\n路径：{file}\n {otherException.Message}");
                }
            }
        }
        #endregion

        #region 打开
        /// <summary>
        /// 打开【资源脚本模板文件夹】
        /// </summary>
        [MenuItem("Tools/脚本模板/打开 资源脚本模板文件夹", false, 2)]
        private static void OpenAssetsScriptTemplatesFolder()
        {
            // 获取【文件夹路径】
            string folderPath = AssetsScriptTemplatesDirectory;

            // 判断 <【文件夹路径】是否不存在>
            if (!Directory.Exists(folderPath))
            {
                try
                {
                    // 创建【资源脚本模板文件夹】
                    Directory.CreateDirectory(folderPath);

                    Debug.Log($"已创建【Assets/ScriptTemplates】文件夹。路径：{folderPath}");

                    // 刷新【AssetDatabase】
                    AssetDatabase.Refresh();
                }
                // 捕获异常：其它
                catch (Exception otherException)
                {
                    Debug.LogError($"创建【Assets/ScriptTemplates】文件夹失败！\n {otherException.Message}");

                    return;
                }
            }

            // 打开【资源脚本模板文件夹】
            EditorUtility.RevealInFinder(folderPath);
        }

        /// <summary>
        /// 打开【包脚本模板文件夹】
        /// </summary>
        [MenuItem("Tools/脚本模板/打开 包脚本模板文件夹", false, 3)]
        private static void OpenPackagesScriptTemplatesFolder()
        {
            // 获取【文件夹路径】
            string folderPath = PackagesScriptTemplatesDirectory;

            // 判断 <【文件夹路径】是否不存在>
            if (!Directory.Exists(folderPath))
            {
                Debug.Log($"源模板文件夹路径不存在。路径：{folderPath}");

                return;
            }

            // 打开【包脚本模板文件夹】
            EditorUtility.RevealInFinder(folderPath);
        }
        #endregion

        #endregion
    }
}
#endif
