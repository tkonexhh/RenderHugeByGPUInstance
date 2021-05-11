﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFrame.GPUInstance
{
    public class GPUInstanceCell//:IEnumerator
    {
        // public OnCellDraw onCellDraw;

        protected GPUInstanceCellItem[] m_Items;
        protected Matrix4x4[] m_TRSMatrices;
        protected MaterialPropertyBlock m_MatPropBlock;
        protected GPUInstanceGroup m_Group;

        protected int m_Size = 0;
        protected int m_Capacity = 0;//Actual Arrat size


        public int CellIndex { set; get; }
        public int Size => m_Size;
        public bool isEmpty => Size == 0;


        // //==static
        // public static GPUInstanceCell CreateCell(GPUInstanceGroup group)
        // {
        //     return new GPUInstanceCell(group);
        // }
        // //==

        public GPUInstanceCell(GPUInstanceGroup group) : this(group, 64)
        {
        }

        public GPUInstanceCell(GPUInstanceGroup group, int capacity)
        {
            if (capacity < 0) throw new ArgumentException("Illegal Capacity: " + capacity);
            m_Group = group;
            m_Capacity = capacity;
            m_Items = new GPUInstanceCellItem[m_Capacity];
            m_TRSMatrices = new Matrix4x4[m_Capacity];
            m_MatPropBlock = new MaterialPropertyBlock();
            OnCellInit();
        }



        #region  Dynamic Array
        public void Clear()
        {
            for (int i = 0; i < m_Size; i++)
            {
                m_Items[i] = null;
            }

            m_Size = 0;
        }

        public GPUInstanceCellItem Get(int index)
        {
            if (index >= m_Size || index < 0)
                throw new System.IndexOutOfRangeException();

            return m_Items[index];
        }

        public void Set(int index, GPUInstanceCellItem element)
        {
            if (index >= m_Size || index < 0)
                throw new System.IndexOutOfRangeException();

            m_Items[index] = element;
        }

        public void Add(GPUInstanceCellItem element)
        {

            if (m_Size + 1 >= GPUInstanceDefine.MAX_CAPACITY)
            {
                Debug.LogError("MAX_CAPACITY Cant add new element :" + element.cellIndex);
                // return false;
            }
            if (m_Size + 1 >= m_Capacity) //注意扩容的消耗
            {
                // Debug.LogError("Try to double capacity");
                if (m_Capacity == 0)
                    m_Capacity = 1;
                else
                    m_Capacity *= 2;

                // Debug.LogError(m_Capacity);
                m_Capacity = Mathf.Min(GPUInstanceDefine.MAX_CAPACITY, m_Capacity);

                GPUInstanceCellItem[] new_arr = new GPUInstanceCellItem[m_Capacity];
                for (int i = 0; i < m_Size; i++)
                {
                    new_arr[i] = m_Items[i];
                }
                m_Items = new_arr;

                Matrix4x4[] new_matrixArr = new Matrix4x4[m_Capacity];
                for (int i = 0; i < m_Size; i++)
                {
                    new_matrixArr[i] = m_TRSMatrices[i];
                }
                m_TRSMatrices = new_matrixArr;
                OnCapacityChange();
            }

            m_Items[m_Size++] = element;
            // return true;
        }

        [Obsolete("消耗较大，慎用")]
        public GPUInstanceCellItem RemoveAt(int index)
        {
            if (index >= m_Size || index < 0)
                throw new IndexOutOfRangeException();
            GPUInstanceCellItem cellItem = m_Items[index];

            GPUInstanceCellItem[] new_arr = new GPUInstanceCellItem[m_Size - 1];
            for (int i = 0, j = 0; i < m_Size; i++, j++)
                if (i == index) j--; // Skip over rm_index by fixing j temporarily
                else new_arr[j] = m_Items[i];

            m_Items = new_arr;
            m_Capacity = --m_Size;

            return cellItem;
        }

        [Obsolete("消耗较大，慎用")]
        public bool Remove(GPUInstanceCellItem element)
        {
            int index = IndexOf(element);
            if (index == -1) return false;
            RemoveAt(index);
            return true;
        }

        public int IndexOf(GPUInstanceCellItem element)
        {
            for (int i = 0; i < m_Size; i++)
            {
                if (element == null)
                {
                    if (m_Items[i] == null)
                        return i;
                }
                else
                {
                    if (element.Equals(m_Items[i]))
                        return i;
                }
            }
            return -1;
        }

        #endregion

        #region 
        public void Draw()
        {
            OnDraw();
            Graphics.DrawMeshInstanced(m_Group.drawMesh, 0, m_Group.material, m_TRSMatrices, m_Size, m_MatPropBlock);
        }

        protected virtual void OnDraw()
        {
            for (int i = 0; i < m_Size; i++)
            {
                m_TRSMatrices[i] = Matrix4x4.TRS(m_Items[i].pos, m_Items[i].rotation, Vector3.one);
            }

            // if (onCellDraw != null)
            // {
            //     onCellDraw(m_MatPropBlock);
            // }
        }

        #endregion

        #region override
        protected virtual void OnCellInit() { }
        protected virtual void OnCapacityChange() { }

        #endregion
    }

    public class CapacityProp<T> //where T : struct
    {
        private T[] arr;

        public T[] array => arr;

        public CapacityProp(int capacity)
        {
            arr = new T[capacity];
        }

        public T this[int index]
        {
            get
            {
                return arr[index];
            }
            set
            {
                arr[index] = value;
            }
        }

        /// <summary>
        /// 扩容
        /// </summary>
        public void Expansion(int nowSize, int newSize)
        {
            T[] new_Arr = new T[newSize];
            for (int i = 0; i < nowSize; i++)
            {
                new_Arr[i] = arr[i];
            }

            arr = new_Arr;
        }
    }
}