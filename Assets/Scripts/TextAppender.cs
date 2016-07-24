using UnityEngine;
using UnityEngine.UI;

public class TextAppender : MonoBehaviour
{
    public Text text;

    void Start()
    {
    }

    void Reset()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
    }


    public void AppendText(string text)
    {
        // TODO: not this, ever
        if (text == "clear") this.text.text = "";
        else this.text.text += "\n" + text + "\n"; // text
    }
}
