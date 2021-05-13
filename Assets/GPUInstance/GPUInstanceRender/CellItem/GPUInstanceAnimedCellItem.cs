using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFrame.GPUInstance
{
    public class GPUInstanceAnimedCellItem : GPUInstanceCellItem
    {
        private class Animation
        {
            public float animStartRate { set; get; }
            public float animEndRate { set; get; }
            public float animRate { set; get; }

            private float m_Speed;

            public void InitTime(AnimMapClip clip)
            {
                animStartRate = (float)clip.startHeight / (float)s_AnimDataInfo.maxHeight;
                float totalRate = (float)clip.height / (float)s_AnimDataInfo.maxHeight;
                animEndRate = animStartRate + totalRate;
                m_Speed = 1.0f / clip.animLen * (animEndRate - animStartRate);
                Replay();
            }

            public bool IsEnd()
            {
                // return animRate>=1;
                return animRate >= animEndRate;
            }

            public void Trick()
            {
                animRate += Time.deltaTime * m_Speed;
            }

            public void Replay()
            {
                // animRate=0;
                animRate = animStartRate;
            }

        }

        private bool m_Playing = false;
        private bool m_Looping = false;
        private bool m_Fading = false;
        private float m_FadeSpeed;

        private Animation m_Anim1 = new Animation();
        private Animation m_Anim2 = new Animation();//动画2,用于动画融合

        public float animRate1 => m_Anim1.animRate;
        public float animRate2 => m_Anim2.animRate;
        public float animLerp = 0;

        //==Static Block
        private static AnimDataInfo s_AnimDataInfo;
        private static Dictionary<string, AnimMapClip> s_AnimDataMap;
        public static void SetAnimData(AnimDataInfo animDataInfo)
        {
            s_AnimDataInfo = animDataInfo;
            s_AnimDataMap = new Dictionary<string, AnimMapClip>();
            foreach (var animData in s_AnimDataInfo.animMapClips)
            {
                s_AnimDataMap.Add(animData.name, animData);
            }
        }
        //==

        public void Update()
        {
            if (m_Playing)
            {
                if (!m_Fading)
                {
                    m_Anim1.Trick();
                    if (m_Anim1.IsEnd())
                    {
                        if (m_Looping)
                        {
                            m_Anim1.Replay();
                        }
                        else
                        {
                            m_Playing = false;
                            PlayRandomAnim();
                        }
                    }
                }
                else
                {
                    m_Anim1.Trick();
                    m_Anim2.Trick();
                    animLerp += Time.deltaTime * m_FadeSpeed;

                    if (m_Anim1.IsEnd())
                        m_Anim1.Replay();

                    if (m_Anim2.IsEnd())
                        m_Anim2.Replay();

                    if (animLerp >= 1.0f)
                    {
                        if (m_Looping)
                        {
                            m_Anim1.animStartRate = m_Anim2.animStartRate;
                            m_Anim1.animEndRate = m_Anim2.animEndRate;
                            m_Anim1.animRate = m_Anim2.animRate;

                            animLerp = 0;
                        }

                        m_Fading = false;

                    }
                }

            }
        }

        public void PlayRandomAnim()
        {
            var animMapClip = s_AnimDataInfo.animMapClips[Random.Range(0, s_AnimDataInfo.animMapClips.Count)];
            Play(animMapClip.name);
        }

        public void Play(string animName, bool loop = false)
        {
            if (!s_AnimDataMap.ContainsKey(animName))
            {
                Debug.LogError("AnimName not Fount:" + animName);
                return;
            }

            var animClip = s_AnimDataMap[animName];
            m_Anim1.InitTime(animClip);
            m_Playing = true;
            m_Looping = loop;
            m_Fading = false;
            animLerp = 0;
        }


        public void CrossFade(string animName, float duringTime, bool loop = false)
        {
            if (!s_AnimDataMap.ContainsKey(animName))
            {
                Debug.LogError("AnimName not Fount:" + animName);
                return;
            }


            var animClip = s_AnimDataMap[animName];
            if (m_Anim2 == null)
                m_Anim2 = new Animation();

            m_Anim2.InitTime(animClip);
            float fadeTime = duringTime;
            if (fadeTime >= animClip.animLen)
                fadeTime = animClip.animLen;

            m_FadeSpeed = 1.0f / fadeTime;

            m_Playing = true;
            m_Looping = loop;
            m_Fading = true;
            animLerp = 0;
        }

        public void Pause()
        {
            m_Playing = false;
        }

        public void Resume()
        {
            m_Playing = true;
        }
    }
}