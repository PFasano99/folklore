using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkCollision : MonoBehaviour
{
    public bool isColliding = false;
    
    public string[] tags;
    public string[] ignoreTags;


    private void OnTriggerEnter(Collider other)
    {
        if (tags.Length == 0 || ignoreTags.Length == 0)
        {
            isColliding = true;

        }
        else
        {
            foreach (string tag in tags)
            {
                if (other.gameObject.CompareTag(tag))
                {
                    isColliding = true;
                }
            }

            foreach (string ignoreTag in ignoreTags)
            {
                if (other.gameObject.CompareTag(ignoreTag))
                {
                    isColliding = false;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (tags.Length == 0 || ignoreTags.Length == 0)
        {
            isColliding = true;
            
        }
        else
        {
            foreach (string tag in tags)
            {
                if (other.gameObject.CompareTag(tag))
                {
                    isColliding = true;
                }
            }

            foreach (string ignoreTag in ignoreTags)
            {
                if (other.gameObject.CompareTag(ignoreTag))
                {
                    isColliding = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tags.Length == 0 || ignoreTags.Length == 0)
        {
            isColliding = false;
        }
        else
        {
            foreach (string tag in tags)
            {
                if (other.gameObject.CompareTag(tag))
                {
                    isColliding = false;
                }
            }

            foreach (string ignoreTag in ignoreTags)
            {
                if (other.gameObject.CompareTag(ignoreTag))
                {
                    isColliding = false;
                }
            }
        }
    }
}
