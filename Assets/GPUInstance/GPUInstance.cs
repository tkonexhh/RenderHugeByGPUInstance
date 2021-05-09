using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstance : MonoBehaviour
{
    [SerializeField] private Mesh m_Mesh;
    [SerializeField] private Material material;
    [SerializeField] private TextAsset animInfoText;

    [SerializeField, Range(0.0f, 1.0f)] private float animRateCtrl;

    MaterialPropertyBlock block;
    private const int MAXCOUNT = 1024;
    private const int Length = 30;
    Matrix4x4[] matrices = new Matrix4x4[Length * Length];
    float[] animRates = new float[Length * Length];
    float[] animLens = new float[Length * Length];
    float[] animStarts = new float[Length * Length];
    float[] animEnds = new float[Length * Length];



    [SerializeField] private List<SingleItem> m_SingleItems = new List<SingleItem>();
    static int AnimRateID = Shader.PropertyToID("_AnimRate");
    static int AnimLenID = Shader.PropertyToID("_AnimLen");
    static int AnimStartRateID = Shader.PropertyToID("_AnimStartRate");
    static int AnimEndRateID = Shader.PropertyToID("_AnimEndRate");

    private void Awake()
    {
        AnimDataInfo animDataInfo = JsonUtility.FromJson<AnimDataInfo>(animInfoText.text);
        SingleItem.SetAnimData(animDataInfo);
        int i = 0;
        for (int x = 0; x < Length; x++)
        {
            for (int y = 0; y < Length; y++)
            {
                SingleItem item = new SingleItem(x, y);
                item.Play("Victory", true);
                //Run
                //Attack01
                //Attack02
                //Victory
                //Walk
                // item.animRate = Random.Range(0.0f, 1.0f);
                m_SingleItems.Add(item);
                animRates[i] = item.animRate;
                i++;
            }
        }

    }


    private void Update()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }

        for (int i = 0; i < m_SingleItems.Count; i++)
        {
            matrices[i] = Matrix4x4.TRS(m_SingleItems[i].pos, m_SingleItems[i].rotation, Vector3.one);
            m_SingleItems[i].Update();
            // animRates[i] = m_SingleItems[i].animRate;
            animLens[i] = m_SingleItems[i].animLen;
            animRates[i] = animRateCtrl;
            animStarts[i] = m_SingleItems[i].animStartRate;
            animEnds[i] = m_SingleItems[i].animEndRate;
        }

        block.SetFloatArray(AnimRateID, animRates);
        block.SetFloatArray(AnimLenID, animLens);
        block.SetFloatArray(AnimStartRateID, animStarts);
        block.SetFloatArray(AnimEndRateID, animEnds);

        Graphics.DrawMeshInstanced(m_Mesh, 0, material, matrices, matrices.Length, block);
    }

}
