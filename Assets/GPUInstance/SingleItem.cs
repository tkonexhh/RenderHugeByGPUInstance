using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SingleItem
{
    public Vector3 pos;
    public Quaternion rotation;
    public float animRate = 0;
    // public float animLen = 1;
    public float animStartRate = 0;
    public float animEndRate = 0;


    private bool m_Playing = false;
    private bool m_Looping = false;

    private bool m_Fading = false;//动画融合


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

    public SingleItem(int x, int y)
    {
        pos = new Vector3(x * 2, 0, y * 2);
        rotation = Quaternion.identity;
    }

    public void Update()
    {
        if (m_Playing)
        {
            animRate += Time.deltaTime;
            if (animRate >= 0.99f)
            {
                if (m_Looping)
                {
                    animRate = 0f;
                }
                else
                {
                    m_Playing = false;
                    PlayRandomAnim();
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
        animRate = 0;
        var animMapClip = s_AnimDataMap[animName];
        animStartRate = (float)animMapClip.startHeight / (float)s_AnimDataInfo.maxHeight;
        float totalRate = (float)animMapClip.height / (float)s_AnimDataInfo.maxHeight;
        animEndRate = animStartRate + totalRate;
        m_Playing = true;
        m_Looping = loop;

        // animLen = animMapClip.animLen;
        // Debug.LogError(animStartRate + "--" + animEndRate + "-" + animLen);
    }

    public void CrossFade(string animName, float fadeTime = 0f, bool loop = false)
    {
        if (!s_AnimDataMap.ContainsKey(animName))
        {
            Debug.LogError("AnimName not Fount:" + animName);
            return;
        }

        if (fadeTime > 0f)
        {
            m_Fading = true;
        }
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
