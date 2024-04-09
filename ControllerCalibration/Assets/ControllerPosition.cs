using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Matrix4x4 = System.Numerics.Matrix4x4;
using NumVector3 = System.Numerics.Vector3;
using Vector3 = UnityEngine.Vector3;


public class ControllerPosition : MonoBehaviour
{
    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text text4;

    public GameObject rightController;
    public InputActionReference iar;
    public InputActionReference calculateAction;

    private Vector3 firstPosition = Vector3.zero;
    private Vector3 secondPosition = Vector3.zero;
    private Vector3 thirdPosition = Vector3.zero;

    private Vector3 firstRotation = Vector3.zero;
    private Vector3 secondRotation = Vector3.zero;
    private Vector3 thirdRotation = Vector3.zero;

    public GameObject circle;
    public GameObject intersectionCircle;
    // Start is called before the first frame update
    void Start()
    {
        iar.action.performed += RegisterPosition;
        calculateAction.action.performed += IntersectionAction;
    }

    private void RegisterPosition(InputAction.CallbackContext obj)
    {
        thirdPosition = secondPosition;
        secondPosition = firstPosition;
        firstPosition = rightController.transform.position;

        thirdRotation = secondRotation;
        secondRotation = firstRotation;
        firstRotation = rightController.transform.rotation.eulerAngles;


        text1.SetText($"x: {firstPosition.x:F4}\ny: {firstPosition.y:F4}\nz: {firstPosition.z:F4}\npitch: {firstRotation.x}\nyaw: {firstRotation.y}\nroll: {firstRotation.z}");
        text2.SetText($"x: {secondPosition.x:F4}\ny: {secondPosition.y:F4}\nz: {secondPosition.z:F4}\npitch: {secondRotation.x}\nyaw: {secondRotation.y}\nroll: {secondRotation.z}");
        text3.SetText($"x: {thirdPosition.x:F4}\ny: {thirdPosition.y:F4}\nz: {thirdPosition.z:F4}\npitch: {thirdRotation.x}\nyaw: {thirdRotation.y}\nroll: {thirdRotation.z}");


        Vector3 p1Angles = EulerToVector(firstRotation.z, firstRotation.x, firstRotation.y);
        Vector3 circlePos = firstPosition + p1Angles;
        circle.transform.position = circlePos;

    }

    private void IntersectionAction(InputAction.CallbackContext obj)
    {
        Vector3 intersection = CalculateIntersection();
        text4.SetText($"x: {intersection.x:F4}\ny: {intersection.y:F4}\nz: {intersection.z:F4}");
        intersectionCircle.transform.position = intersection;

    }

    private Vector3 CalculateIntersection()
    {
        Vector3 p1Angles = EulerToVector(firstRotation.z, firstRotation.x, firstRotation.y);
        Vector3 p2Angles = EulerToVector(secondRotation.z, secondRotation.x, secondRotation.y);
        Vector3 p3Angles = EulerToVector(thirdPosition.z, thirdPosition.x, thirdPosition.y);
        return CalculateIntersection(firstPosition, p1Angles, secondPosition, p2Angles, thirdPosition, p3Angles);
        // Vector3 a1 = new Vector3(0, 0, 0);
        // Vector3 d1 = new Vector3(1, 1, 0);
        // Vector3 a2 = new Vector3(2, 0, 0);
        // Vector3 d2 = new Vector3(1,-1, 0);
        // Vector3 a3 = new Vector3(1, 0, 0);
        // Vector3 d3 = new Vector3(0, 1, 0);
        // return CalculateIntersection(a1,d1,a2,d2,a3,d3);
    }

    public Vector3 EulerToVector(float roll, float pitch, float yaw)
    {
        // Convert angles to radians
        roll = roll * Mathf.Deg2Rad;
        yaw = -1*(yaw-90) * Mathf.Deg2Rad;
        pitch = pitch * Mathf.Deg2Rad;

        // Calculate the direction vector
        float x = Mathf.Cos(pitch) * Mathf.Cos(yaw);
        float y = -1*Mathf.Sin(pitch);
        float z = Mathf.Cos(pitch) * Mathf.Sin(yaw);

        return new Vector3(x, y, z);
    }

    private Vector3 CalculateIntersection(Vector3 a1, Vector3 d1, Vector3 a2, Vector3 d2, Vector3 a3, Vector3 d3)
    {
        // https://math.stackexchange.com/questions/61719/finding-the-intersection-point-of-many-lines-in-3d-point-closest-to-all-lines
        // Build a matrix representing system of equations for least square approximation of intersection point.
        double SXX = 3 - (d1.x * d1.x) / d1.sqrMagnitude - (d2.x * d2.x) / d2.sqrMagnitude - (d3.x * d3.x) / d3.sqrMagnitude;
        double SXY = -(d1.x * d1.y) / d1.sqrMagnitude - (d2.x * d2.y) / d2.sqrMagnitude - (d3.x * d3.y) / d3.sqrMagnitude;
        double SXZ = -(d1.x * d1.z) / d1.sqrMagnitude - (d2.x * d2.z) / d2.sqrMagnitude - (d3.x * d3.z) / d3.sqrMagnitude;
        double SYX = -(d1.y * d1.x) / d1.sqrMagnitude - (d2.y * d2.x) / d2.sqrMagnitude - (d3.y * d3.x) / d3.sqrMagnitude;
        double SYY = 3 - (d1.y * d1.y) / d1.sqrMagnitude - (d2.y * d2.y) / d2.sqrMagnitude - (d3.y * d3.y) / d3.sqrMagnitude;
        double SYZ = -(d1.y * d1.z) / d1.sqrMagnitude - (d2.y * d2.z) / d2.sqrMagnitude - (d3.y * d3.z) / d3.sqrMagnitude;
        double SZX = -(d1.z * d1.x) / d1.sqrMagnitude - (d2.z * d2.x) / d2.sqrMagnitude - (d3.z * d3.x) / d3.sqrMagnitude;
        double SZY = -(d1.z * d1.y) / d1.sqrMagnitude - (d2.z * d2.y) / d2.sqrMagnitude - (d3.z * d3.y) / d3.sqrMagnitude;
        double SZZ = 3 - (d1.z * d1.z) / d1.sqrMagnitude - (d2.z * d2.z) / d2.sqrMagnitude - (d3.z * d3.z) / d3.sqrMagnitude;

        Debug.Log($"SXX: {SXX}");
        Debug.Log($"SXY: {SXY}");
        Debug.Log($"SXZ: {SXZ}");
        Debug.Log($"SYX: {SYX}");
        Debug.Log($"SYY: {SYY}");
        Debug.Log($"SYZ: {SYZ}");
        Debug.Log($"SZX: {SZX}");
        Debug.Log($"SZY: {SZY}");
        Debug.Log($"SZZ: {SZZ}");
        Debug.Log($"{d1.sqrMagnitude} {d2.sqrMagnitude} {d3.sqrMagnitude}");


        double outX = a1.x + a2.x + a3.x - d1.x*(a1.x * d1.x + a1.y * d1.y + a1.z * d1.z)/d1.sqrMagnitude - d2.x*(a2.x * d2.x + a2.y * d2.y + a2.z * d2.z)/d2.sqrMagnitude - d3.x*(a3.x * d3.x + a3.y * d3.y + a3.z * d3.z)/d3.sqrMagnitude;
        double outY = a1.y + a2.y + a3.y - d1.y*(a1.x * d1.x + a1.y * d1.y + a1.z * d1.z)/d1.sqrMagnitude - d2.y*(a2.x * d2.x + a2.y * d2.y + a2.z * d2.z)/d2.sqrMagnitude - d3.y*(a3.x * d3.x + a3.y * d3.y + a3.z * d3.z)/d3.sqrMagnitude;
        double outZ = a1.z + a2.z + a3.z - d1.z*(a1.x * d1.x + a1.y * d1.y + a1.z * d1.z)/d1.sqrMagnitude - d2.z*(a2.x * d2.x + a2.y * d2.y + a2.z * d2.z)/d2.sqrMagnitude - d3.z*(a3.x * d3.x + a3.y * d3.y + a3.z * d3.z)/d3.sqrMagnitude;


        double[][] S = new double[][] { new double[] { SXX, SXY, SXZ }, new double[] { SYX, SYY, SYZ }, new double[] { SZX, SZY, SZZ } }; 

        double[][] invertedS = matrixExample.Program.MatrixInverse(S);
        double x_out = outX * invertedS[0][0] + outY * invertedS[0][1] + outZ * invertedS[0][2];
        double y_out = outX * invertedS[1][0] + outY * invertedS[1][1] + outZ * invertedS[1][2];
        double z_out = outX * invertedS[2][0] + outY * invertedS[2][1] + outZ * invertedS[2][2];
        Vector3 output = new Vector3((float)x_out, (float)y_out, (float)z_out);
        return output;
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}



namespace matrixExample
{

    class Program
    {

        static void Main(string[] args)
        {

            double[][] m = new double[][] { new double[] { 7, 2, 1 }, new double[] { 0, 3, -1 }, new double[] { -3, 4, 2 } };
            double[][] inv = MatrixInverse(m);


            //printing the inverse
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    Console.Write(Math.Round(inv[i][j], 1).ToString().PadLeft(5, ' ') + "|");
                Console.WriteLine();
            }

        }

        public static double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        static double[][] MatrixIdentity(int n)
        {
            // return an n x n Identity matrix
            double[][] result = MatrixCreate(n, n);
            for (int i = 0; i < n; ++i)
                result[i][i] = 1.0;

            return result;
        }

        public static double[][] MatrixProduct(double[][] matrixA, double[][] matrixB)
        {
            int aRows = matrixA.Length; int aCols = matrixA[0].Length;
            int bRows = matrixB.Length; int bCols = matrixB[0].Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");

            double[][] result = MatrixCreate(aRows, bCols);

            for (int i = 0; i < aRows; ++i) // each row of A
                for (int j = 0; j < bCols; ++j) // each col of B
                    for (int k = 0; k < aCols; ++k) // could use k less-than bRows
                        result[i][j] += matrixA[i][k] * matrixB[k][j];

            return result;
        }

        public static double[][] MatrixInverse(double[][] matrix)
        {
            int n = matrix.Length;
            double[][] result = MatrixDuplicate(matrix);

            int[] perm;
            int toggle;
            double[][] lum = MatrixDecompose(matrix, out perm,
              out toggle);
            if (lum == null)
                throw new Exception("Unable to compute inverse");

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }

                double[] x = HelperSolve(lum, b);

                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        }

        static double[][] MatrixDuplicate(double[][] matrix)
        {
            // allocates/creates a duplicate of a matrix.
            double[][] result = MatrixCreate(matrix.Length, matrix[0].Length);
            for (int i = 0; i < matrix.Length; ++i) // copy the values
                for (int j = 0; j < matrix[i].Length; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        static double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }

        static double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // rerturns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            int rows = matrix.Length;
            int cols = matrix[0].Length; // assume square
            if (rows != cols)
                throw new Exception("Attempt to decompose a non-square m");

            int n = rows; // convenience

            double[][] result = MatrixDuplicate(matrix);

            perm = new int[n]; // set up row permutation result
            for (int i = 0; i < n; ++i) { perm[i] = i; }

            toggle = 1; // toggle tracks row swaps.
                        // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

            for (int j = 0; j < n - 1; ++j) // each column
            {
                double colMax = Math.Abs(result[j][j]); // find largest val in col
                int pRow = j;
                //for (int i = j + 1; i less-than n; ++i)
                //{
                //  if (result[i][j] greater-than colMax)
                //  {
                //    colMax = result[i][j];
                //    pRow = i;
                //  }
                //}

                // reader Matt V needed this:
                for (int i = j + 1; i < n; ++i)
                {
                    if (Math.Abs(result[i][j]) > colMax)
                    {
                        colMax = Math.Abs(result[i][j]);
                        pRow = i;
                    }
                }
                // Not sure if this approach is needed always, or not.

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    double[] rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                // --------------------------------------------------
                // This part added later (not in original)
                // and replaces the 'return null' below.
                // if there is a 0 on the diagonal, find a good row
                // from i = j+1 down that doesn't have
                // a 0 in column j, and swap that good row with row j
                // --------------------------------------------------

                if (result[j][j] == 0.0)
                {
                    // find a good row to swap
                    int goodRow = -1;
                    for (int row = j + 1; row < n; ++row)
                    {
                        if (result[row][j] != 0.0)
                            goodRow = row;
                    }

                    if (goodRow == -1)
                        throw new Exception("Cannot use Doolittle's method");

                    // swap rows so 0.0 no longer on diagonal
                    double[] rowPtr = result[goodRow];
                    result[goodRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[goodRow]; // and swap perm info
                    perm[goodRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }
                // --------------------------------------------------
                // if diagonal after swap is zero . .
                //if (Math.Abs(result[j][j]) less-than 1.0E-20) 
                //  return null; // consider a throw

                for (int i = j + 1; i < n; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (int k = j + 1; k < n; ++k)
                    {
                        result[i][k] -= result[i][j] * result[j][k];
                    }
                }


            } // main j column loop

            return result;
        }




    }
}