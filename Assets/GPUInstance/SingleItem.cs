using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SingleItem
{
    public Vector3 pos;
    public Quaternion rotation;
    public float animRate = 0;
    public float animLen = 1;
    public float animStartRate = 0;
    public float animEndRate = 0;

    private float m_StartRate;
    private float m_EndRate;


    private bool m_Playing = false;
    private bool m_Looping = false;


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
            if (animRate >= m_EndRate)
            {
                if (m_Looping)
                {
                    animRate = m_StartRate;
                }
                else
                {
                    m_Playing = false;
                    PlayRandomAnim();
                }


            }
            else
            {
                animRate += Time.deltaTime * 0.07f;
                // animRate = Mathf.Clamp(animRate, m_StartRate, m_EndRate);
            }
        }
        // Debug.LogError(animRate);
        // animRate += Time.deltaTime;
        // if (animRate >= 1)
        // {
        //     animRate = 0;
        // }
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
        int maxHeight = s_AnimDataInfo.maxHeight;
        var animMapClip = s_AnimDataMap[animName];
        int startHeight = animMapClip.startHeight;
        int height = animMapClip.height;
        m_StartRate = (float)startHeight / (float)maxHeight;

        // Debug.LogError(m_StartRate);
        float totalRate = (float)height / (float)maxHeight;
        m_EndRate = m_StartRate + totalRate;
        m_Playing = true;
        m_Looping = loop;
        animRate = m_StartRate;
        animLen = animMapClip.animLen;
        animStartRate = m_StartRate;
        animEndRate = m_EndRate;
        Debug.LogError(m_StartRate + "--" + m_EndRate + "-" + animLen);
    }
}
