#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Editor    = global::UnityEditor.Editor;
    using Object    = global::UnityEngine.Object;
    using Component = global::UnityEngine.Component;

    /// <summary>
    /// 检视窗口编辑器：原生组件
    /// </summary>
    /// <remarks>
    /// 用于自定义 <see cref="global::UnityEngine.Component">原生的 Unity 官方组件</see> 在 <see cref="global::UnityEditor.InspectorWindow">检视窗口</see> 的显示。
    /// <para/>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型所在的脚本文件应放在 Editor 文件夹下。
    /// </remarks>
    /// <typeparam name="TComponent">泛型 原生的 Unity 官方组件</typeparam>
    // [CanEditMultipleObjects] // 可以编辑多个对象
    // [CustomEditor(typeof(TComponent))] // 设置自定义检视窗口编辑器对应的类型
    public abstract class OriginalComponentInspectorEditor<TComponent> : Editor
        where TComponent : Component
    {
        #region 静态字段
        /// <summary>
        /// 原生组件检视窗口编辑器
        /// </summary>
        /// <remarks>
        /// 原生的 Unity 官方组件对应的检视窗口拓展类型。
        /// </remarks>
        private static Editor s_OriginalComponentEditor;
        #endregion

        #region 静态属性
        /// <summary>
        /// 原生组件检视窗口编辑器
        /// </summary>
        /// <remarks>
        /// 原生的 Unity 官方组件对应的检视窗口拓展类型。
        /// </remarks>
        protected static Editor OriginalComponentEditor
        {
            get
            {
                return s_OriginalComponentEditor;
            }
        }
        #endregion

        #region 静态私有方法
        /// <summary>
        /// 获取检视窗口编辑器
        /// </summary>
        /// <param name="targets">指定需要获取的目标对象。</param>
        /// <param name="inspectorEditorTypeFullName">指定需要获取实例的检视窗口编辑器类型的全称。</param>
        /// <returns>
        /// 如果获取成功，返回值为检视窗口编辑器；
        /// <br/>
        /// 如果获取失败，返回值为 <see langword="null"/>。
        /// </returns>
        private static Editor GetInspectorEditor(Object[] targets, string inspectorEditorTypeFullName)
        {
            // 获取检视窗口编辑器类型
            Type editorType = Assembly
                // 获取检视窗口编辑器类型所在的程序集
                .GetAssembly(typeof(global::UnityEditor.Editor))
                // 获取程序集下的所有类型
                .GetTypes()
                // 获取名称对应的原生的 Unity 官方组件编辑器类型
                .FirstOrDefault(item => item.FullName == inspectorEditorTypeFullName);

            // 判断 <对应的检视窗口编辑器类型是否为空值>
            if (editorType == null)
            {
                return null;
            }

            // 创建输入目标对应的检视窗口编辑器实例
            Editor inspectorEditor = CreateEditor(targets, editorType);

            return inspectorEditor;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 目标
        /// </summary>
        protected TComponent Target
        {
            get
            {
                return target as TComponent;
            }
        }

        /// <summary>
        /// 所有目标
        /// </summary>
        protected TComponent[] Targets
        {
            get
            {
                return targets.Cast<TComponent>().ToArray();
            }
        }

        /// <summary>
        /// 原生组件检视器类型全称
        /// </summary>
        protected abstract string OriginalComponentEditorTypeFullName { get; }
        #endregion

        #region 生命周期方法
        /// <summary>
        /// 启用时
        /// </summary>
        private void OnEnable()
        {
            // 获取原生组件检视窗口编辑器
            s_OriginalComponentEditor = GetInspectorEditor(targets, OriginalComponentEditorTypeFullName);

            // 判断 <原生组件检视窗口编辑器是否为空值>
            if (s_OriginalComponentEditor == null)
            {
                Debug.LogError(string.Format
                (
                    "无法加载 {0} 类型的编辑器，请检查 {1} 属性的值是否正确！\r\n{1} = {2}",
                    typeof(TComponent).FullName,
                    nameof(OriginalComponentEditorTypeFullName),
                    OriginalComponentEditorTypeFullName
                ));

                return;
            }

            // 获取序列化对象
            SerializedObject serializedObject = base.serializedObject;

            // 判断 <序列化对象是否为空>
            if (serializedObject != null)
            {
                GetSerializedProperties(serializedObject);
            }

            OnEnter();
        }

        /// <summary>
        /// 绘制检视窗口 GUI 时
        /// </summary>
        public sealed override void OnInspectorGUI()
        {
            // 判断 <原生组件检视窗口编辑器是否为空值>
            if (s_OriginalComponentEditor == null)
            {
                EditorGUILayout.HelpBox
                (
                    string.Format
                    (
                        "无法加载 {0} 类型的编辑器，请检查 {1} 属性的值是否正确！",
                        typeof(TComponent).FullName,
                        nameof(OriginalComponentEditorTypeFullName)
                    ),
                    MessageType.Error
                );

                return;
            }

            serializedObject.Update();

            // 绘制原生组件检视窗口编辑器的检视窗口 GUI
            s_OriginalComponentEditor.OnInspectorGUI();

            OnDraw();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        private void OnDisable()
        {
            OnExit();

            // 判断 <原生组件检视窗口编辑器是否为空值>
            if (s_OriginalComponentEditor == null)
            {
                return;
            }

            // 立即销毁原生组件检视窗口编辑器
            DestroyImmediate(s_OriginalComponentEditor);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        /// <param name="serializedObject">指定需要获取其中的序列化属性的序列化对象，</param>
        protected virtual void GetSerializedProperties(SerializedObject serializedObject) { }

        /// <summary>
        /// 进入时
        /// </summary>
        /// <remarks>
        /// 在 <see cref="OnEnable">启用时</see> 的最后调用。
        /// </remarks>
        protected virtual void OnEnter() { }

        /// <summary>
        /// 绘制时
        /// </summary>
        /// <remarks>
        /// 在 <see cref="OnInspectorGUI">当绘制检视窗口 GUI 时</see> 的最后调用。
        /// </remarks>
        protected abstract void OnDraw();

        /// <summary>
        /// 退出时
        /// </summary>
        /// <remarks>
        /// 在 <see cref="OnDisable">禁用时</see> 的最后调用。
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
