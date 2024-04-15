using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public class TestingShader : MonoBehaviour
{
    public struct Triangle {
        public Vector3 vertexA;
        public Vector3 vertexB;
        public Vector3 vertexC;
    }

    public Mesh mesh;
    private Dictionary<Vector3, int> _vertice_num = new Dictionary<Vector3, int>();
    public RenderTexture _testTextures;
    public ComputeBuffer _pointsBuffer;
    public List<Vector3> _points = new List<Vector3>();
    public Triangle[] _triangles;
    public ComputeBuffer _appendBuffer;
    public ComputeBuffer _verticesBuffer;
    public float _threshold;
    public Texture2D viewTexture;
    public ComputeShader shader;
    private int _width = 512;
    private int _height = 512;
    private int _depth = 3;
    private string _assetsPath;
    private Sprite _sprite;
    private string _dicomPath;
    private float _zScale = 4.0f;
    public Texture2D[] _sprites;
    public Texture2DArray texture2DArray;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.indexFormat = IndexFormat.UInt32;
        _depth = _sprites.Length;
        texture2DArray = new Texture2DArray(_width, _height, _depth, TextureFormat.ARGB32, false);
        for(int i = 0 ; i < _sprites.Length; i++)
        {
            //Debug.Log($"Current Image: {spr.Key}");
            RenderTexture rt = new RenderTexture(_width, _height, 24);
            RenderTexture.active = rt;
            Graphics.Blit(_sprites[i], rt);
            Texture2D result = new Texture2D(_width, _height);
            result.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
            result.Apply();
            Graphics.CopyTexture(result, 0, 0, 0, 0, _width, _height, texture2DArray, i , 0, 0, 0);
        }

        _testTextures = new RenderTexture(_width, _height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        _testTextures.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
        _testTextures.enableRandomWrite = true;
        _testTextures.volumeDepth = _depth;
        _testTextures.Create();
        
        RenderTexture.active = _testTextures;
        viewTexture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(texture2DArray, _testTextures);
        Graphics.CopyTexture(texture2DArray, 18, viewTexture, 0);
        for (int i = 0; i < _depth * _width * _height; i++)
        {
            int z = i / (_width * _height);
            int x = i % _width;
            int y = (i % (_width * _height)) / _width;
            _points.Add(new Vector3(x, y, z));
        }
        Debug.Log(texture2DArray.GetPixels(1)[0]);
        _pointsBuffer = new ComputeBuffer(_points.Count, sizeof(float) * 3);
        _pointsBuffer.SetData(_points);
        _appendBuffer = new ComputeBuffer(18481152 * 2, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        _appendBuffer.SetCounterValue(0);
        //_verticesBuffer = new ComputeBuffer(18481152 * 2, sizeof(float) * 4, ComputeBufferType.Append);
        //_verticesBuffer.SetCounterValue(0);
        //shader.SetBuffer(0, "colors", _verticesBuffer);
        shader.SetTexture(0, "textures", _testTextures);
        shader.SetFloat("threshold", _threshold);
        shader.SetBuffer(0, "points", _pointsBuffer);
        shader.SetBuffer(0, "triangles", _appendBuffer);
        shader.Dispatch(0, _points.Count / 1024, 1, 1);
        _triangles = getItemsFromAppend<Triangle>(_appendBuffer);
        //Vector4[] read_colors = getItemsFromAppend<Vector4>(_verticesBuffer);
        _pointsBuffer.GetData(_points.ToArray());
        //_verticesBuffer.Dispose();
        _pointsBuffer.Dispose();
        _appendBuffer.Dispose();


        //foreach (var item in read_colors)
        //{
        //    int it = (int) item.x;
            // Convert.ToString(it, 2)}
        //    Debug.Log($"Value: {item}");
        //}


        Debug.Log($"Number of polygons: {_triangles.Length}");
        _vertice_num = new Dictionary<Vector3, int>();
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        int k = 0;
        for (int i = 0; i < _triangles.Length; i++)
        {
            foreach (FieldInfo v in typeof(Triangle).GetFields())
            {
                Vector3 vec = (Vector3)v.GetValue(_triangles[i]);
                vec.z = vec.z * _zScale;
                vec = vec / 10;
                //Debug.Log($"Field {v.Name}: {vec}");
                int index = 0;
                bool in_dict = _vertice_num.TryGetValue(vec, out index);
                if (in_dict)
                {
                    indices.Add(index);
                }
                else
                {
                    _vertice_num.Add(vec, k);
                    indices.Add(k);
                    k++;
                    vertices.Add(vec);
                }
            }
        }
        Debug.Log($"Remainders (should be zero): {indices.Count % 3}");
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        //foreach (var pt in _points)
        //{
        //    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    sphere.transform.position = pt;
        //    sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //}
        Debug.Log($"verts {_vertice_num.Count}");
        gameObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        Application.targetFrameRate = 60;
    }

    private T[] getItemsFromAppend<T>(ComputeBuffer buffer)
    {
        var counterBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        ComputeBuffer.CopyCount(buffer, counterBuffer, 0);
        int[] sizeOfAppend = new int[1] { 0 };
        counterBuffer.GetData(sizeOfAppend);
        var output = new T[sizeOfAppend[0]];
        buffer.GetData(output);
        counterBuffer.Dispose();
        return output;
    }

    private Color32[] generateColorLayer( float[] vals)
    {
        Color32[] ret = new Color32[vals.Length];
        for (int i = 0; i < vals.Length; i++)
        {
            Color32 col = new Color(vals[i], vals[i], vals[i], vals[i]);
            ret[i] = col;
        }
        return ret;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }
}
