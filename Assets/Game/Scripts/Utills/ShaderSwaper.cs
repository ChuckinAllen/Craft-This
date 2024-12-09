using TMPro;
using UnityEngine;

public class ShaderSwaper : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI applicationTypeText;

    [SerializeField] Material Web;
    [SerializeField] Material Mobile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Application.isMobilePlatform)
        {
            //gameObject.GetComponent<Renderer>().material = Mobile;

            applicationTypeText.text = "Application: Mobile";
        }
        else
        {
            //gameObject.GetComponent<Renderer>().material = Web;

            applicationTypeText.text = "Application: Web";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
