using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlockGrid
{
    //public int Height;
    public int Width;
    public int NbVertices;
    public Vector3[] Vertices;

    public FlockGrid(int pawnNumber = 1, int pawnsPerRow = 5)
    {
        Width = pawnsPerRow;
        NbVertices = pawnNumber;
        //Height = Mathf.CeilToInt(pawns.Count() / pawnsPerRow);

        Generate();
    }

    //public FlockGrid(int width = 5, int height = 1)
    //{
    //    Width = width;
    //    NbVertices = 
    //    Height = height;

    //    Generate();
    //}

    public void Generate()
    {
        if (NbVertices == 0 || Width == 0)
            return;

        Vertices = new Vector3[NbVertices];

        //Array.Clear(Vertices, 0, Vertices.Length);

        //int NbVerticesMinusCommander = NbVertices - 1;

        //int height = Mathf.CeilToInt((float)NbVerticesMinusCommander / (float)Width);

        for (int row = 0; row < GetHeight(); row++)
        {
            int nbCol = (NbVertices - 1) - row * Width;

            if (nbCol > Width)
                nbCol = Width;


            for (int col = 0; col < nbCol; col++)
            {
                float xCentered = col - (float)nbCol / 2 + 0.5f;
                float zCentered = row - GetHeight() / 2;
                Vector3 pos = new(xCentered, 0, zCentered);
                //Vector3 pos = new(xCentered, 0, -row);
                Vertices[Width * row + col] = pos;
            }
        }
    }




    public float GetRadius()
    {
        float height = GetHeight();
        float z =  -height / 2;
        Vector3 centerPoint = Vector3.forward * z;

        return Vector3.Distance(centerPoint, Vector3.forward);
    }




    public int GetHeight()
    {
        float NbVerticesMinusCommander = NbVertices - 1;

        // The casts are important, otherwise for example Mathf.CeilToInt(14 / 5) would return 2 and not 3
        return Mathf.CeilToInt(NbVerticesMinusCommander / (float)Width);
    }





    public Vector3[] GetOffsetedVertices(Transform pivot)
    {
        Vector3[] result = new Vector3[Vertices.Length];

        for (int i = 0; i < Vertices.Length; i++)
        {
            result[i] = pivot.rotation * Vertices[i] + pivot.position;
        }

        return result;
    }


    public Vector3[] GetOffsetedVertices(Quaternion rotation, Vector3 offset)
    {
        Vector3[] result = new Vector3[Vertices.Length];

        for (int i = 0; i < Vertices.Length; i++)
        {
            result[i] = rotation * Vertices[i] + offset;
        }

        return result;
    }





    public Vector3[] RotateBy(Quaternion rotation)
    {
        Vector3[] result = new Vector3[Vertices.Length];

        for (int i = 0; i < Vertices.Length; i++)
        {
            result[i] = rotation * Vertices[i];
        }

        return result;
    }





    public Vector3[] OffsetBehind(Transform pivot)
    {
        Vector3[] result = new Vector3[Vertices.Length];

        for (int i = 0; i < Vertices.Length; i++)
        {
            result[i] = pivot.rotation * Vertices[i] + pivot.position - pivot.forward;
        }

        return result;
    }
}
