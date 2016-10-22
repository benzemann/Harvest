using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWar : MonoBehaviour {

    #region Private
    /// <summary>
    /// The size of the texture in BOTH x and y.
    /// Should be a power of 2.
    /// </summary>
    [SerializeField]
    private int _textureSize = 256;
    [SerializeField]
    private Color _fogOfWarColor;
    [SerializeField]
    private Color _hiddenColor;
    [SerializeField]
    private LayerMask _fogOfWarLayer;

    private Texture2D _texture;
    private Color[] _pixels;
    private List<Revealer> _revealers;
    private int _pixelsPerUnit;
    private Vector2 _centerPixel;
    private bool firstClear = true;

    private static FogOfWar _instance;
    #endregion

    #region Public
    /// <summary>
    /// Note this is NOT a singleton!
    /// This just needs to be globally accessable AND still be a MonoBehaviour.
    /// </summary>
    public static FogOfWar Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion

    private void Awake()
    {
        _instance = this;

        var renderer = GetComponent<Renderer>();
        Material fogOfWarMat = null;
        if (renderer != null)
        {
            fogOfWarMat = renderer.material;
        }

        if (fogOfWarMat == null)
        {
            Debug.LogError("Material for Fog Of War not found!");
            return;
        }

        _texture = new Texture2D(_textureSize, _textureSize, TextureFormat.RGBA32, false);
        _texture.wrapMode = TextureWrapMode.Clamp;

        _pixels = _texture.GetPixels();
        ClearPixels();

        fogOfWarMat.mainTexture = _texture;

        _revealers = new List<Revealer>();

        _pixelsPerUnit = Mathf.RoundToInt(_textureSize / transform.lossyScale.x);

        _centerPixel = new Vector2(_textureSize * 0.5f, _textureSize * 0.5f);
    }


    public void RegisterRevealer(Revealer revealer)
    {
        _revealers.Add(revealer);
    }

    private void ClearPixels()
    {
        for (var i = 0; i < _pixels.Length; i++)
        {
            if (firstClear || _pixels[i].a > 0.7f)
                _pixels[i] = _hiddenColor;
            else
                _pixels[i] = _fogOfWarColor;
        }
        firstClear = false;
    }

    private void SmoothPixels()
    {
        Color[] _pixelsCopy = new Color[_pixels.Length];
        for (var x = 0; x < _textureSize; x++)
        {
            for(var y = 0; y < _textureSize; y++)
            {
                var sum = 0f;
                var neighbors = 0;
                for(int i = -1; i < 2; i++)
                {
                    for(int j = -1; j < 2; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        var x_n = x + i;
                        var y_n = y + j;
                        if (x_n < 0 || x_n >= _textureSize || y_n < 0 || y_n >= _textureSize)
                            continue;
                        var indx = y_n * _textureSize + x_n;
                        sum +=  _pixels[indx].a;
                        neighbors++;
                    }
                }
                _pixelsCopy[y * _textureSize + x] = _pixels[y * _textureSize + x];
                _pixelsCopy[y * _textureSize + x].a = (sum / neighbors) * 0.8f + _pixels[y * _textureSize + x].a * 0.2f;
            }
        }
        _pixels = _pixelsCopy;
    }

    /// <summary>
    /// Sets the pixels in _pixels to clear a circle.
    /// </summary>
    /// <param name="originX">in pixels</param>
    /// <param name="originY">in pixels</param>
    /// <param name="radius">in unity units</param>
    private void CreateCircle(int originX, int originY, int radius)
    {
        for (var y = -radius * _pixelsPerUnit; y <= radius * _pixelsPerUnit; ++y)
        {
            for (var x = -radius * _pixelsPerUnit; x <= radius * _pixelsPerUnit; ++x)
            {
                if (x * x + y * y <= (radius * _pixelsPerUnit) * (radius * _pixelsPerUnit))
                {
                    if ((originY + y) < 0 || (originY + y) >= _textureSize || (originX + x) < 0 || (originX + x) >= _textureSize)
                        continue;
                    int indx = (originY + y) * _textureSize + originX + x;
                    _pixels[indx] = new Color(0, 0, 0, 0);
                }
            }
        }
    }

    private void Update()
    {
        ClearPixels();

        _revealers.RemoveAll(revealer => revealer == null);

        foreach (var revealer in _revealers)
        {
            var translatedPos = revealer.transform.position - transform.position;

            var pixelPosX = Mathf.RoundToInt(translatedPos.x * _pixelsPerUnit + _centerPixel.x);
            var pixelPosY = Mathf.RoundToInt(translatedPos.z * _pixelsPerUnit + _centerPixel.y);

            CreateCircle(pixelPosX, pixelPosY, revealer.radius);
           
        }

        SmoothPixels();

        _texture.SetPixels(_pixels);
        _texture.Apply(false);
    }

    public bool IsVisible(Vector3 pos)
    {
        var translatedPos = pos - transform.position;

        var pixelPosX = Mathf.RoundToInt(translatedPos.x * _pixelsPerUnit + _centerPixel.x);
        var pixelPosY = Mathf.RoundToInt(translatedPos.z * _pixelsPerUnit + _centerPixel.y);

        int indx = pixelPosY * _textureSize + pixelPosX;
        if (_pixels[indx].a == 0)
            return true;
        return false;
    }
}
