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
    /// The text for the current reveal
    /// </summary>
    string currentText;

    /// <summary>
    /// The default wait in between each character. Can be overridden at call time.
    /// </summary>
    public float defaultWaitTime = 0.00025f;

    /// <summary>
    /// The current index of the string.
    /// </summary>
    public int index { get; private set; }

    /// <summary>
    /// The Text component to update. If left as null, will look for a Text component
    /// on this GameObject.
    /// </summary>
    public Text textComponent;

    /// <summary>
    /// The currently running coroutine, or null if it is not.
    /// </summary>
    IEnumerator coroutine;

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

    void Update() {}

    /// <summary>
    /// Pause the current coroutine.
    /// </summary>
    public void PauseReveal()
    {
        if (state != State.Revealing) return;
        StopCoroutine(this.coroutine);
        state = State.Paused;
    }

    /// <summary>
    /// Stop the current revealing text.
    /// This does not affect the text area at all.
    /// </summary>
    public void StopReveal()
    {
        if (state == State.DoingNothing) return;
        StopCoroutine(this.coroutine);
        this.coroutine = null;
        state = State.DoingNothing;
    }

    public void StartReveal(string text, float waitTime)
    {
        if (state != State.DoingNothing) return;
        this.currentText = text;
        state = State.Revealing;
        index = 0;
        this.coroutine = RevealText(waitTime);
        StartCoroutine(this.coroutine);
    }

    /// <summary>
    /// Start revealing the given text.
    /// </summary>
    /// <param name="text">text to overwrite with and reveal.</param>
    public void StartReveal(string text)
    {
		ClearPrevText ();
        StartReveal(text, defaultWaitTime);
    }

    /// <summary>
    /// Resume showing the class's text.
    /// </summary>
    public void ResumeReveal()
    {
        if (state != State.Paused) return;
        state = State.Revealing;
        StartCoroutine(this.coroutine);
    }

    /// <summary>
    /// The text reveal coroutine.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    IEnumerator RevealText(float waitTime)
    {
        char[] charray = this.currentText.ToCharArray();

        while(index < charray.Length)
        {
            textComponent.text += charray[index];
            index++;
            yield return new WaitForSeconds(waitTime);
        }

        state = State.DoingNothing;
    }

    /// <summary>
    /// Reveal text at the default wait speed.
    /// </summary>
    /// <returns></returns>
    IEnumerator RevealText()
    {
        return RevealText(this.defaultWaitTime);
    }

	public void ClearPrevText()
	{
		this.textComponent.text = "";
	}

	public void AppendText(string text)
	{
		textComponent.text = text;
	}

	public void AddAdditionalText(string text)
	{
		textComponent.text += text;
	}
}
