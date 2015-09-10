using UnityEngine;
using System.Collections;
using Magicolo;

public class Card : MonoBehaviour
{

    public string name;

    [Button("Flip", "Flip")]
    public bool flip;

    public GameObject front;
    public GameObject back;
    float t = 0;
    bool onFront = true;
    bool fliped;

    void Flip()
    {
        if (t <= 0)
        {
            t = 180;
            fliped = false;
        }

    }

    void Start()
    {

    }

    void Update()
    {
        if (t > 0)
        {
            float rotation = 1;
            t -= rotation;
            transform.Rotate(0, rotation, 0);
            if (!fliped && t <= 90)
            {
                if (onFront)
                {
                    front.SetActive(false);
                    back.SetActive(true);
                    onFront = false;
                }
                else
                {
                    back.SetActive(false);
                    front.SetActive(true);
                    onFront = true;
                }
                fliped = true;
            }
            if(t<= 0)
            {
                if (onFront)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
                }

            }
        }
    }
}
