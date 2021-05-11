using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFrame.GPUInstance;
public class FootmanCell : GPUInstanceCell
{
    CapacityProp<float> animRates1;
    CapacityProp<float> animRates2;
    CapacityProp<float> animLerp;

    public FootmanCell(GPUInstanceGroup group) : base(group)
    {
    }

    protected override void OnCellInit()
    {
        animRates1 = new CapacityProp<float>(m_Capacity);
        animRates2 = new CapacityProp<float>(m_Capacity);
        animLerp = new CapacityProp<float>(m_Capacity);
    }
    protected override void OnCapacityChange()
    {
        animRates1.Expansion(m_Size, m_Capacity);
        animRates2.Expansion(m_Size, m_Capacity);
        animLerp.Expansion(m_Size, m_Capacity);
    }

    protected override void OnDraw()
    {

        for (int i = 0; i < m_Size; i++)
        {
            var cell = m_Items[i] as FootmanCellItem;
            if (cell != null)
            {
                cell.Update();
                m_TRSMatrices[i] = Matrix4x4.TRS(m_Items[i].pos, m_Items[i].rotation, Vector3.one);
                animRates1[i] = cell.animRate1;
                animRates2[i] = cell.animRate2;
                animLerp[i] = cell.animLerp;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            (m_Group as FootmanGroup).CrossFade();
        }

        m_MatPropBlock.SetFloatArray(FootmanGroup.AnimRate1ID, animRates1.array);
        m_MatPropBlock.SetFloatArray(FootmanGroup.AnimRate2ID, animRates2.array);
        m_MatPropBlock.SetFloatArray(FootmanGroup.AnimLerpID, animLerp.array);

    }
}
