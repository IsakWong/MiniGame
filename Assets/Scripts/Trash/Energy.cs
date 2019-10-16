using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{

    MeshFilter meshFilter;
    Mesh originalMesh;
    Mesh clonedMesh;
    Vector2[] vertices;
    Vector2[] uvs;
    ushort[] triangles;
    Sprite sprite;
    private float _amountValue;
    public int m_FillOrigin;
    public bool fillClockWise;

    static readonly Vector2[] s_Xy = new Vector2[4];
    static readonly Vector2[] s_Uv = new Vector2[4];

    public float AmountValue
    {
        get
        {
            return _amountValue;
        }

        set
        {
            List<Vector2> vertices = new List<Vector2>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            Vector4 v = new Vector4(2.6f, -2.6f, 2.6f, -2.6f);
            Vector4 outer = new Vector4(1, 1, -1, -1);
            float tx0 = outer.x;
            float ty0 = outer.y;
            float tx1 = outer.z;
            float ty1 = outer.w;

            s_Xy[0] = new Vector2(v.x, v.y);
            s_Xy[1] = new Vector2(v.x, v.w);
            s_Xy[2] = new Vector2(v.z, v.w);
            s_Xy[3] = new Vector2(v.z, v.y);

            s_Uv[0] = new Vector2(tx0, ty0);
            s_Uv[1] = new Vector2(tx0, ty1);
            s_Uv[2] = new Vector2(tx1, ty1);
            s_Uv[3] = new Vector2(tx1, ty0);

            for (int corner = 0; corner < 4; ++corner)
            {
                float fx0, fx1, fy0, fy1;

                if (corner < 2) { fx0 = 0f; fx1 = 0.5f; }
                else { fx0 = 0.5f; fx1 = 1f; }

                if (corner == 0 || corner == 3) { fy0 = 0f; fy1 = 0.5f; }
                else { fy0 = 0.5f; fy1 = 1f; }

                s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                s_Xy[1].x = s_Xy[0].x;
                s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                s_Xy[3].x = s_Xy[2].x;

                s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                s_Xy[2].y = s_Xy[1].y;
                s_Xy[3].y = s_Xy[0].y;

                s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                s_Uv[1].x = s_Uv[0].x;
                s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                s_Uv[3].x = s_Uv[2].x;

                s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                s_Uv[2].y = s_Uv[1].y;
                s_Uv[3].y = s_Uv[0].y;

                float val = fillClockWise ?
                    value * 4f - ((corner + m_FillOrigin) % 4) :
                    value * 4f - (3 - ((corner + m_FillOrigin) % 4));

                if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), fillClockWise, ((corner + 2) % 4)))
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        vertices.Add(s_Xy[i]);
                        uvs.Add(s_Uv[i]);
                        Debug.Log(s_Xy[i]);
                       //vbo.Add(uiv);
                    }
                }
            }
        }
    }

    static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
    {
        int i0 = corner;
        int i1 = ((corner + 1) % 4);
        int i2 = ((corner + 2) % 4);
        int i3 = ((corner + 3) % 4);

        if ((corner & 1) == 1)
        {
            if (sin > cos)
            {
                cos /= sin;
                sin = 1f;

                if (invert)
                {
                    xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                    xy[i2].x = xy[i1].x;
                }
            }
            else if (cos > sin)
            {
                sin /= cos;
                cos = 1f;

                if (!invert)
                {
                    xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                    xy[i3].y = xy[i2].y;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }

            if (!invert) xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
            else xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
        }
        else
        {
            if (cos > sin)
            {
                sin /= cos;
                cos = 1f;

                if (!invert)
                {
                    xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                    xy[i2].y = xy[i1].y;
                }
            }
            else if (sin > cos)
            {
                cos /= sin;
                sin = 1f;

                if (invert)
                {
                    xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                    xy[i3].x = xy[i2].x;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }

            if (invert) xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
            else xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
        }
    }
    static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
    {
        // Nothing to fill
        if (fill < 0.001f) return false;

        // Even corners invert the fill direction
        if ((corner & 1) == 1) invert = !invert;

        // Nothing to adjust
        if (!invert && fill > 0.999f) return true;

        // Convert 0-1 value into 0 to 90 degrees angle in radians
        float angle = Mathf.Clamp01(fill);
        if (invert) angle = 1f - angle;
        angle *= 90f * Mathf.Deg2Rad;

        // Calculate the effective X and Y factors
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        RadialCut(xy, cos, sin, invert, corner);
        RadialCut(uv, cos, sin, invert, corner);
        return true;
    }
    void Awake()
    {
         sprite = GetComponent<SpriteRenderer>().sprite;
        AmountValue = 0.1f;

    
    }

    // Update is called once per frame
    void Update()
    {

    }
}
