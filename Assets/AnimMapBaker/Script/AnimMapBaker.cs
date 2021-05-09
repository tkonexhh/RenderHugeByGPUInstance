/*
 * 用来烘焙动作贴图。烘焙对象使用animation组件，并且在导入时设置Rig为Legacy
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

/// <summary>
/// 保存需要烘焙的动画的相关数据
/// </summary>
public struct AnimData
{
    #region FIELDS

    private int _vertexCount;
    private int _mapWidth;
    private readonly List<AnimationState> _animClips;
    private string _name;

    private Animation _animation;
    private SkinnedMeshRenderer _skin;

    public List<AnimationState> AnimationClips => _animClips;
    public int MapWidth => _mapWidth;
    public string Name => _name;

    #endregion

    public AnimData(Animation anim, SkinnedMeshRenderer smr, string goName)
    {
        _vertexCount = smr.sharedMesh.vertexCount;
        _mapWidth = Mathf.NextPowerOfTwo(_vertexCount);
        _animClips = new List<AnimationState>(anim.Cast<AnimationState>());
        _animation = anim;
        _skin = smr;
        _name = goName;
    }

    #region METHODS

    public void AnimationPlay(string animName)
    {
        _animation.Play(animName);
    }

    public void SampleAnimAndBakeMesh(ref Mesh m)
    {
        SampleAnim();
        BakeMesh(ref m);
    }

    private void SampleAnim()
    {
        if (_animation == null)
        {
            Debug.LogError("animation is null!!");
            return;
        }

        _animation.Sample();
    }

    private void BakeMesh(ref Mesh m)
    {
        if (_skin == null)
        {
            Debug.LogError("skin is null!!");
            return;
        }

        _skin.BakeMesh(m);
    }


    #endregion

}

/// <summary>
/// 烘焙后的数据
/// </summary>
public struct BakedData
{
    #region FIELDS

    private readonly string _name;
    // private readonly float _animLen;
    private readonly byte[] _rawAnimMap;
    private readonly int _animMapWidth;
    private readonly int _animMapHeight;
    private readonly string _jsonInfo;

    #endregion

    public BakedData(string name, Texture2D animMap, string jsonInfo)
    {
        _name = name;
        // _animLen = animLen;
        _animMapHeight = animMap.height;
        _animMapWidth = animMap.width;
        _rawAnimMap = animMap.GetRawTextureData();
        _jsonInfo = jsonInfo;
    }

    public int AnimMapWidth => _animMapWidth;

    public string Name => _name;

    // public float AnimLen => _animLen;

    public byte[] RawAnimMap => _rawAnimMap;

    public int AnimMapHeight => _animMapHeight;
    public string JsonInfo => _jsonInfo;
}

/// <summary>
/// 烘焙器
/// </summary>
public class AnimMapBaker
{

    #region FIELDS

    private AnimData? _animData = null;
    private Mesh _bakedMesh;
    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<BakedData> _bakedDataList = new List<BakedData>();

    #endregion

    #region METHODS

    public void SetAnimData(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("go is null!!");
            return;
        }

        var anim = go.GetComponent<Animation>();
        var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();

        if (anim == null || smr == null)
        {
            Debug.LogError("anim or smr is null!!");
            return;
        }
        _bakedMesh = new Mesh();
        _animData = new AnimData(anim, smr, go.name);
    }

    public List<BakedData> Bake()
    {
        if (_animData == null)
        {
            Debug.LogError("bake data is null!!");
            return _bakedDataList;
        }

        int totalHeight = 0;
        AnimDataInfo animDataInfo = new AnimDataInfo();
        //所有动作生成在一个动作图上面
        for (int i = 0; i < _animData.Value.AnimationClips.Count; i++)
        {
            var animationState = _animData.Value.AnimationClips[i];

            if (!animationState.clip.legacy)//因为是顶点动画所以只能是legacy
            {
                Debug.LogError(string.Format($"{animationState.clip.name} is not legacy!!"));
                continue;
            }
            int startHeight = totalHeight;
            int frameHeight = Mathf.ClosestPowerOfTwo((int)(animationState.clip.frameRate * animationState.length));//得到动画总帧数
            totalHeight += frameHeight;

            AnimMapClip animMapClip = new AnimMapClip();
            animMapClip.startHeight = startHeight;
            animMapClip.height = frameHeight;
            animMapClip.animLen = animationState.clip.length;
            animMapClip.name = animationState.name;
            animDataInfo.animMapClips.Add(animMapClip);
        }

        // totalHeight = Mathf.NextPowerOfTwo(totalHeight);
        animDataInfo.maxHeight = totalHeight;
        var animMap = new Texture2D(_animData.Value.MapWidth, totalHeight, TextureFormat.RGBAHalf, true);
        animMap.name = string.Format($"{_animData.Value.Name}.animMap");

        Debug.LogError(totalHeight);

        for (int i = 0; i < animDataInfo.animMapClips.Count; i++)
        {
            BakePerAnimClip(_animData.Value.AnimationClips[i], ref animMap, animDataInfo.animMapClips[i]);
        }
        animMap.Apply();
        //在生成一个动画信息文本
        var animInfoJson = JsonUtility.ToJson(animDataInfo);
        Debug.LogError(animInfoJson);

        _bakedDataList.Add(new BakedData(animMap.name, animMap, animInfoJson));
        return _bakedDataList;
    }

    private void BakePerAnimClip(AnimationState curAnim, ref Texture2D texture, AnimMapClip animMapClip)
    {
        float sampleTime = 0;
        float perFrameTime = 0;
        perFrameTime = curAnim.length / animMapClip.height;//得到单位时间的帧数
        _animData.Value.AnimationPlay(curAnim.name);

        for (int i = animMapClip.startHeight; i < animMapClip.startHeight + animMapClip.height; i++)
        {
            curAnim.time = sampleTime;

            _animData.Value.SampleAnimAndBakeMesh(ref _bakedMesh);
            for (int j = 0; j < _bakedMesh.vertexCount; j++)
            {
                var vertex = _bakedMesh.vertices[j];
                texture.SetPixel(j, i, new Color(vertex.x, vertex.y, vertex.z));
            }
            sampleTime += perFrameTime;
        }
        // texture.Apply();
    }
    #endregion


}

[System.Serializable]
public class AnimDataInfo
{
    public int maxHeight;
    public List<AnimMapClip> animMapClips = new List<AnimMapClip>();
}

[System.Serializable]
public struct AnimMapClip
{
    //起始高度
    public int startHeight;
    public int height;
    public string name;
    public float animLen;
}
