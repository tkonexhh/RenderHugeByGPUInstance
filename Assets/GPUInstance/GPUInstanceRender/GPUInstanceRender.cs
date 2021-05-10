using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFrame.GPUInstance
{
    public class GPUInstanceRender : MonoBehaviour
    {
        [SerializeField] private Mesh m_Mesh;
        [SerializeField] private Material material;
        [SerializeField] private TextAsset animInfoText;
        List<GPUInstanceGroup> m_Groups;

        private void Awake()
        {
            m_Groups = new List<GPUInstanceGroup>();
            AnimDataInfo animDataInfo = JsonUtility.FromJson<AnimDataInfo>(animInfoText.text);
            GPUInstanceGroup group = new FootmanGroup(16, m_Mesh, material, animDataInfo);
            m_Groups.Add(group);
        }

        private void Update()
        {
            for (int i = 0; i < m_Groups.Count; i++)
            {
                m_Groups[i].Draw();
            }
        }
    }
}