using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Linq;

public class StateSetDemo : MonoBehaviour
{
    public TextAsset xml;
    XElement root;
    StateSet[] states;
    public Text text;

    void OnValidate()
    {
        if (xml == null) Debug.LogError("Missing xml file", this);
        if (text == null)
        {
            Debug.LogWarning("No text component for StateSetDemo, defaulting to self");
            if (!(text = GetComponent<Text>())) Debug.LogError("Missing text", this);
        }
    }

    // Use this for initialization
    void Start()
    {
        root = XElement.Parse(xml.text);
        states = root.Elements("states").Select((x) => new StateSet(x)).ToArray();

        text.text = string.Join("\n\n", states.Select((x) => x.ToString()).ToArray());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
