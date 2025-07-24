#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    /// <summary>
    /// 检视窗口编辑器
    /// </summary>
    /// <remarks>
    /// <para>用于自定义【<see cref="global::UnityEngine.Component">组件</see>】或【<see cref="global::UnityEngine.ScriptableObject">脚本对象</see>】在【<see cref="global::UnityEditor.InspectorWindow">检视窗口</see>】的显示。</para>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件应放在【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    /// <typeparam name="TUnityObject">泛型 Unity 对象，建议为 MonoBahviour 或 ScriptableObejct 的子级类型</typeparam>
    /// <example>
    /// <code><![CDATA[
    /// 
    /// /// <summary>
    /// /// 检视窗口编辑器：示例脚本
    /// /// <para>注意：该脚本需要放在 Editor 文件夹下</para>
    /// /// </summary>
    /// [CustomEditor(typeof(Example))] // 设置自定义检视窗口编辑器对应的类型
    /// public sealed class ExampleInspectorEditor : ScriptEditorBase<ExampleEditor>
    /// {
    ///     #region 字段
    ///     /// <summary>
    ///     /// 序列化属性：名称
    ///     /// </summary>
    ///     private m_NameSerializedProperty
    ///     #endregion
    ///     
    ///     #region 属性
    ///     /// <inheritdoc/>
    ///     protected virtual string Name
    ///     {
    ///         get
    ///         {
    ///             return "示例组件";
    ///         }
    ///     }
    ///     #endregion
    ///     
    ///     #region 私有方法
    ///     /// <inheritdoc/>
    ///     protected override void GetSerializedProperties(SerializedObject serializedProperty)
    ///     {
    ///         // 获取【绘制时需要用到的序列化属性】，示例如下：
    ///         
    ///         // 获取序列化属性：名称
    ///         m_NameSerializedProperty = serializedObject.FindProperty("m_Name");
    ///     }    
    ///     
    ///     /// <inheritdoc/>
    ///     protected override void OnDraw()
    ///     {
    ///         // 绘制【需要显示在检视窗口中的编辑器拓展内容】，示例如下：
    ///         
    ///         // 开始【GUI 变更检测】
    ///         EditorGUI.BeginChangeCheck();
    ///         
    ///         // 绘制属性：名称
    ///         EditorGUILayout.PropertyField(m_NameSerializedProperty);
    ///
    ///         // 停止【GUI 变更检测】，并判断 <是否发生变更>
    ///         if (EditorGUI.BeginChangeCheck())
    ///         {
    ///             // 应用【修改后的属性】
    ///             base.serializedObject.ApplyModifiedProperties();
    ///         }
    ///     }
    ///     #endregion
    /// }
    /// 
    /// ]]></code>
    /// </example>
    // [CanEditMultipleObjects] // 可以编辑多个对象
    // [CustomEditor(typeof(TUnityObject))] // 设置自定义检视窗口编辑器对应的类型
    public abstract class InspectorEditor<TUnityObject> : Editor
        where TUnityObject : UnityEngine.Object
    {
        #region 静态私有方法
        /// <summary>
        /// 获取【Mono 脚本】
        /// </summary>
        /// <param name="unityObject">UnityEngine 命名空间下的 Object 类型实例，此处需要输入 MonoBehaviour 或 ScriptableObject 的子类实例</param>
        /// <returns>输入的 UnityEngine.Object 子类实例对应的 Mono 脚本</returns>
        private static MonoScript GetMonoScript(UnityEngine.Object unityObject)
        {
            // 判断 输入对象是否为空
            if (unityObject == null)
                return null;

            // 判断 输入对象是否为 MonoBehaviour 子类
            if (unityObject is MonoBehaviour)
            {
                return MonoScript.FromMonoBehaviour(unityObject as MonoBehaviour); // 获取 MonoBehaviour 的 Mono 脚本
            }

            // 判断 输入对象是否为 ScriptableObject 子类
            if (unityObject is ScriptableObject)
            {
                return MonoScript.FromScriptableObject(unityObject as ScriptableObject); // 获取 ScriptableObject 的 Mono 脚本
            }

            // 获取 方法信息：UnityEditor.MonoScript 中的内部方法 【internal static extern MonoScript FromScriptedObject(UnityEngine.Object obj)】
            MethodInfo fromScriptedObjectMethod = Type.GetType("UnityEditor.MonoScript, UnityEditor").GetMethod("FromScriptedObject", BindingFlags.NonPublic | BindingFlags.Static);
            return (MonoScript)fromScriptedObjectMethod.Invoke(null, new object[] { unityObject }); // 使用反射尝试获取输入对象的 Mono 脚本
        }
        #endregion

        #region 字段
        /// <summary>
        /// 目标
        /// </summary>
        protected TUnityObject m_Target;

        /// <summary>
        /// 目标脚本文件
        /// </summary>
        /// <remarks>
        /// 当前拓展目标类型对应的脚本文件
        /// </remarks>
        private MonoScript m_TargetScript;

        /// <summary>
        /// 编辑器拓展脚本文件
        /// </summary>
        /// <remarks>
        /// 当前拓展目标类型对应的编辑器拓展脚本文件
        /// </remarks>
        private MonoScript m_EditorScript;
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        protected virtual string Name
        {
            get
            {
                // 默认返回组件脚本的类型名称
                return typeof(TUnityObject).Name;
            }
        }
        #endregion

        #region 生命周期方法
        /// <summary>
        /// 启用时
        /// </summary>
        private void OnEnable()
        {
            // 获取【当前选中的实例】
            m_Target = target as TUnityObject;

            #region 获取【脚本】
            m_TargetScript = GetMonoScript(m_Target); // 获取【组件脚本】

            m_EditorScript = GetMonoScript(this); // 获取【编辑器拓展脚本】
            #endregion

            // 获取【序列化对象】
            SerializedObject serializedObject = base.serializedObject;

            // 判断 <【序列化对象】是否为【空】>
            if (serializedObject != null)
            {
                GetSerializedProperties(base.serializedObject);
            }

            OnEnter();
        }

        /// <summary>
        /// 绘制检视窗口 GUI 时
        /// </summary>
        public override void OnInspectorGUI()
        {
            #region 绘制【标题】
            // 开始附带背景的【水平布局】
            EditorGUILayout.BeginHorizontal("box");

            // 添加【自适应空间】
            GUILayout.FlexibleSpace();

            // 绘制文本为粗体的【标签】
            GUILayout.Label(Name, EditorStyles.boldLabel);

            // 添加【自适应空间】
            GUILayout.FlexibleSpace();

            // 结束【水平布局】
            EditorGUILayout.EndHorizontal();
            #endregion

            #region 绘制【脚本】
            // 禁用 GUI 交互
            GUI.enabled = false;

            // 判断 <是否显示目标脚本>
            if (InspectorEditorUtility.IsDisplayTargetScript)
            {
                EditorGUILayout.ObjectField
                (
                    new GUIContent("组件脚本", "组件对应的脚本。"),
                    m_TargetScript,
                    typeof(MonoScript),
                    false
                );
            }

            // 判断 <是否显示编辑器脚本>
            if (InspectorEditorUtility.IsDisplayEditorScript)
            {
                EditorGUILayout.ObjectField
                (
                    new GUIContent("编辑器脚本", "该检视窗口编辑器对应的脚本。"),
                    m_EditorScript,
                    typeof(MonoScript),
                    false
                );
            }

            // 启用 GUI 交互
            GUI.enabled = true;
            #endregion

            EditorGUILayout.Space();

            OnDraw();
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        private void OnDisable()
        {
            OnExit();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取【序列化属性】
        /// </summary>
        /// <remarks>
        /// 在【<see cref="OnEnter">进入时</see>】之前调用
        /// </remarks>
        /// <param name="serializedObject">序列化对象</param>
        protected virtual void GetSerializedProperties(SerializedObject serializedObject) { }

        /// <summary>
        /// 进入时
        /// </summary>
        /// <remarks>
        /// 在【<see cref="OnEnable">启用时</see>】的最后调用
        /// </remarks>
        protected virtual void OnEnter() { }

        /// <summary>
        /// 绘制时
        /// </summary>
        /// <remarks>
        /// 在【<see cref="OnInspectorGUI">当绘制检视窗口 GUI 时</see>】的最后调用
        /// </remarks>
        protected abstract void OnDraw();

        /// <summary>
        /// 退出时
        /// </summary>
        /// <remarks>
        /// 在【<see cref="OnDisable">禁用时</see>】的最后调用
        /// </remarks>
        protected virtual void OnExit() { }

        /// <summary>
        /// 应用【修改后的属性】
        /// </summary>
        protected void ApplyModifiedProperties()
        {
            base.serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}
#endif
