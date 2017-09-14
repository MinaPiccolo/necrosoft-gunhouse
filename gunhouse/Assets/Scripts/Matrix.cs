using UnityEngine;
public struct MatrixType
{
    public float[][] matrix;
    public float this[int x, int y]
    {
        get { return matrix[x][y]; }
        set { matrix[x][y] = value; }
    }
};

public static class Matrix
{
    public static int size = 3;

    static MatrixType operations = new MatrixType { matrix = new float[][] { new float[size], new float[size], new float[size], } };
    static MatrixType multiply = new MatrixType { matrix = new float[][] { new float[size], new float[size], new float[size] } };

    public static MatrixType Identity(MatrixType matrix)
    {
        for (int x = 0; x < size; ++x) {
            matrix[x, 0] = (x == 0) ? 1 : 0;
            matrix[x, 1] = (x == 1) ? 1 : 0;
            matrix[x, 2] = (x == 2) ? 1 : 0;
        }
        return matrix;
    }

    public static MatrixType Multiply(MatrixType a, MatrixType b)
    {
        for (int x = 0; x < size; ++x) {
            multiply[x, 0] = a[x, 0];
            multiply[x, 1] = a[x, 1];
            multiply[x, 2] = a[x, 2];
        }

        for (int x = 0; x < 2; ++x) {           // 2x3
            for (int y = 0; y < size; ++y) {
                float sum = 0;
                sum += multiply[0, y] * b[x, 0];
                sum += multiply[1, y] * b[x, 1];
                sum += multiply[2, y] * b[x, 2];
                a[x, y] = sum;
            }
        }
        return a;
    }

    public static MatrixType Scale(MatrixType matrix, Vector2 scale)
    {
        operations = Identity(operations);
        operations[0, 0] = scale.x;
        operations[1, 1] = scale.y;

        return Multiply(matrix, operations);
    }

    public static MatrixType Rotate(MatrixType matrix, float amount)
    {
        operations = Identity(operations);
        operations[0, 0] = (float)System.Math.Cos(amount);
        operations[1, 0] = (float)System.Math.Sin(amount);
        operations[0, 1] = -operations[1, 0];
        operations[1, 1] = operations[0, 0];

        return Multiply(matrix, operations);
    }

    public static MatrixType Translation(MatrixType matrix, Vector2 translate)
    {
        operations = Identity(operations);
        operations[0, 2] = translate.x;
        operations[1, 2] = translate.y;

        return Multiply(matrix, operations);
    }

    public static Vector2 Apply(MatrixType matrix, Vector2 vector)
    {
        return new Vector2(vector.x * matrix[0, 0] + vector.y * matrix[0, 1] + matrix[0, 2],
                           vector.x * matrix[1, 0] + vector.y * matrix[1, 1] + matrix[1, 2]);
    }
}
