#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    /// <summary>
    /// 编辑器窗口：脚本模板创建器
    /// </summary>
    /// <remarks>
    /// <b>注意：</b>该脚本需要放在 Editor 文件夹下
    /// </remarks>
    public sealed partial class ScriptTemplateCreator : EditorWindow
    {
        /// <summary>
        /// 显示窗口
        /// </summary>
        // [MenuItem("Tools/脚本/脚本模板创建器", false, 0)]
        private static void ShowWindow()
        {
            // 获取【窗口】
            ScriptTemplateCreator window = EditorWindow.GetWindow<ScriptTemplateCreator>(false, "脚本模板创建器", true);

            // 设置【尺寸】
            float width = 450f;
            float height = 230f;

            // 获取【当前监视器的分辨率】以设置窗口到【屏幕中心】
            window.position = new Rect(Screen.currentResolution.width / 2 - width / 2, Screen.currentResolution.height / 2 - height / 2, width, height);

            // 设置【最小尺寸】
            window.minSize = new Vector2(width, height);

            window.titleContent.image = EditorGUIUtility.IconContent("cs Script Icon").image;

            // 绘制 窗口
            window.Show();

            // 聚焦窗口
            window.Focus();
        }

        #region 静态私有函数
        /// <summary>
        /// 创建【模板脚本菜单】
        /// </summary>
        [MenuItem("Assets/模板脚本", false, 0)]
        private static void CreateTemplateScriptMenu()
        {
            GetTemplateScriptMenu().DropDown(new Rect(0, 0, 0, 0));
        }

        /// <summary>
        /// 获取【模板脚本菜单】
        /// </summary>
        /// <returns>返回创建的模板脚本菜单</returns>
        private static GenericMenu GetTemplateScriptMenu()
        {
            GenericMenu menu = new GenericMenu();

            string[] DisplayNames = new string[]
            {
                "C#/引用类型",
                "C#/值类型",
                "C#/接口",
                "C#/枚举",
                "C#/标记枚举",

                "UnityEngine/MonoBehaviour",
                "UnityEngine/ScriptableObject",
                "UnityEngine/Attribute",

                "UnityEditor/编辑器窗口",
                "UnityEditor/检视器窗口",
                "UnityEditor/属性绘制器",
                "UnityEditor/特性绘制器",
            };

            string[] fileNames = new string[]
            {
                "Class",
                "Struct",
                "Interface",
                "Enum",
                "EnumFlags",

                "MonoBehaviour",
                "ScriptableObject",
                "Attribute",

                "EditorWindow",
                "EditorDrawer",
                "PropertyDrawer",
                "AttributeDrawer",
            };

            // 获取【模板文件夹路径】
            string templateFolderPath = GetTemplateFolderPath();

            string[] filePaths = new string[]
            {
                #region CSharp
                templateFolderPath + "/" + GetTemplateFileName
                (
                    0,
                    GetMenuName("CSharp", "Class"),
                    GetDefaultFileName(fileNames[0])
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    1,
                    GetMenuName("CSharp", "Struct"),
                    GetDefaultFileName(fileNames[1])
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    2,
                    GetMenuName("CSharp", "Interface"),
                    GetDefaultFileName(fileNames[3])
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    3,
                    GetMenuName("CSharp", "Enum"),
                    GetDefaultFileName("Enum")
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    4,
                    GetMenuName("CSharp", "EnumFlags"),
                    GetDefaultFileName("EnumFlags")
                ),
                #endregion

                #region Unity
                templateFolderPath + "/" + GetTemplateFileName
                (
                    11,
                    GetMenuName("Unity", "Engine", "MonoBehaviour"),
                    GetDefaultFileName("MonoBehaviour")
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    12,
                    GetMenuName("Unity", "Engine", "ScriptableObject"),
                    GetDefaultFileName("ScriptableObject")
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    13,
                    GetMenuName("Unity", "Engine", "Attribute"),
                    GetDefaultFileName("Attribute")
                ),

                templateFolderPath + "/" + GetTemplateFileName
                (
                    21,
                    GetMenuName("Unity", "Editor", "EditorWindow"),
                    GetDefaultFileName("EditorWindow")
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    22,
                    GetMenuName("Unity", "Editor", "EditorDrawer"),
                    GetDefaultFileName("EditorDrawer")
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    23,
                    GetMenuName("Unity", "Editor", "PropertyDrawer"),
                    GetDefaultFileName("PropertyDrawer")
                ),
                templateFolderPath + "/" + GetTemplateFileName
                (
                    24,
                    GetMenuName("Unity", "Editor", "AttributeDrawer"),
                    GetDefaultFileName("AttributeDrawer")
                ),
                #endregion
            };

            // 循环以添加菜单项
            for (int i = 0; i < fileNames.Length; i++)
            {
                int index = i;

                menu.AddItem
                (
                    new GUIContent(DisplayNames[i]),
                    false,
                    () =>
                    {
                        CreateScript
                        (
                            $"New{fileNames[index]}",
                            filePaths[index]
                        );
                    }
                );
            }

            return menu;
        }

        /// <summary>
        /// 创建【脚本】
        /// </summary>
        /// <param name="fileName">资源文件名称</param>
        /// <param name="filePath">资源文件路径</param>
        private static void CreateScript(string fileName, string filePath)
        {
            // 如果项目窗口已打开，则开始编辑名称，使用指定的脚本模板文件创建新的脚本文件
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists
            (
                // 实例编号
                0,
                // 创建一个新的 CreateScriptEndNameEditorAction 实例，用于处理结束名称编辑回调
                ScriptableObject.CreateInstance<CreateScriptEndNameEditorAction>(),
                // 获取选择的路径或者默认路径并拼接新脚本文件名
                GetSelectedPathOrFallback() + $"/{fileName}.cs",
                // 获取指定图标
                (Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image,
                // 资源文件路径
                filePath
            );
        }

        /// <summary>
        /// 获取【模板文件夹路径】
        /// </summary>
        /// <returns>若获取成功，返回文件夹路径；否则，返回【空字符串】</returns>
        private static string GetTemplateFolderPath()
        {
            // 获取类型所在的源文件路径
            string sourceFilePath = new StackTrace(true).GetFrame(0).GetFileName();

            // 获取【最后一个 '/' 的位置】
            int lastSlashIndex = sourceFilePath.LastIndexOf("\\Core");

            // 判断 <是否未获取到正确的位置>
            if (lastSlashIndex == -1)
            {
                return string.Empty;
            }

            // 截取字符串，去掉最后一个 '\Core' 及其后面的内容
            sourceFilePath = sourceFilePath.Substring(0, lastSlashIndex);

            return sourceFilePath + "/Templates";
        }

        /// <summary>
        /// 获取【当前选择路径】或【默认路径】
        /// </summary>
        /// <returns>若获取成功，则返回【当前选择路径】；否则，返回【默认路径】</returns>
        private static string GetSelectedPathOrFallback()
        {
            // 设置默认路径为根目录
            string path = "Assets";

            // 遍历循环当前选择的所有对象
            foreach (global::UnityEngine.Object @object in Selection.GetFiltered(typeof(global::UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(@object);

                // 判断 <路径是否为【空】>、<是否指向文件而非文件夹>
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    // 获取文件所在的文件夹路径
                    path = global::System.IO.Path.GetDirectoryName(path);

                    break;
                }
            }
            return path;
        }

        /// <summary>
        /// 获取【模板文件名称】
        /// </summary>
        /// <param name="index">序号，数字越小优先级越高</param>
        /// <param name="menuName">菜单名称</param>
        /// <param name="defaultFileName"></param>
        /// <returns>若获取成功，则返回文件名称；否则，返回【空字符串】</returns>
        private static string GetTemplateFileName(int index, string menuName, string defaultFileName)
        {
            /*
             * 文件名称 = 序号-菜单名称-默认文件名称.txt
             * 
             * 要求：
             * 1、序号数字越小优先级越高
             * 2、序号、菜单名称、默认文件名称之间使用减号【-】进行分隔
             * 3、必须为文本文件【.txt】
             */

            // 判断 <【菜单名称】或【默认文件名称】是否为【空】>
            if (string.IsNullOrWhiteSpace(menuName) || string.IsNullOrWhiteSpace(defaultFileName))
            {
                return string.Empty;
            }

            return string.Format("{0}-{1}-{2}.txt", index.ToString(), menuName, defaultFileName);
        }

        /// <summary>
        /// 获取【菜单名称】
        /// </summary>
        /// <remarks>
        /// 显示在【项目窗口】的菜单中的名称
        /// </remarks>
        /// <param name="menuName">菜单名称</param>
        /// <param name="subMenuNames">子级菜单名称数组</param>
        /// <returns>若获取成功，则返回菜单名称；否则，返回【空字符串】</returns>
        private static string GetMenuName(string menuName, params string[] subMenuNames)
        {
            /*
             * 菜单名称 = 菜单名称_子级菜单名称_子级菜单名称_……
             * 
             * 要求：
             * 1、菜单名称必须添加，子级菜单名称非必须添加
             * 2、菜单名称与子级菜单名称之间使用双下划线【__】进行分隔
             */

            // 判断 <【输入菜单名称】是否为【空】>
            if (string.IsNullOrWhiteSpace(menuName))
            {
                return string.Empty;
            }

            // 使用正则表达式匹配【所有非空格的空白字符】替换为【空字符串】
            string result = Regex.Replace(menuName, @"[\s-[ ]]", string.Empty);

            // 判断 <【返回值】是否为【空】>
            if (string.IsNullOrWhiteSpace(result))
            {
                return string.Empty;
            }

            // 判断 <【子级菜单名称数组】是否为【空】>
            if (subMenuNames == null || subMenuNames.Length == 0)
            {
                return result;
            }

            // 获取【子级菜单名称】
            string subMenuName;

            // 循环以累加子级菜单名称到返回值
            for (int i = 0; i < subMenuNames.Length; i++)
            {
                // 判断 <【当前子级菜单名称】是否为【空】>
                if (string.IsNullOrWhiteSpace(subMenuNames[i]))
                {
                    continue;
                }

                // 使用正则表达式匹配【所有非空格的空白字符】替换为【空字符串】
                subMenuName = Regex.Replace(subMenuNames[i], @"[\s-[ ]]", string.Empty);

                // 判断 <【当前子级菜单名称】是否为【空】>
                if (string.IsNullOrWhiteSpace(subMenuName))
                {
                    continue;
                }

                result = string.Format("{0}__{1}", result, subMenuName);
            }

            return result;
        }

        /// <summary>
        /// 获取【默认文件名称】
        /// </summary>
        /// <remarks>
        /// 创建脚本文件时使用的默认文件名称
        /// </remarks>
        /// <param name="defaultFileName">默认文件名称</param>
        /// <returns>若获取成功，则返回默认文件名称；否则，返回【空字符串】</returns>
        private static string GetDefaultFileName(string defaultFileName)
        {
            /*
             * 默认文件名称 = 默认文件名称.扩展名
             * 
             * 要求：
             * 1、必须添加文件扩展名
             */

            // 判断 <【输入默认文件名称】是否为【空】>
            if (string.IsNullOrWhiteSpace(defaultFileName))
            {
                return string.Empty;
            }

            // 使用正则表达式匹配【所有非空格的空白字符】替换为【空字符串】
            string result = Regex.Replace(defaultFileName, @"[\s-[ ]]", string.Empty);

            // 判断 <【返回值】是否为【空】>
            if (string.IsNullOrWhiteSpace(result))
            {
                return string.Empty;
            }

            return string.Format("{0}.cs", result);
        }
        #endregion

        #region 生命周期函数
        private void OnGUI()
        {
            GUILayout.Label("基础脚本");

            GUILayout.Label("引擎脚本");

            GUILayout.Label("编辑器脚本");
        }
        #endregion
    }
}
#endif
