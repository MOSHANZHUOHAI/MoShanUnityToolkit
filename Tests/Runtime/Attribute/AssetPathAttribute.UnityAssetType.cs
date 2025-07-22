using System;

namespace MoShan.Unity.EngineExpand.Test
{
    /// <summary>
    /// 资产路径特性
    /// </summary>
    public partial class AssetPathAttribute
    {
        #region 枚举
        /// <summary>
        /// Unity 资产类型
        /// </summary>
        [Serializable]
        public enum UnityAssetType : int
        {
            /// <summary>
            /// 所有 Unity 可识别的资产类型
            /// </summary>
            All = 0,
            /// <summary>
            /// 动画片段
            /// </summary>
            AnimationClip,
            /// <summary>
            /// 音频片段
            /// </summary>
            AudioClip,
            /// <summary>
            /// GUI 风格
            /// </summary>
            GUISkin,
            /// <summary>
            /// 字体
            /// </summary>
            Font,
            /// <summary>
            /// 材质
            /// </summary>
            Material,
            /// <summary>
            /// 模型
            /// </summary>
            Mesh,
            /// <summary>
            /// 物理材质
            /// </summary>
            PhysicMaterial,
            /// <summary>
            /// 二维物理材质
            /// </summary>
            PhysicsMaterial2D,
            /// <summary>
            /// 预制件
            /// </summary>
            Prefab,
            /// <summary>
            /// 脚本对象
            /// </summary>
            ScriptableObject,
            /// <summary>
            /// Shader
            /// </summary>
            Shader,
            /// <summary>
            /// 精灵
            /// </summary>
            Sprite,
            /// <summary>
            /// 文本
            /// </summary>
            Text,
            /// <summary>
            /// 纹理
            /// </summary>
            Texture,

            // 以下枚举对应了编辑器命名空间中的资产类型

            /// <summary>
            /// 动画控制器
            /// </summary>
            AnimatorController,
            /// <summary>
            /// 程序集定义
            /// </summary>
            AssemblyDefinition,
            /// <summary>
            /// 文件夹
            /// </summary>
            Folder,
            /// <summary>
            /// 脚本
            /// </summary>
            MonoScript,
            /// <summary>
            /// 场景
            /// </summary>
            Scene,
        }
        #endregion
    }
}
