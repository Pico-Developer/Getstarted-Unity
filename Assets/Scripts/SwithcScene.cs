using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SwithcScene : MonoBehaviour
{
    public InputActionProperty InputActionProperty;

    // Start is called before the first frame update
    void Start()
    {
        InputActionProperty.action.performed += p =>
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            if (index == 0)
                SceneManager.LoadScene(1);
            else if (index == 1)
                SceneManager.LoadScene(0);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
