using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show_Hide : MonoBehaviour
{
    [SerializeField] private bool hideFirst = true;

    [SerializeField] private GameObject setObject;

    [ShowInInspector, ReadOnly]
    private bool isActive;

    private void Start()
    {
        if(hideFirst)
        {
            Hide();
        }
    }

    public void Hide() 
    {
        setObject.SetActive(false);
    }


    public void ShowHide()
    {
        isActive = !isActive;

        setObject.SetActive(isActive);
    }
}
