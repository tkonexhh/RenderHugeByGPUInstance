using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFrame.GPUInstance;
public class FootmanGroup : GPUInstanceGroup
{
    public static int AnimRateID = Shader.PropertyToID("_AnimRate");
    public static int AnimLenID = Shader.PropertyToID("_AnimLen");
    public static int AnimStartRateID = Shader.PropertyToID("_AnimStartRate");
    public static int AnimEndRateID = Shader.PropertyToID("_AnimEndRate");

    public FootmanGroup(Mesh mesh, Material material, AnimDataInfo animDataInfo) : base(mesh, material, animDataInfo)
    {
        FootmanCellItem.SetAnimData(animDataInfo);
        // for (int i = 0; i < 100; i++)
        // {
        //     AddCellItem(null);
        // }
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                FootmanCellItem item = new FootmanCellItem();
                item.Play("Attack01", true);
                item.pos = new Vector3(x * 2, 0, y * 2);
                item.rotation = Quaternion.identity;
                AddCellItem(item);
            }
        }

        // for (int i = 0; i < 675; i++)
        // {

        //     RemoveCellItem(m_Cells[0].Get(i));
        // }
    }

    protected override GPUInstanceCell OnCreateCell()
    {
        return new FootmanCell(this);
    }

}
