using System;
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
        private int m_DrawCapacity;
        protected List<GPUInstanceCell> m_Cells;


        public GPUInstanceGroup(Mesh mesh, Material material, AnimDataInfo animDataInfo)
        {
            m_DrawMesh = mesh;
            m_Mat = material;
            m_AnimDataInfo = animDataInfo;
            m_Cells = new List<GPUInstanceCell>();
            // CreateCell();
        }

        public void AddCellItem(GPUInstanceCellItem cellItem)
        {
            for (int i = 0; i < m_Cells.Count; i++)
            {
                int index = m_Cells[i].IndexOf(null);
                if (index != -1)
                {
                    m_Cells[i].Set(index, cellItem);
                    if (cellItem != null)
                        cellItem.cellIndex = m_Cells[i].CellIndex;
                    // Debug.LogError("找到null，直接set:" + m_Cells[i].CellIndex);
                    return;
                }
            }

            GPUInstanceCell cell;
            //寻找合适的
            if (m_DrawCount + 1 >= m_DrawCapacity - m_Cells.Count + 1)
            {
                //创建新的Cell
                cell = CreateCell();
            }
            else
            {
                cell = m_Cells[m_Cells.Count - 1];

            }
            if (cellItem != null)
            {
                cellItem.cellIndex = cell.CellIndex;
                // Debug.LogError(cellItem.cellIndex);
            }

            cell.Add(cellItem);
            m_DrawCount++;
        }

        public bool RemoveCellItem(GPUInstanceCellItem cellItem)
        {
            if (cellItem == null)
            {
                return true;
            }

            int cellIndex = cellItem.cellIndex;
            int index = m_Cells[cellIndex].IndexOf(cellItem);
            if (index == -1)
                return false;
            // Debug.LogError("Remove:" + index);
            m_Cells[cellIndex].Set(index, null);
            // m_DrawCount--;
            return true;
        }

        public GPUInstanceCell CreateCell()
        {
            m_DrawCapacity = GPUInstanceDefine.MAX_CAPACITY * (m_Cells.Count + 1);
            GPUInstanceCell cell = OnCreateCell();
            cell.CellIndex = m_Cells.Count;
            // cell.OnCellInit();
            // OnCellAdd(cell);
            m_Cells.Add(cell);
            return cell;
        }

        public void Draw()
        {
            for (int i = 0; i < m_Cells.Count; i++)
            {
                m_Cells[i].Draw();
            }
        }

        #region override
        // protected virtual void OnCellAdd(GPUInstanceCell cell) { }
        protected virtual GPUInstanceCell OnCreateCell()
        {
            return new GPUInstanceCell(this);
        }
        #endregion

    }
}