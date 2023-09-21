using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FloatEditor : MonoBehaviour
{
    public class FloatEvent : UnityEvent<float>{}

    public FloatEvent onValueChanged;

    [Header("Components")]
    public Slider slider;
    public TMP_InputField input;
    // string used to format the text field when you move the slider
    // see https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings for details
    public string formatString = "0.0";

    // property for our float value that sets any slider or input we may have attached
    private float _floatValue;
    public float floatValue
    {
        get { return _floatValue; }
        set
        {
            // update our internal variable
            _floatValue = value;
            
            // make sure all our controls are visually consistent
            if (slider)
                slider.value = value;
            if (input)
                input.text = (value * 100).ToString(formatString);
            
            // update any client code that has registered with our event
            onValueChanged.Invoke(_floatValue);
        }
    }

    void Start()
    {
        if(slider)
            slider.onValueChanged.AddListener((float value) => { floatValue = value; });
        if(input)
            input.onEndEdit.AddListener((string text) =>
            {
                float parsedValue;
                if (float.TryParse(text, out parsedValue))
                    floatValue = parsedValue / 100;
            });
    }
}
