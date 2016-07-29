using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A GradualTextRevealer reveals a string one character at a time.
/// </summary>
public class GradualTextRevealer : MonoBehaviour
{
    enum State
    {
        DoingNothing,
        Revealing,
        Paused
    };

    State state = State.DoingNothing;

    /// <summary>
    /// The text to gradually reveal.
    /// If you set this and call Resume()... idk meng
    /// </summary>
    [TextArea]
    public string text;

    /// <summary>
    /// How long to wait in between each character
    /// </summary>
    public float waitTime = 0.025f;

    /// <summary>
    /// The current index of the string.
    /// </summary>
    int i;

    /// <summary>
    /// The current index of the string.
    /// </summary>
    public int index
    {
        get
        {
            return i;
        }
    }

    /// <summary>
    /// The Text component to update. If left as null, will look for a Text component
    /// on this GameObject.
    /// </summary>
    public Text textComponent;

    /// <summary>
    /// Use the Text component on this GameObject.
    /// </summary>
    void UseOwnText()
    {
        textComponent = GetComponent<Text>();
    }

    void Start()
    {
        if (textComponent == null) UseOwnText();
    }

    void Update()
    {
    }

    /// <summary>
    /// Pause the current coroutine.
    /// </summary>
    public void PauseReveal()
    {
        if (state != State.Revealing) return;
        state = State.Paused; // THIS IS RACY I THINK
        StopCoroutine("RevealText");

    }

    /// <summary>
    /// Stop the current revealing text.
    /// This does not affect the text area at all.
    /// </summary>
    public void StopReveal()
    {
        if (state == State.DoingNothing) return;
        StopCoroutine("RevealText");
        state = State.DoingNothing;
    }

    /// <summary>
    /// Start revealing the given text.
    /// This will overwrite the class's text.
    /// This is offered for convenience.
    /// </summary>
    /// <param name="text">text to overwrite with and reveal.</param>
    public void StartReveal(string text)
    {
        if (state != State.DoingNothing) return;
        this.text = text;
        StartReveal();
    }

    /// <summary>
    /// Start revealing the class's assigned text.
    /// </summary>
    public void StartReveal()
    {
        if (state != State.DoingNothing) return;
        state = State.Revealing;
        i = 0;
        StartCoroutine("RevealText");
    }

    /// <summary>
    /// Resume showing the class's text.
    /// </summary>
    public void ResumeReveal()
    {
        if (state != State.Paused) return;
        state = State.Revealing;
        StartCoroutine("RevealText");
    }

    /// <summary>
    /// The text reveal coroutine.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    IEnumerator RevealText()
    {
        char[] charray = this.text.ToCharArray();

        while(i < charray.Length)
        {
            textComponent.text += charray[i++];
            yield return new WaitForSeconds(waitTime);
        }

        state = State.DoingNothing;
    }
}
