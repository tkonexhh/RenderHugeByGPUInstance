using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFrame.GPUInstance
{
    public class GPUInstanceGroup
    {
        private Mesh m_DrawMesh;
        private Material m_Mat;
        private AnimDataInfo m_AnimDataInfo;//文本中的信息

        public Mesh drawMesh => m_DrawMesh;
        public Material material => m_Mat;

        private int m_DrawCount;
        protected List<GPUInstanceCell> m_Cells;


        public GPUInstanceGroup(int count, Mesh mesh, Material material, AnimDataInfo animDataInfo)
        {
            m_DrawMesh = mesh;
            m_Mat = material;
            m_AnimDataInfo = animDataInfo;

            m_DrawCount = count;
            m_Cells = new List<GPUInstanceCell>();
            // CreateCell();
        }

        public void AddCell(GPUInstanceCell cell)
        {
            OnCellAdd(cell);
            m_Cells.Add(cell);
        }

        // public void CreateCell()
        // {
        //     GPUInstanceCell cell = new GPUInstanceCell(this);
        //     // cell.OnCellInit();
        //     OnCellAdd(cell);
        //     m_Cells.Add(cell);
        // }

        public void Draw()
        {
            for (int i = 0; i < m_Cells.Count; i++)
            {
                m_Cells[i].Draw();
            }
        }

        #region override
        protected virtual void OnCellAdd(GPUInstanceCell cell) { }
        #endregion

    }
}