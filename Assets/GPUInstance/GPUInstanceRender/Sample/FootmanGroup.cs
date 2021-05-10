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

    public FootmanGroup(int count, Mesh mesh, Material material, AnimDataInfo animDataInfo) : base(count, mesh, material, animDataInfo)
    {
        FootmanCellItem.SetAnimData(animDataInfo);
        for (int x = 0; x < 70; x++)
        {
            for (int y = 0; y < 70; y++)
            {
                FootmanCellItem item = new FootmanCellItem();
                item.Play("Attack01", false);
                item.pos = new Vector3(x * 2, 0, y * 2);
                item.rotation = Quaternion.identity;
                AddCellItem(item);
            }
        }
    }

    protected override GPUInstanceCell OnCreateCell()
    {
        return new FootmanCell(this);
    }

}
