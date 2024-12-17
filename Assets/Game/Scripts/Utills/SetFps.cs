using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetFps : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;

    [SerializeField] private float fps = 60;

    private float deltaTime;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        //fpsText.text = Application.targetFrameRate.ToString();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
    }
}
