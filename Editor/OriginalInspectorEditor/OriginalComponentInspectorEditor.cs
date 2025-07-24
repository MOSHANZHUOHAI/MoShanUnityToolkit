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
    /// 检视窗口编辑器：原始组件
    /// </summary>
    /// <remarks>
    /// <para>用于自定义【<see cref="global::UnityEngine.Component">原生的 Unity 官方组件</see>】在【<see cref="global::UnityEditor.InspectorWindow">检视窗口</see>】的显示。</para>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件应放在【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    /// <typeparam name="TComponent">泛型 原生的 Unity 官方组件</typeparam>
    // [CanEditMultipleObjects] // 可以编辑多个对象
    // [CustomEditor(typeof(TComponent))] // 设置自定义检视窗口编辑器对应的类型
    public abstract class OriginalComponentInspectorEditor<TComponent> : Editor
        where TComponent : Component
    {
        #region 静态字段
        /// <summary>
        /// 原始组件检视窗口编辑器
        /// </summary>
        /// <remarks>
        /// 原生的 Unity 官方组件对应的检视窗口拓展类型
        /// </remarks>
        private static Editor s_OriginalComponentEditor;
        #endregion

        #region 静态属性
        /// <summary>
        /// 原始组件检视窗口编辑器
        /// </summary>
        /// <remarks>
        /// 原生的 Unity 官方组件对应的检视窗口拓展类型
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
        /// 获取【检视窗口编辑器】
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="inspectorEditorTypeFullName">检视窗口编辑器类型全称</param>
        /// <returns>若获取成功，返回【检视窗口编辑器】；否则，返回【空】</returns>
        private static Editor GetInspectorEditor(Object target, string inspectorEditorTypeFullName)
        {
            // 获取【检视窗口编辑器】类型
            Type editorType = Assembly
                // 获取【检视窗口编辑器】类型所在的程序集
                .GetAssembly(typeof(global::UnityEditor.Editor))
                // 获取【程序集】下的所有类型
                .GetTypes()
                // 获取名称对应的【原始组件编辑器】类型
                .FirstOrDefault(item => item.FullName == inspectorEditorTypeFullName);

            // 判断 <对应的【检视窗口编辑器】类型是否为【空】>
            if (editorType == null)
            {
                return null;
            }

            // 创建【输入目标】对应的【检视窗口编辑器】实例
            Editor inspectorEditor = CreateEditor(target, editorType);

            return inspectorEditor;
        }
        #endregion

        #region 字段
        /// <summary>
        /// 目标
        /// </summary>
        private TComponent m_Target;
        #endregion

        #region 属性
        /// <summary>
        /// 目标
        /// </summary>
        protected TComponent Target
        {
            get
            {
                return m_Target;
            }
        }

        /// <summary>
        /// 原始组件检视器类型全称
        /// </summary>
        protected abstract string OriginalComponentEditorTypeFullName { get; }
        #endregion

        #region 生命周期方法
        /// <summary>
        /// 启用时
        /// </summary>
        private void OnEnable()
        {
            // 获取【目标】
            m_Target = target as TComponent;

            // 获取【原始组件检视窗口编辑器】
            s_OriginalComponentEditor = GetInspectorEditor(target, OriginalComponentEditorTypeFullName);

            // 获取【序列化对象】
            SerializedObject serializedObject = base.serializedObject;

            // 判断 <【序列化对象】是否为空>
            if (serializedObject != null)
            {
                GetSerializedProperties(base.serializedObject);
            }

            OnEnter();
        }

        /// <summary>
        /// 绘制检视窗口 GUI 时
        /// </summary>
        public sealed override void OnInspectorGUI()
        {
            // 判断 <【原始组件检视窗口编辑器】是否为【空】>
            if (s_OriginalComponentEditor == null)
            {
                return;
            }

            // 绘制【原始组件检视窗口编辑器】的检视窗口 GUI
            s_OriginalComponentEditor.OnInspectorGUI();

            OnDraw();
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        private void OnDisable()
        {
            OnExit();

            // 判断 <【原始组件检视窗口编辑器】是否为【空】>
            if (s_OriginalComponentEditor == null)
            {
                return;
            }

            // 立即销毁【原始组件检视窗口编辑器】
            DestroyImmediate(s_OriginalComponentEditor);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取【序列化属性】
        /// </summary>
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
