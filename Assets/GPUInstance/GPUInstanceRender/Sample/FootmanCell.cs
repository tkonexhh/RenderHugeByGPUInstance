using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFrame.GPUInstance;
public class FootmanCell : GPUInstanceCell
{
    CapacityProp<float> animRates;
    CapacityProp<float> animLens;
    CapacityProp<float> animStarts;
    CapacityProp<float> animEnds;

    public FootmanCell(GPUInstanceGroup group) : base(group)
    {
    }

    protected override void OnCellInit()
    {
        animRates = new CapacityProp<float>(m_Capacity);
        animLens = new CapacityProp<float>(m_Capacity);
        animStarts = new CapacityProp<float>(m_Capacity);
        animEnds = new CapacityProp<float>(m_Capacity);
    }
    protected override void OnCapacityChange()
    {
        animRates.Expansion(m_Size, m_Capacity);
        animLens.Expansion(m_Size, m_Capacity);
        animStarts.Expansion(m_Size, m_Capacity);
        animEnds.Expansion(m_Size, m_Capacity);
    }

    protected override void OnDraw()
    {

        for (int i = 0; i < m_Size; i++)
        {
            var cell = m_Items[i] as FootmanCellItem;
            cell.Update();
            m_TRSMatrices[i] = Matrix4x4.TRS(m_Items[i].pos, m_Items[i].rotation, Vector3.one);
            animRates[i] = cell.animRate;
            animLens[i] = cell.animLen;
            animStarts[i] = cell.animStartRate;
            animEnds[i] = cell.animEndRate;
        }

        m_MatPropBlock.SetFloatArray(FootmanGroup.AnimRateID, animRates.array);
        m_MatPropBlock.SetFloatArray(FootmanGroup.AnimLenID, animLens.array);
        m_MatPropBlock.SetFloatArray(FootmanGroup.AnimStartRateID, animStarts.array);
        m_MatPropBlock.SetFloatArray(FootmanGroup.AnimEndRateID, animEnds.array);
    }
}
