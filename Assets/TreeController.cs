using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;

public class TreeController : MonoBehaviour
{

    private Transform sprite;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = Transform.FindObjectOfType<PlayerManager>().transform;
        sprite = transform.Find("Sprite");
    }

    // Update is called once per frame
    void Update()
    {
        var renderer = sprite.GetComponent<Renderer>();
        var footOfTree = transform.Find("Foot").position;
        var playerCameraPos = player.position;
        if (player.position.z < footOfTree.z)
        {
            if (transparentFlag < 1) {
                transparentFlag += transparentRate * Time.deltaTime;
            }

            renderer.material.SetFloat("_TransparentFlag", transparentFlag);
        }
        else
        {

            if (transparentFlag > 0)
            {
                transparentFlag -= transparentRate * Time.deltaTime;
            }

            renderer.material.SetFloat("_TransparentFlag", transparentFlag);
            renderer.material.SetVector("_Position", playerCameraPos);
        }
    }

    private float transparentFlag;
    private float transparentRate = 3;

}
