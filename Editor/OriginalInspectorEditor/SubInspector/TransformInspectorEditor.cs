#if UNITY_EDITOR
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Vector3    = global::UnityEngine.Vector3;
    using Quaternion = global::UnityEngine.Quaternion;
    using Transform  = global::UnityEngine.Transform;

    /// <summary>
    /// 检视窗口编辑器：【<see cref="UnityEngine.Transform">变换</see>】
    /// </summary>
    /// <remarks>
    /// <para>用于自定义【<see cref="UnityEngine.Transform">变换组件</see>】在【<see cref="global::UnityEditor.InspectorWindow">检视窗口</see>】的显示。</para>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件应放在【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Transform))]
    internal sealed class TransformInspectorEditor : OriginalComponentInspectorEditor<Transform>
    {
        #region 常量
        /// <summary>
        /// 编辑器首选项键：是否显示扩展功能
        /// </summary>
        private const string IS_DISPLAY_EXTENSION_FUNCTION_EDITOR_PREFS_KEY = nameof(TransformInspectorEditor) + "." + nameof(m_IsDisplayExtensionFunction);
        #endregion

        #region 静态字段
        /// <summary>
        /// 位置复制按钮内容
        /// </summary>
        private static readonly GUIContent s_PositionCopyButtonContent;

        /// <summary>
        /// 旋转复制按钮内容
        /// </summary>
        private static readonly GUIContent s_RotationCopyButtonContent;

        /// <summary>
        /// 缩放复制按钮内容
        /// </summary>
        private static readonly GUIContent s_ScaleCopyButtonContent;

        /// <summary>
        /// 所有属性复制按钮内容
        /// </summary>
        private static readonly GUIContent s_AllCopyButtonContent;

        /// <summary>
        /// 位置粘贴按钮内容
        /// </summary>
        private static readonly GUIContent s_PositionPasteButtonContent;

        /// <summary>
        /// 旋转粘贴按钮内容
        /// </summary>
        private static readonly GUIContent s_RotationPasteButtonContent;

        /// <summary>
        /// 缩放粘贴按钮内容
        /// </summary>
        private static readonly GUIContent s_ScalePasteButtonContent;

        /// <summary>
        /// 所有属性粘贴按钮内容
        /// </summary>
        private static readonly GUIContent s_AllPasteButtonContent;

        /// <summary>
        /// 位置重置按钮内容
        /// </summary>
        private static readonly GUIContent s_PositionResetButtonContent;

        /// <summary>
        /// 旋转重置按钮内容
        /// </summary>
        private static readonly GUIContent s_RotationResetButtonContent;

        /// <summary>
        /// 缩放重置按钮内容
        /// </summary>
        private static readonly GUIContent s_ScaleResetButtonContent;

        /// <summary>
        /// 所有属性重置按钮内容
        /// </summary>
        private static readonly GUIContent s_AllResetButtonContent;
        #endregion

        #region 静态构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        static TransformInspectorEditor()
        {
            // 复制
            s_PositionCopyButtonContent = new GUIContent("复制位置", "将该变换的相对位置信息复制到剪贴板。");
            s_RotationCopyButtonContent = new GUIContent("复制旋转", "将该变换的相对旋转信息复制到剪贴板。");
            s_ScaleCopyButtonContent    = new GUIContent("复制缩放", "将该变换的相对缩放信息复制到剪贴板。");
            s_AllCopyButtonContent      = new GUIContent("复制所有属性", "将该变换的所有属性信息复制到剪贴板。");

            // 粘贴
            s_PositionPasteButtonContent = new GUIContent("粘贴位置", "将剪贴板存储的相对位置信息粘贴到该变换。");
            s_RotationPasteButtonContent = new GUIContent("粘贴旋转", "将剪贴板存储的相对旋转信息粘贴到该变换。");
            s_ScalePasteButtonContent    = new GUIContent("粘贴缩放", "将剪贴板存储的相对缩放信息粘贴到该变换。");
            s_AllPasteButtonContent      = new GUIContent("粘贴所有属性", "将剪贴板存储的所有属性信息粘贴到该变换。");

            // 重置
            s_PositionResetButtonContent = new GUIContent("重置位置", "将该变换的相对位置重置为(0, 0, 0)。");
            s_RotationResetButtonContent = new GUIContent("重置旋转", "将该变换的相对缩放重置为(0, 0, 0)。\n（欧拉角(0, 0, 0) = 四元数(0, 0, 0, 1)）");
            s_ScaleResetButtonContent    = new GUIContent("重置缩放", "将该变换的相对缩放重置为(1, 1, 1)。");
            s_AllResetButtonContent      = new GUIContent
            (
                "重置所有属性",
                "将该变换的所有属性重置。" + "\n将该变换的相对位置重置为(0, 0, 0)。" + "\n将该变换的相对旋转重置为(0, 0, 0)。" + "\n将该变换的相对缩放重置为(1, 1, 1)。"
            );
        }
        #endregion

        #region 静态私有方法
        /// <summary>
        /// 绘制【复制按钮】
        /// </summary>
        /// <param name="copyContent">复制内容</param>
        /// <param name="buttonContent">按钮内容</param>
        private static void DrawCopyButton(Vector3 copyContent, GUIContent buttonContent)
        {
            // 绘制【复制】按钮，并判断 <该按钮是否被触发>
            if (GUILayout.Button(buttonContent))
            {
                // 添加到剪贴板
                GUIUtility.systemCopyBuffer = GetVector3String(copyContent); ;
            }
        }

        /// <summary>
        /// 绘制【重置按钮】
        /// </summary>
        /// <param name="currentValue">当前值</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="buttonContent">按钮内容</param>
        /// <param name="result">返回值</param>
        /// <returns>返回是否触发重置按钮的判断结果</returns>
        private static bool DrawResetButton(Vector3 currentValue, Vector3 defaultValue, out Vector3 result, GUIContent buttonContent)
        {
            // 判断 <【当前值】是否不等于【默认值】>，即<【当前值】是否已发生变更>
            if (!currentValue.Equals(defaultValue))
            {
                // 绘制【重置】按钮，并判断 <该按钮是否被触发>
                if (GUILayout.Button(buttonContent))
                {
                    result = defaultValue;

                    return true;
                }
            }
            else
            {
                // 禁用 GUI 交互
                GUI.enabled = false;

                GUILayout.Button(buttonContent);

                // 启用 GUI 交互
                GUI.enabled = true;
            }

            result = currentValue;

            return false;
        }

        /// <summary>
        /// 获取【三维向量字符串】
        /// </summary>
        /// <param name="value">需要转换为字符串的三维向量值</param>
        /// <returns>获取输入三维向量的字符串值</returns>
        private static string GetVector3String(Vector3 value)
        {
            // 格式：Vector3(x,y,z)
            return $"Vector3({value.x:F2},{value.y:F2},{value.z:F2})";
        }

        /// <summary>
        /// 尝试解析【三维向量】
        /// </summary>
        /// <param name="content">需要进行解析的内容</param>
        /// <param name="result">解析后返回的内容</param>
        /// <returns>返回尝试解析是否成功的判断结果</returns>
        private static bool TryParseVector3(string content, out Vector3 result)
        {
            result = Vector3.zero;

            // 使用正则表达式匹配【x, y, z】的字符串
            Match match = Regex.Match(content, @"^Vector3\(\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)\s*\)$");

            // 判断 <正则表达式是否未匹配成功>
            if (!match.Success)
            {
                // 解析失败
                return false;
            }

            // 提取匹配的组
            string xString = match.Groups[1].Value; // 第一个数字
            string yString = match.Groups[3].Value; // 第二个数字
            string zString = match.Groups[5].Value; // 第三个数字

            // 判断 <每个部分的解析尝试是否成功>
            if (float.TryParse(xString, NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(yString, NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(zString, NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
            {
                result = new Vector3(x, y, z);

                // 解析成功
                return true;
            }

            // 解析失败
            return false;
        }

        /// <summary>
        /// 尝试解析【三维向量】
        /// </summary>
        /// <param name="xString">需要进行解析的 X 轴内容</param>
        /// <param name="yString">需要进行解析的 Y 轴内容</param>
        /// <param name="zString">需要进行解析的 Z 轴内容</param>
        /// <param name="result">解析后返回的内容</param>
        /// <returns>返回尝试解析是否成功的判断结果</returns>
        private static bool TryParseVector3(string xString, string yString, string zString, out Vector3 result)
        {
            result = Vector3.zero;

            // 判断 <每个部分的解析尝试是否成功>
            if (float.TryParse(xString, NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(yString, NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(zString, NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
            {
                result = new Vector3(x, y, z);

                // 解析成功
                return true;
            }

            // 解析失败
            return false;
        }

        /// <summary>
        /// 尝试解析【所有三维向量】
        /// </summary>
        /// <param name="content">需要进行解析的内容</param>
        /// <param name="position">解析后返回的位置</param>
        /// <param name="rotation">解析后返回的旋转</param>
        /// <param name="scale">解析后返回的缩放</param>
        /// <returns>返回尝试解析是否成功的判断结果</returns>
        private static bool TryParseVector3s(string content, out Vector3 position, out Vector3 rotation, out Vector3 scale)
        {
            position = Vector3.zero;
            rotation = Vector3.zero;
            scale    = Vector3.zero;

            // 使用正则表达式匹配【x, y, z】的字符串
            Match match = Regex.Match(content, @"^Vector3\(\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)\s*\),Vector3\(\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)\s*\),Vector3\(\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)\s*\)$");

            // 判断 <正则表达式是否未匹配成功>
            if (!match.Success)
            {
                // 解析失败
                return false;
            }

            // 提取匹配的组
            string xString = match.Groups[1].Value; // 第一个数字
            string yString = match.Groups[3].Value; // 第二个数字
            string zString = match.Groups[5].Value; // 第三个数字

            // 判断 <尝试解析【位置】是否失败>
            if (!TryParseVector3(xString, yString, zString, out position))
            {
                position = Vector3.zero;

                // 解析失败
                return false;
            }

            // 提取匹配的组
            xString = match.Groups[7].Value;  // 第一个数字
            yString = match.Groups[9].Value;  // 第二个数字
            zString = match.Groups[11].Value; // 第三个数字

            // 判断 <尝试解析【旋转】是否失败>
            if (!TryParseVector3(xString, yString, zString, out rotation))
            {
                position = Vector3.zero;

                // 解析失败
                return false;
            }

            // 提取匹配的组
            xString = match.Groups[13].Value; // 第一个数字
            yString = match.Groups[15].Value; // 第二个数字
            zString = match.Groups[17].Value; // 第三个数字

            // 判断 <尝试解析【缩放】是否失败>
            if (!TryParseVector3(xString, yString, zString, out scale))
            {
                position = Vector3.zero;
                rotation = Vector3.zero;

                // 解析失败
                return false;
            }

            // 解析成功
            return true;
        }
        #endregion

        #region 字段
        /// <summary>
        /// 是否显示扩展功能
        /// </summary>
        private bool m_IsDisplayExtensionFunction;

        #region 序列化属性
        /// <summary>
        /// 序列化属性：相对坐标
        /// </summary>
        private SerializedProperty m_LocalPositionProperty;

        /// <summary>
        /// 序列化属性：相对旋转
        /// </summary>
        private SerializedProperty m_LocalRotationProperty;

        /// <summary>
        /// 序列化属性：相对缩放
        /// </summary>
        private SerializedProperty m_LocalScaleProperty;
        #endregion

        #endregion

        #region 属性
        /// <inheritdoc/>
        protected override string OriginalComponentEditorTypeFullName
        {
            get
            {
                return "UnityEditor.TransformInspector";
            }
        }
        #endregion

        #region 私有方法
        /// <inheritdoc/>
        protected override void GetSerializedProperties(SerializedObject serializedObject)
        {
            // 获取序列化属性：相对坐标
            m_LocalPositionProperty = serializedObject.FindProperty("m_LocalPosition");

            // 获取序列化属性：相对旋转
            m_LocalRotationProperty = serializedObject.FindProperty("m_LocalRotation");

            // 获取序列化属性：相对缩放
            m_LocalScaleProperty = serializedObject.FindProperty("m_LocalScale");
        }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            // 从【编辑器首选项】中读取【是否显示扩展功能】
            m_IsDisplayExtensionFunction = EditorPrefs.GetBool(IS_DISPLAY_EXTENSION_FUNCTION_EDITOR_PREFS_KEY);
        }

        /// <inheritdoc/>
        protected override void OnDraw()
        {
            // 绘制间隔
            GUILayout.Space(10);

            // 绘制【折页】
            m_IsDisplayExtensionFunction = EditorGUILayout.BeginFoldoutHeaderGroup(m_IsDisplayExtensionFunction, "拓展功能");

            // 判断 <是否显示扩展功能>
            if (!m_IsDisplayExtensionFunction)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();

                return;
            }

            DrawCopyButton();

            DrawPasteButton();

            DrawResetButton();

            DrawAllPropertyCopyButton();

            DrawAllPropertyPasteButton();

            DrawAllPropertyResetButton();

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        /// <inheritdoc/>
        protected override void OnExit()
        {
            // 将【是否显示扩展功能】存入【编辑器首选项】
            EditorPrefs.SetBool(IS_DISPLAY_EXTENSION_FUNCTION_EDITOR_PREFS_KEY, m_IsDisplayExtensionFunction);
        }

        /// <summary>
        /// 绘制【复制按钮】
        /// </summary>
        private void DrawCopyButton()
        {
            // 判断 <检查器窗口所示的【游戏对象】是否为【空】>
            if (Selection.activeGameObject == null)
            {
                return;
            }

            // 开始水平布局
            GUILayout.BeginHorizontal();

            DrawCopyButton(m_LocalPositionProperty.vector3Value, s_PositionCopyButtonContent);

            DrawCopyButton(m_LocalRotationProperty.quaternionValue.eulerAngles, s_RotationCopyButtonContent);

            DrawCopyButton(m_LocalScaleProperty.vector3Value, s_ScaleCopyButtonContent);

            // 结束水平布局
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制【粘贴按钮】
        /// </summary>
        private void DrawPasteButton()
        {
            // 判断 <检查器窗口所示的【游戏对象】是否为【空】>
            if (Selection.activeGameObject == null)
            {
                return;
            }

            string content = GUIUtility.systemCopyBuffer;

            // 判断 <尝试解析【剪贴板内容】是否成功>，即<内容格式是否为：Vector3(x,y,z)>
            bool isCanParse = TryParseVector3(GUIUtility.systemCopyBuffer, out Vector3 result);

            // 开始水平布局
            GUILayout.BeginHorizontal();

            // 判断 <转换是否失败>
            if (!isCanParse)
            {
                // 禁用 GUI 交互
                GUI.enabled = false;

                // 绘制【粘贴位置】按钮
                GUILayout.Button(s_PositionPasteButtonContent);

                // 绘制【粘贴旋转】按钮
                GUILayout.Button(s_RotationPasteButtonContent);

                // 绘制【粘贴缩放】按钮
                GUILayout.Button(s_ScalePasteButtonContent);

                // 启用 GUI 交互
                GUI.enabled = true;

                // 结束水平布局
                GUILayout.EndHorizontal();

                return;
            }

            // 绘制【粘贴位置】按钮，并判断 <该按钮是否被触发>
            if (GUILayout.Button(s_PositionPasteButtonContent))
            {
                m_LocalPositionProperty.vector3Value = result;

                ApplyModifiedProperties();
            }

            // 绘制【粘贴旋转】按钮，并判断 <该按钮是否被触发>
            if (GUILayout.Button(s_RotationPasteButtonContent))
            {
                m_LocalRotationProperty.quaternionValue = Quaternion.Euler(result);

                ApplyModifiedProperties();
            }

            // 绘制【粘贴缩放】按钮，并判断 <该按钮是否被触发>
            if (GUILayout.Button(s_ScalePasteButtonContent))
            {
                m_LocalScaleProperty.vector3Value = result;

                ApplyModifiedProperties();
            }

            // 结束水平布局
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制【重置按钮】
        /// </summary>
        private void DrawResetButton()
        {
            // 开始水平布局
            GUILayout.BeginHorizontal();

            // 判断 <是否触发【位置重置】按钮>
            if (DrawResetButton(m_LocalPositionProperty.vector3Value, Vector3.zero, out Vector3 localPosition, s_PositionResetButtonContent))
            {
                m_LocalPositionProperty.vector3Value = localPosition;

                ApplyModifiedProperties();
            }

            // 判断 <是否触发【旋转重置】按钮>
            if (DrawResetButton(m_LocalRotationProperty.quaternionValue.eulerAngles, Vector3.zero, out Vector3 localRotation, s_RotationResetButtonContent))
            {
                m_LocalRotationProperty.quaternionValue = Quaternion.Euler(localRotation);

                ApplyModifiedProperties();
            }

            // 判断 <是否触发【缩放重置】按钮>
            if (DrawResetButton(m_LocalScaleProperty.vector3Value, Vector3.one, out Vector3 localScale, s_ScaleResetButtonContent))
            {
                m_LocalScaleProperty.vector3Value = localScale;

                ApplyModifiedProperties();
            }

            // 结束水平布局
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制【所有属性复制按钮】
        /// </summary>
        private void DrawAllPropertyCopyButton()
        {
            // 绘制【所有属性复制】按钮，并判断 <该按钮是否被触发>
            if (GUILayout.Button(s_AllCopyButtonContent))
            {
                // 添加到剪贴板
                // 格式：位置,旋转,缩放
                GUIUtility.systemCopyBuffer = string.Format
                (
                    "{0},{1},{2}",
                    GetVector3String(m_LocalPositionProperty.vector3Value),
                    GetVector3String(m_LocalRotationProperty.quaternionValue.eulerAngles),
                    GetVector3String(m_LocalScaleProperty.vector3Value)
                );
            }
        }

        /// <summary>
        /// 绘制【所有属性粘贴按钮】
        /// </summary>
        private void DrawAllPropertyPasteButton()
        {
            // 判断 <解析【系统剪贴板】是否成功>
            if (TryParseVector3s(GUIUtility.systemCopyBuffer, out Vector3 position, out Vector3 rotation, out Vector3 scale))
            {
                // 绘制【所有属性粘贴】按钮，并判断 <该按钮是否被触发>
                if (GUILayout.Button(s_AllPasteButtonContent))
                {
                    // 更新【相对位置】
                    m_LocalPositionProperty.vector3Value = position;

                    // 更新【相对旋转】
                    m_LocalRotationProperty.quaternionValue = Quaternion.Euler(rotation);

                    // 更新【相对缩放】
                    m_LocalScaleProperty.vector3Value = scale;

                    ApplyModifiedProperties();
                }

                return;
            }

            // 禁用 GUI 交互
            GUI.enabled = false;

            GUILayout.Button(s_AllPasteButtonContent);

            // 启用 GUI 交互
            GUI.enabled = true;
        }

        /// <summary>
        /// 绘制【所有属性重置按钮】
        /// </summary>
        private void DrawAllPropertyResetButton()
        {
            // 判断 <是否存在属性值发生变更>
            if (m_LocalPositionProperty.vector3Value != Vector3.zero ||
                m_LocalRotationProperty.quaternionValue != Quaternion.identity ||
                m_LocalScaleProperty.vector3Value != Vector3.one)
            {
                // 绘制【所有属性重置】按钮，并判断 <该按钮是否被触发>
                if (GUILayout.Button(s_AllResetButtonContent))
                {
                    // 重置【相对位置】
                    m_LocalPositionProperty.vector3Value = Vector3.zero;

                    // 重置【相对旋转】
                    m_LocalRotationProperty.quaternionValue = Quaternion.identity;

                    // 重置【相对缩放】
                    m_LocalScaleProperty.vector3Value = Vector3.one;

                    ApplyModifiedProperties();
                }

                return;
            }

            // 禁用 GUI 交互
            GUI.enabled = false;

            GUILayout.Button(s_AllResetButtonContent);

            // 启用 GUI 交互
            GUI.enabled = true;
        }
        #endregion
    }
}
#endif
