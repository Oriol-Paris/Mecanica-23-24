using System;
using System.Collections;
using System.Collections.Generic;

public class MatrixC
{
    #region FIELDS
    public float[,] nodes;
    public int matrixSize;
    #endregion

    #region PROPIERTIES
    public static MatrixC identityMatrixSize3
    {
        get
        {
            return new MatrixC(new float[,] { { 1.0f, 0.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 0.0f, 1.0f } });
        }
    }
    public static MatrixC identityMatrixSize4
    {
        get
        {
            return new MatrixC(new float[,] 
            { 
                { 1.0f, 0.0f, 0.0f, 0.0f }, { 0.0f, 1.0f, 0.0f, 0.0f }, 
                { 0.0f, 0.0f, 1.0f, 0.0f }, { 0.0f, 0.0f, 0.0f, 1.0f } 
            });
        }
    }
    #endregion

    #region CONSTRUCTORS
    public MatrixC(float[,] nodes)
    {
        this.nodes = nodes;
        matrixSize = (int)Math.Sqrt(nodes.Length);
    }
    #endregion

    #region OPERATORS
    public static MatrixC operator *(MatrixC m1, MatrixC m2)
    {
        if (m1.matrixSize == 3)
        {
            return new MatrixC(new float[,]
            {
                {
                    (m1.nodes[0, 0] * m2.nodes[0, 0]) + (m1.nodes[0, 1] * m2.nodes[1, 0]) + (m1.nodes[0, 2] * m2.nodes[2, 0]),
                    (m1.nodes[0, 0] * m2.nodes[0, 1]) + (m1.nodes[0, 1] * m2.nodes[1, 1]) + (m1.nodes[0, 2] * m2.nodes[2, 1]),
                    (m1.nodes[0, 0] * m2.nodes[0, 2]) + (m1.nodes[0, 1] * m2.nodes[1, 2]) + (m1.nodes[0, 2] * m2.nodes[2, 2]),
                },
                {
                    (m1.nodes[1, 0] * m2.nodes[0, 0]) + (m1.nodes[1, 1] * m2.nodes[1, 0]) + (m1.nodes[1, 2] * m2.nodes[2, 0]),
                    (m1.nodes[1, 0] * m2.nodes[0, 1]) + (m1.nodes[1, 1] * m2.nodes[1, 1]) + (m1.nodes[1, 2] * m2.nodes[2, 1]),
                    (m1.nodes[1, 0] * m2.nodes[0, 2]) + (m1.nodes[1, 1] * m2.nodes[1, 2]) + (m1.nodes[1, 2] * m2.nodes[2, 2]),
                },
                {
                    (m1.nodes[2, 0] * m2.nodes[0, 0]) + (m1.nodes[2, 1] * m2.nodes[1, 0]) + (m1.nodes[2, 2] * m2.nodes[2, 0]),
                    (m1.nodes[2, 0] * m2.nodes[0, 1]) + (m1.nodes[2, 1] * m2.nodes[1, 1]) + (m1.nodes[2, 2] * m2.nodes[2, 1]),
                    (m1.nodes[2, 0] * m2.nodes[0, 2]) + (m1.nodes[2, 1] * m2.nodes[1, 2]) + (m1.nodes[2, 2] * m2.nodes[2, 2]),
                }
            });
        }
        else
            return new MatrixC(new float[,] { });
    }

    public static Vector3C operator *(MatrixC matrix, Vector3C point)
    {
        return new Vector3C
        (
            (point.x * matrix.nodes[0, 0]) + (point.y * matrix.nodes[0, 1]) + (point.z * matrix.nodes[0, 2]),
            (point.x * matrix.nodes[1, 0]) + (point.y * matrix.nodes[1, 1]) + (point.z * matrix.nodes[1, 2]),
            (point.x * matrix.nodes[2, 0]) + (point.y * matrix.nodes[2, 1]) + (point.z * matrix.nodes[2, 2])
        );
    }

    public static MatrixC operator *(MatrixC m1, float product)
    {
        for (int i = 0; i < m1.matrixSize; i++)
        {
            for (int j = 0; j < m1.matrixSize; j++)
            {
                m1.nodes[i, j] *= product;
            }
        }
        return m1;
    }
    #endregion

    #region METHODS

    #endregion

    #region FUNCTIONS
    public static MatrixC RotateX3x3(float angle)
    {
        return new MatrixC(new float[,] 
        {
            { 1.0f, 0.0f, 0.0f },
            { 0.0f, MathF.Cos(angle), -MathF.Sin(angle) },
            { 0.0f, MathF.Sin(angle), MathF.Cos(angle) }
        });
    }

    public static MatrixC RotateY3x3(float angle)
    {
        return new MatrixC(new float[,]
        {
            { MathF.Cos(angle), 0.0f, MathF.Sin(angle) },
            { 0.0f, 1.0f, 0.0f },
            { -MathF.Sin(angle), 0.0f, MathF.Cos(angle) }
        });
    }

    public static MatrixC RotateZ3x3(float angle)
    {
        return new MatrixC(new float[,]
        {
            { MathF.Cos(angle), -MathF.Sin(angle), 0.0f },
            { MathF.Sin(angle), MathF.Cos(angle), 0.0f },
            { 0.0f, 0.0f, 1.0f }
        });
    }


    public static Vector3C RotationToVector(Vector3C euler, Vector3C point, Vector3C origin)
    {
        return RotateX3x3(euler.x) * (RotateY3x3(euler.y) * RotateZ3x3(euler.z)) * point;
    }
    #endregion
}
