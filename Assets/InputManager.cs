using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    CombatManager cm;
    PlayerManager pm;
    // Start is called before the first frame update
    void Start()
    {
        cm = GameObject.FindObjectOfType<CombatManager>();
        pm = GameObject.FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!cm.GamePaused)
        {
            checkKey();
        }
        else
        {
            checkMenuKeys();
        }
    }

    void checkKey()
    {
        if (pm.isActiveAndEnabled == true)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                cm.Key1Down();

            }
            else if (Input.GetKeyUp(KeyCode.Q))
            {
                cm.Key1Up();
            }
            if (Input.GetKey(KeyCode.E))
            {
                cm.Key2Down();

            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                cm.Key2Up();
            }

            if (Input.GetKey(KeyCode.R))
            {
                cm.Key3Down();

            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                cm.Key3Up();
            }

            if (Input.GetKey(KeyCode.F))
            {
                cm.Key4Down();

            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                cm.Key4Up();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            cm.Mouse1Down();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            cm.Mouse1Up();
        }

        if (Input.GetMouseButtonDown(1))
        {
            cm.Mouse2Down();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            cm.Mouse2Up();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            cm.EscDown();

        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            cm.EscUp();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            cm.SpaceDown();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            cm.SpaceUp();
        }
        if (Input.GetKey(KeyCode.F1))
        {
            cm.F1Down();
        }
        else if (Input.GetKeyUp(KeyCode.F1))
        {
            cm.F1Up();
        }
        if (Input.GetKey(KeyCode.C))
        {
            cm.CDown();
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            cm.CUp();
        }

    }

    void checkMenuKeys()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            cm.EscDown();

        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            cm.EscUp();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            cm.SpaceDown();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            cm.SpaceUp();
        }
        if (Input.GetKey(KeyCode.Return))
        {
            cm.ReturnDown();
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            cm.ReturnUp();
        }
    }

}
