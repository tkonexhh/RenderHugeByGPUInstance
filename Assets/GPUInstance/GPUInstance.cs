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
    Matrix4x4[] matrices = new Matrix4x4[Length * Length];
    float[] animRate = new float[Length * Length];

    private const int Length = 30;

    [SerializeField] private List<SingleItem> m_SingleItems = new List<SingleItem>();
    static int AnimRateID = Shader.PropertyToID("_AnimRate");

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
                item.Play("Walk", true);
                //Run
                //Attack01
                //Attack02
                //Victory
                //Walk
                // item.animRate = Random.Range(0.0f, 1.0f);
                m_SingleItems.Add(item);
                animRate[i] = item.animRate;
                i++;
            }
        }

    }


    private void Update()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
            // block.SetVectorArray(baseColorID, baseColors);
        }

        for (int i = 0; i < m_SingleItems.Count; i++)
        {
            matrices[i] = Matrix4x4.TRS(m_SingleItems[i].pos, m_SingleItems[i].rotation, Vector3.one);
            m_SingleItems[i].Update();
            animRate[i] = m_SingleItems[i].animRate;
            // animRate[i] = animRateCtrl;
        }

        block.SetFloatArray(AnimRateID, animRate);

        Graphics.DrawMeshInstanced(m_Mesh, 0, material, matrices, matrices.Length, block);
    }

}
