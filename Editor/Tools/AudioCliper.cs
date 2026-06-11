#if UNITY_EDITOR
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace MoShan.Unity.EditorExpand
{
    using Vector2 = global::UnityEngine.Vector2;
    using Rect    = global::UnityEngine.Rect;

    /// <summary>
    /// 编辑器窗口：音频剪辑器
    /// </summary>
    /// <remarks>
    /// 注意：该脚本需要放在 Editor 文件夹下
    /// </remarks>
    internal sealed class AudioCliper : EditorWindow
    {
        #region 启动
        /// <summary>
        /// 显示窗口
        /// </summary>
        [MenuItem("Tools/音频剪辑器")]
        public static void ShowWindow()
        {
            // 获取窗口
            AudioCliper window = EditorWindow.GetWindow<AudioCliper>(true, "音频剪辑器", true);

            // 设置窗口的大小
            float width  = 500f;
            float height = 800f;

            // 设置窗口的大小下限
            window.minSize = new Vector2(width, height);

            // 获取当前显示器的分辨率以设置窗口到屏幕中心
            window.position = new Rect
            (
                Screen.currentResolution.width  * 0.5f - width  * 0.5f,
                Screen.currentResolution.height * 0.5f - height * 0.5f,
                width,
                height
            );

            // 绘制窗口
            window.Show();

            // 聚焦窗口
            window.Focus();
        }
        #endregion

        #region 字段
        /// <summary>
        /// 音频片段
        /// </summary>
        private AudioClip m_AudioClip;

        /// <summary>
        /// 临时音频片段
        /// </summary>
        private AudioClip m_TempAudioClip;

        /// <summary>
        /// 组件：音频源
        /// </summary>
        private AudioSource m_AudioSource;

        /// <summary>
        /// 开始时间
        /// </summary>
        private float m_StartTime = 0f;

        /// <summary>
        /// 结束时间
        /// </summary>
        private float m_EndTime = 0f;

        /// <summary>
        /// 音量
        /// </summary>
        private float m_Volume = 1f;

        /// <summary>
        /// 波形图
        /// </summary>
        private Texture2D m_WaveformTexture;

        /// <summary>
        /// 进度
        /// </summary>
        private float m_Progress = 0f;

        /// <summary>
        /// 数组：样本
        /// </summary>
        private float[] m_Samples;
        #endregion

        #region 生命周期方法
        private void OnEnable()
        {
            // 创建隐藏的音频源组件用于播放音频
            m_AudioSource = EditorUtility.CreateGameObjectWithHideFlags
            (
                "E_AudioSource",
                HideFlags.HideAndDontSave,
                typeof(AudioSource)
            ).GetComponent<AudioSource>();
        }

        private void OnGUI()
        {
            // 开始垂直布局
            EditorGUILayout.BeginVertical();

            // 获取音频片段
            m_AudioClip = EditorGUILayout.ObjectField("音频片段", m_AudioClip, typeof(AudioClip), false) as AudioClip;

            // 判断 <音频片段是否发生变化>
            if (m_TempAudioClip != m_AudioClip)
            {
                m_TempAudioClip = m_AudioClip;

                // 初始化
                Init();
            }

            // 判断 <音频片段是否不为空值>
            if (m_AudioClip)
            {
                DisplayAudioClipInfo();

                AdjustClipSection();

                // DisplayVolumeControl();

                //  获取音量
                m_Volume = EditorGUILayout.Slider("音量", m_Volume, 0f, 1f);

                GUILayout.Space(10);

                // 判断 <是否正在播放>
                if  (m_AudioSource.isPlaying)
                {
                    // 判断 <是否点击暂停按钮>
                    if (GUILayout.Button(EditorGUIUtility.IconContent("PauseButton On"), GUILayout.Height(40)))
                    {
                        // 暂停播放
                        m_AudioSource.Pause();
                    }
                }
                else
                {
                    // 判断 <是否点击播放按钮>
                    if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton On"), GUILayout.Height(40)))
                    {
                        // 判断 <是否正在播放>
                        if (m_AudioSource.time != 0f)
                        {
                            // 恢复播放
                            m_AudioSource.UnPause();
                        }
                        else
                        {
                            PlayTestAudio();
                        }
                    }
                }

                // 判断 <是否正在播放>
                if (m_AudioSource.time != 0f)
                {
                    // 判断 <是否点击停止按钮>
                    if (GUILayout.Button("■", GUILayout.Height(40)))
                    {
                        // 停止播放
                        m_AudioSource.Stop();
                    }
                }

                m_Progress = m_AudioSource.time / m_AudioClip.length;
                EditorGUILayout.Slider("进度", m_Progress, 0f, 1f);

                GUILayout.Space(10);

                // 判断 <是否点击保存按钮>
                if (GUILayout.Button("保存", GUILayout.Height(40)))
                {
                    ClipAndSave();
                }
            }

            // 结束垂直布局
            EditorGUILayout.EndVertical();

            // 判断 <是否正在播放>
            if (m_AudioSource.isPlaying)
            {
                // 重绘
                Repaint();
            }
        }

        private void OnDisable()
        {
            // 销毁音频源组件
            DestroyImmediate(m_AudioSource.gameObject);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            // 判断 <音频片段是否不为空值>
            if (m_AudioClip != null)
            {
                // 获取结束时间
                m_EndTime = m_AudioClip.length;
                m_Samples = new float[m_AudioClip.samples * m_AudioClip.channels];
                m_AudioClip.GetData(m_Samples, 0);
                UpdateWaveformTexture();
            }
        }

        /// <summary>
        /// 显示音频片段信息
        /// </summary>
        private void DisplayAudioClipInfo()
        {
            EditorGUILayout.LabelField($"开始时间: {m_StartTime:F2}s");
            EditorGUILayout.LabelField($"结束时间: {m_EndTime:F2}s");

            GUILayout.Space(10);
        }

        /// <summary>
        /// 调整剪辑音频片段
        /// </summary>
        private void AdjustClipSection()
        {
            GUILayout.Label("调整剪辑片段");

            Rect waveformRect = GUILayoutUtility.GetRect(position.width, 64);

            // 判断 <波浪图是否为空值>或<波浪图宽度是否不等于窗口宽度>
            if (m_WaveformTexture == null || m_WaveformTexture.width != (int)waveformRect.width)
            {
                UpdateWaveformTexture();
            }

            // 判断 <波浪图是否不为空值>
            if (m_WaveformTexture != null)
            {
                EditorGUI.DrawPreviewTexture(waveformRect, m_WaveformTexture);
            }

            EditorGUILayout.MinMaxSlider("", ref m_StartTime, ref m_EndTime, 0f, m_AudioClip != null ? m_AudioClip.length : 10f);
        }

        /// <summary>
        /// 播放测试音频片段
        /// </summary>
        private void PlayTestAudio()
        {
            // 判断 <音频片段是否不为空值>
            if (m_AudioClip != null)
            {
                AudioClip clippedAudioClip = ClipAudio(m_AudioClip, m_StartTime, m_EndTime);

                // 设置音频片段
                m_AudioSource.clip = clippedAudioClip;

                // 设置音量
                m_AudioSource.volume = m_Volume;

                // 播放音频片段
                m_AudioSource.Play();
            }
            else
            {
                Debug.LogError("请指定一段音频");
            }
        }

        /// <summary>
        /// 剪辑并保存音频片段
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClipAndSave()
        {
            // 判断 <音频片段是否不为空值>
            if (m_AudioClip != null)
            {
                AudioClip clippedAudioClip = ClipAudio(m_AudioClip, m_StartTime, m_EndTime);
                string savePath = EditorUtility.SaveFilePanel("保存剪辑的音频", "Assets", clippedAudioClip.name + "_New", "wav");

                // 判断 <保存路径是否不为空值>
                if (!string.IsNullOrEmpty(savePath))
                {
                    // SaveAudioClipToFile(clippedAudioClip, savePath);
                }
            }
            else
            {
                Debug.LogError("请指定一段音频");
            }
        }

        /// <summary>
        /// 更新波形图
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateWaveformTexture()
        {
            // 判断 <样本是否不为空值>
            if (m_Samples != null)
            {
                m_WaveformTexture = GenerateWaveformTexture(m_Samples, m_AudioClip.channels, (int)position.width, 64);
            }
        }

        /// <summary>
        /// 生成波形图
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="channels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>返回生成的波形图</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Texture2D GenerateWaveformTexture(float[] samples, int channels, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            Color[] colors = new Color[width * height];

            // 循环以填充贴图颜色数组
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.black;
            }

            // 更新贴图像素
            texture.SetPixels(colors);

            int packSize = (samples.Length / channels) / width;
            for (int x = 0; x < width; x++)
            {
                float max = 0;
                for (int i = 0; i < packSize; i++)
                {
                    float sampleValue = samples[(x * packSize + i) * channels] * m_Volume;
                    if (sampleValue > max) max = sampleValue;
                }
                int heightValue = (int)(max * height);
                for (int y = 0; y < heightValue; y++)
                {
                    texture.SetPixel(x, y, Color.yellow);
                }
            }

            // 应用贴图修改
            texture.Apply();

            return texture;
        }

        /// <summary>
        /// 剪辑音频片段
        /// </summary>
        /// <param name="originalClip">指定需要剪辑的原始音频片段。</param>
        /// <param name="start">指定剪辑 <paramref name="originalClip"/> 的开始时间。</param>
        /// <param name="end">指定剪辑 <paramref name="originalClip"/> 的结束时间。</param>
        /// <returns>返回值为 <paramref name="originalClip"/> 的剪辑结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private AudioClip ClipAudio(AudioClip originalClip, float start, float end)
        {
            float[] data = new float[originalClip.samples * originalClip.channels];
            originalClip.GetData(data, 0);

            int newSampleCount = (int)((end - start) * originalClip.frequency);
            float[] newData = new float[newSampleCount * originalClip.channels];

            int startIndex = (int)(start * originalClip.frequency * originalClip.channels);
            for (int i = 0; i < newSampleCount * originalClip.channels; i++)
            {
                newData[i] = data[startIndex + i] * m_Volume;
            }

            AudioClip clippedAudioClip = AudioClip.Create("ClippedAudio", newSampleCount, originalClip.channels, originalClip.frequency, false);
            clippedAudioClip.SetData(newData, 0);

            return clippedAudioClip;
        }
        #endregion
    }
}
#endif
