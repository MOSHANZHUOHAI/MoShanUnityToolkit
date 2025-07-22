using System;
using UnityEngine;

namespace MoShan.Unity.EngineExpand.Test
{
    using Object = global::UnityEngine.Object;

    /// <summary>
    /// 资产路径特性
    /// </summary>
    /// <remarks>
    /// <para>
    /// <br><b>使用：</b></br>
    /// <br>使用该特性标记的【字符串字段】将在检视窗口中显示为获取对应资产的引用获取字段。</br>
    /// </para>
    /// <para>
    /// <br><b>注意：</b></br>
    /// <br>该类型的特性标签对字段进行标记时，应位于其它 Unity 原生特性标签之前，否则特性效果可能不会生效。</br>
    /// <br>该类型所在的脚本文件应放置在非【Editor】文件夹下，否则会因为找不到该类型而导致报错。</br>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// 
    /// /// <summary>
    /// /// 精灵路径_0
    /// /// </summary>
    /// [AssetPath(AssetPathAttribute.EAssetType.Sprite)]
    /// public string SpritePath_0 = string.Empty;
    /// 
    /// /// <summary>
    /// /// 精灵路径_1
    /// /// </summary>
    /// [AssetPath(typeof(UnityEngine.Sprite))]
    /// public string SpritePath_1 = string.Empty;
    /// 
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)] // 仅对字段生效，不可继承，单个字段上不允许添加多个该属性
    public sealed partial class AssetPathAttribute : PropertyAttribute
    {
        #region 静态私有方法
        /// <summary>
        /// 获取【资产类型】
        /// </summary>
        /// <param name="assetType">资产类型</param>
        /// <returns>返回【输入 Unity 资产类型枚举】对应的【Unity 资产类型】。</returns>
        private static Type GetAssetType(UnityAssetType assetType)
        {
            return assetType switch
            {
                UnityAssetType.All               => typeof(Object),
                UnityAssetType.AnimationClip     => typeof(AnimationClip),
                UnityAssetType.AudioClip         => typeof(AudioClip),
                UnityAssetType.Font              => typeof(Font),
                UnityAssetType.GUISkin           => typeof(GUISkin),
                UnityAssetType.Material          => typeof(Material),
                UnityAssetType.Mesh              => typeof(Mesh),
                UnityAssetType.PhysicMaterial    => typeof(PhysicMaterial),
                UnityAssetType.PhysicsMaterial2D => typeof(PhysicsMaterial2D),
                UnityAssetType.Prefab            => typeof(GameObject),
                UnityAssetType.ScriptableObject  => typeof(ScriptableObject),
                UnityAssetType.Sprite            => typeof(Sprite),
                UnityAssetType.Shader            => typeof(Shader),
                UnityAssetType.Text              => typeof(TextAsset),
                UnityAssetType.Texture           => typeof(Texture),
#if UNITY_EDITOR
                UnityAssetType.AnimatorController => typeof(UnityEditor.Animations.AnimatorController),
                UnityAssetType.AssemblyDefinition => typeof(UnityEditorInternal.AssemblyDefinitionAsset),
                UnityAssetType.Folder             => typeof(UnityEditor.DefaultAsset),
                UnityAssetType.MonoScript         => typeof(UnityEditor.MonoScript),
                UnityAssetType.Scene              => typeof(UnityEditor.SceneAsset),
#endif
                _                                => null,
            };
        }
        #endregion

        #region 字段
        /// <summary>
        /// 资产类型
        /// </summary>
        public readonly Type AssetType;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="assetType">资产类型</param>
        public AssetPathAttribute(UnityAssetType assetType)
        {
            AssetType = GetAssetType(assetType);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="type">类型</param>
        public AssetPathAttribute(Type type)
        {
            // 判断 <【输入类型】是否不为【空】>
            if (type != null)
            {
                AssetType = type;
            }
        }
        #endregion
    }
}
