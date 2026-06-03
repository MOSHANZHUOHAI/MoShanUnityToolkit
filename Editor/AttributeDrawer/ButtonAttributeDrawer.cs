#if UNITY_EDITOR
using MoShan.Unity.EngineExpand;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    /// <summary>
    /// 检视窗口编辑器：按钮特性
    /// </summary>
    /// <remarks>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型所在的脚本文件需要放置在 Editor 文件夹下；
    /// <br/>
    /// 该类型需要配合 <see cref="ButtonAttribute">按钮特性</see> 类型使用，且对应类型所在的脚本文件应放置在非Editor文件夹下。
    /// </remarks>
    internal abstract class ButtpnAttributeDrawer<T> : Editor
        where T : global::UnityEngine.Object
    {
        #region 生命周期方法
        /// <summary>
        /// 绘制检视窗口 GUI 时
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 绘制原有检视窗口的 GUI 内容
            base.OnInspectorGUI();

            // 获取目标
            object target;

            // 判断 <当前实例的类型是否为 T 类型的子级类型>
            if (base.target.GetType().IsSubclassOf(typeof(T)))
            {
                target = base.target as T;
            }
            else
            {
                return;
            }

            // 判断 <当前实例是否为空值>
            if (target == null)
            {
                return;
            }

            // 获取类型中所有被 ButtonAttribute 标记的方法信息
            MethodInfo[] methodInfos = target
                // 获取目标类型
                .GetType()
                // 获取目标类型中所有的公开、非公开、实例、静态的方法
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                // 获取目标类型中的被 ButtonAttribute 标记的方法
                .Where(method => Attribute.IsDefined(method, typeof(ButtonAttribute)))
                // 转换为数组
                .ToArray();

            // 遍历循环以使用获取到的所有方法信息绘制按钮
            foreach (MethodInfo methodInfo in methodInfos)
            {
                // 判断 <当前方法信息是否为空值>
                if (methodInfo == null)
                {
                    return;
                }

                DrawButton(methodInfo.GetCustomAttribute<ButtonAttribute>().Name, methodInfo);
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取返回值时
        /// </summary>
        /// <param name="target">指定调用方法的对象。</param>
        /// <param name="methodInfo">指定调用方法的信息。</param>
        /// <param name="returnValue">指定调用方法后的返回值。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private protected virtual void OnGetReturnValue(global::UnityEngine.Object target, MethodInfo methodInfo, object returnValue) { }
        #endregion

        #region 私有方法
        /// <summary>
        /// 绘制按钮
        /// </summary>
        /// <param name="methodName">指定需要绘制的按钮对应的方法的名称。</param>
        /// <param name="methodInfo">指定需要绘制的按钮对应的方法的方法信息。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawButton(string methodName, MethodInfo methodInfo)
        {
            // 判断 <输入方法名称是否为空值或空白字符串>
            if (string.IsNullOrEmpty(methodName))
            {
                // 获取输入方法信息的名称
                methodName = methodInfo.Name;
            }

            // 绘制调用方法按钮，并判断 <该按钮是否被触发>
            if (GUILayout.Button(new GUIContent(methodName), GUILayout.ExpandWidth(true)))
            {
                // 遍历循环所有被检查实例
                foreach (global::UnityEngine.Object target in targets)
                {
                    object currentTarget;

                    // 判断 <当前目标的类型是否为 T 类型的子级类型>
                    if (target.GetType().IsSubclassOf(typeof(T)))
                    {
                        currentTarget = target as T;
                    }
                    else
                    {
                        continue;
                    }

                    // 判断 <当前实例是否为空值>
                    if (currentTarget == null)
                    {
                        continue;
                    }

                    // 获取调用方法的返回值
                    object returnValue = methodInfo.Invoke(currentTarget, new object[] { });

                    // 判断 <调用方法的返回值类型是否为 System.Void>
                    if (methodInfo.ReturnType == typeof(void))
                    {
                        return;
                    }

                    OnGetReturnValue(target, methodInfo, returnValue);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 检视窗口编辑器：MonoBehaviour 的子级类型中的按钮特性
    /// </summary>
    /// <remarks>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型所在的脚本文件需要放置在 Editor 文件夹下；
    /// <br/>
    /// 该类型需要配合 <see cref="ButtonAttribute">按钮特性</see> 类型使用，且对应类型所在的脚本文件应放置在非Editor文件夹下。
    /// </remarks>
    [CanEditMultipleObjects] // 可编辑多个对象
    [CustomEditor(typeof(MonoBehaviour), true)] // 设置自定义检视窗口编辑器对应的类型与是否子类可继承
    internal sealed class MonoBehaviourButtonAttributeDrawer : ButtpnAttributeDrawer<MonoBehaviour>
    {
        #region 内部方法
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private protected sealed override void OnGetReturnValue(global::UnityEngine.Object target, MethodInfo methodInfo, object returnValue)
        {
            // 判断 <返回值是否为迭代器（即协程方法返回值）>、<当前实例为 MonoBehaviour 的子类>
            if (returnValue is IEnumerator coroutine && target is MonoBehaviour monoBehaviour)
            {
                // 开启协程
                monoBehaviour.StartCoroutine(coroutine);
            }
            else
            {
                Debug.Log($"{methodInfo.Name}() 方法调用后的返回值：{returnValue}");
            }
        }
        #endregion
    }

    /// <summary>
    /// 检视窗口编辑器：ScriptableObject 的子级类型中的按钮特性
    /// </summary>
    /// <remarks>
    /// <b>注意：</b>
    /// <br/>
    /// 该类型所在的脚本文件需要放置在 Editor 文件夹下；
    /// <br/>
    /// 该类型需要配合 <see cref="ButtonAttribute">按钮特性</see> 类型使用，且对应类型所在的脚本文件应放置在非Editor文件夹下。
    /// </remarks>
    [CanEditMultipleObjects] // 可编辑多个对象
    [CustomEditor(typeof(ScriptableObject), true)] // 设置自定义检视窗口编辑器对应的类型与是否子类可继承
    internal sealed class ScriptableObjectButtonAttributeDrawer : ButtpnAttributeDrawer<ScriptableObject>
    {
        #region 内部方法
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private protected sealed override void OnGetReturnValue(global::UnityEngine.Object target, MethodInfo methodInfo, object returnValue)
        {
            // 判断 <返回值是否为迭代器（即协程方法返回值）>
            if (returnValue is IEnumerator)
            {
                Debug.Log($"{methodInfo.Name}() 可能是为迭代器方法，但 ScriptableObject 无法开启协程。");
            }
            else
            {
                Debug.Log($"{methodInfo.Name}() 方法调用后的返回值：{returnValue}");
            }
        }
        #endregion
    }
}
#endif
