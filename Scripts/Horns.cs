using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Horns : MonoBehaviour
{
    // Start is called before the first frame update
    public Horns_Output horns_Output;
    public List<Vector3> source_transformed;
    public List<Vector3> target;
    public List<Vector3> source;
    public struct Horns_Output
    {
        public Vector3 translation;
        public Vector3 rotation;
        public float scale;
    }

    public Horns_Output HornsAlgorithm( List <Vector3> physicalFiducials,  List <Vector3> ctFiducials)
    {
        int nFiducials = physicalFiducials.Count;
        Debug.Log($"Fids: {nFiducials}");
        // Get the means centroids
        var seed = new Vector3 (0, 0, 0);
        Vector3 physicalMean = physicalFiducials.Aggregate(seed, (acc, x) => acc + x) / nFiducials;
        Debug.Log($"Physical Mean: {physicalMean}");
        seed = new Vector3(0, 0, 0);
        Vector3 ctMean = ctFiducials.Aggregate(seed, (acc, x) => acc + x) / nFiducials;
        Debug.Log($"Physical Mean: {ctMean}");
        // Move by their centroids
        var physicalAlgorithm = physicalFiducials.Select(x => x - physicalMean).ToList();
        var ctAlgorithm = ctFiducials.Select(x => x - ctMean).ToList();

        // Get Scaling Factor
        var physNorm = physicalAlgorithm.Aggregate(0.0, (acc, x) => acc + x.sqrMagnitude);
        var ctNorm = ctAlgorithm.Aggregate(0.0, (acc, x) => acc + x.sqrMagnitude);
        var scale = Math.Sqrt(physNorm / ctNorm);
        var rotation = RotationOptimiser(ctAlgorithm, physicalAlgorithm);
        var mean_rotated = Quaternion.Euler(rotation) * ctMean;
        var translation = physicalMean - (float) scale * mean_rotated;
        Horns_Output output;
        output.rotation = rotation;
        output.translation = translation;
        output.scale = (float) scale;
        return output;
    }

    public Vector3 RotationOptimiser(List<Vector3> ctShifted, List<Vector3> physShifted)
    {
        int steps_per_level = 10;
        int steps_cubed = steps_per_level * steps_per_level * steps_per_level;
        List<Vector3> guesses = new List<Vector3> { new Vector3(90f, 90f,  90f) };
        int nGuesses = 30;
        float gap = 360;
        for (int level = 3; level > 0; level--)
        {
            float step = gap / steps_per_level;
            //Debug.Log($"Guess: {rotation_guess}");
            Vector3[] rotation_guesses = new Vector3[steps_cubed * guesses.Count];
            float[] minimal_answers = new float[steps_cubed * guesses.Count];
            for (int i = 0; i < steps_per_level; i++)
            {
                for (int j = 0; j < steps_per_level; j++)
                {
                    for (int k = 0; k < steps_per_level; k++)
                    {
                        for (int l = 0; l < guesses.Count; l++)
                        {
                            //Debug.Log($"Step: {step}");
                            int index = i + j * steps_per_level + k * (steps_per_level * steps_per_level);
                            float x = guesses[l].x + (i * step) - (gap / 2);
                            float y = guesses[l].y + (j * step) - (gap / 2);
                            float z = guesses[l].z + (k * step) - (gap / 2);
                            var this_guess = new Vector3(x, y, z);
                            rotation_guesses[l * steps_cubed + index] = this_guess;
                            minimal_answers[l * steps_cubed + index] = RotationFunction(ctShifted, physShifted, this_guess);
                            Debug.Log($"Try: {this_guess}, Loss: {minimal_answers[l * steps_cubed + index]}");
                            //Debug.Log($"Rotation Function: {minimal_answers[index]}");
                        }
                    }
                }
            }
            var minimum_argument = GetSortedIndices(minimal_answers);
            guesses = minimum_argument.Take(nGuesses).Select(x => rotation_guesses[(int)x]).ToList();
            gap = gap / steps_per_level;
        }
        return guesses[0];
    }

    private int[] GetSortedIndices(float[] list)
    {
        var output = list.Select((x, i) => new { value = x, index = i}).OrderBy(x => x.value).Select(x => x.index).ToArray();
        return output;
    }

    public float RotationFunction(List<Vector3> ctShifted, List<Vector3> physShifted, Vector3 guess)
    {
        var sourceNorm = ctShifted.Aggregate(0.0, (acc, x) => acc + x.sqrMagnitude);
        var targetNorm = physShifted.Aggregate(0.0, (acc, x) => acc + x.sqrMagnitude);
        var firstTerm = Math.Sqrt(sourceNorm * targetNorm);
        //Debug.Log($"First Term: {firstTerm}");
        var rotated = ctShifted.Select(x => Quaternion.Euler(guess) * x);
        var secondTerm = rotated.Zip(physShifted, (s, r) => Vector3.Dot(s, r)).Aggregate((acc, x) => acc + x);
        //Debug.Log($"Second Term: {secondTerm}");
        return (float)(firstTerm - secondTerm);
    }

    void Start()
    {
        source = new List<Vector3>
        {
            new Vector3(1.16181421f, 1.4115198f,  1.36104564f),
            new Vector3(1.05294962f, 3.0252922f,  1.77816076f),
            new Vector3(1.11215146f, 5.1968371f, 1.60849993f),
        };
        target = new List<Vector3>
        {
            new Vector3( 1.81709887f, -2.25784091f, -2.01429712f),
            new Vector3( 0.11384091f, -0.73683354f,  0.42411228f),
            new Vector3(-1.87703118f,  2.40413162f,  2.69619372f),
        };

        horns_Output = HornsAlgorithm(source, target);
        Debug.Log($"Scale: {horns_Output.scale}");
        Debug.Log($"Rotation: {horns_Output.rotation}");
        Debug.Log($"Translation: {horns_Output.translation}");
        var R = horns_Output.rotation;
        var t = horns_Output.translation;
        var s = horns_Output.scale;
        source_transformed = target.Select(x => s * (Quaternion.Euler(R) * x) + t).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
