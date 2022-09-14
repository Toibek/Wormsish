using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCellular : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Texture2D t = Cellular.GenerateTexture();
            Sprite s = Sprite.Create(t, new Rect(Vector2.zero, new(t.width, t.height)),Vector2.zero);
            GetComponent<Image>().sprite = s;
        }
    }
}
