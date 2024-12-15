using TMPro;
using UnityEngine;

public class ApplicationCheck : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI applicationTypeText;
    [SerializeField] private TextMeshProUGUI CompatableText;

    [SerializeField] private RectTransform Disclamer;

    [SerializeField] Material Web;
    [SerializeField] Material Mobile;

    [SerializeField] private StartGame startGame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Application.isMobilePlatform)
        {
            //gameObject.GetComponent<Renderer>().material = Mobile;

            applicationTypeText.text = "Application: Mobile";

            CompatableText.text = "Compatable: True";

            startGame.StartTheGame();
        }
        else
        {
            //gameObject.GetComponent<Renderer>().material = Web;

            applicationTypeText.text = "Application: Web";

            CompatableText.text = "Compatable: False";

            Disclamer.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
