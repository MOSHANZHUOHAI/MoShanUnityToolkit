#if UNITY_EDITOR
using MoShan.Unity.EngineExpand.Test;
using System;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand.Test
{
    using Rect = global::UnityEngine.Rect;
    using Object = global::UnityEngine.Object;

    /// <summary>
    /// 属性绘制器：资产路径
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型所在的脚本文件需要放置在【Editor】文件夹下；</br>
    /// <br>该类型需要配合【<see cref="AssetPathAttribute">资产路径特性</see>】类型使用，且对应类型所在的脚本文件应放置在非【Editor】文件夹下。</br>
    /// </para>
    /// </remarks>
    [CustomPropertyDrawer(typeof(AssetPathAttribute))]
    internal sealed class AssetPathAttributeDrawer : PropertyDrawer
    {
        #region 生命周期方法
        /// <summary>
        /// 绘制 GUI 时
        /// </summary>
        /// <param name="position">绘制区域</param>
        /// <param name="property">绘制属性</param>
        /// <param name="label">绘制属性的标签</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 判断 <【属性类型】是否不为【字符串】>
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "标记字段类型非【字符串 string】，请选择正确的字段。", MessageType.Error);

                return;
            }

            EditorGUI.LabelField(position, label);

            AssetPathAttribute target = attribute as AssetPathAttribute;

            // 根据路径获取【对应的资产对象】
            Object asset = AssetDatabase.LoadAssetAtPath(property.stringValue, target.AssetType);

            // 开始【GUI 变更检测】
            EditorGUI.BeginChangeCheck();

            asset = EditorGUI.ObjectField
            (
                position,
                label,
                asset != null ? asset : null,
                target.AssetType,
                false
            );

            // 结束【GUI 变更检测】，并判断 <是否未发生 GUI 变更>
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            // 更新【路径】
            property.stringValue = AssetDatabase.GetAssetPath(asset);
        }

        /// <summary>
        /// 获取【属性高度】
        /// </summary>
        /// <param name="property">序列化属性</param>
        /// <param name="label">GUI内容</param>
        /// <returns>返回属性在【<see cref="UnityEditor.InspectorWindow">检视窗口</see>】的绘制高度</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        #endregion
    }
}
#endif
