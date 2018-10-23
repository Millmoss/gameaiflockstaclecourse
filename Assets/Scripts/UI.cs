using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    public Button sc, em, tl;
    public GameObject sco, emo, tlo;
    public Text cur;
    enum curState { sc, em, tl };
    static curState state = curState.sc;

    private void Start()
    {

        if (state == curState.sc)
        {
            sco.SetActive(true);
            emo.SetActive(false);
            tlo.SetActive(false);
        }
        else if (state == curState.em)
        {
            emo.SetActive(true);
            sco.SetActive(false);
            tlo.SetActive(false);
        }
        else
        {
            tlo.SetActive(true);
            emo.SetActive(false);
            sco.SetActive(false);
        }
    }

    private void Update()
    {
        if (state == curState.sc)
            cur.text = "Currently : Scalable";
        else if (state == curState.em)
            cur.text = "Currently : Emergent";
        else
            cur.text = "Currently : Two Level";
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }

    public void scal()
    {
        state = curState.sc;
        Reset();
    }
    public void emer()
    {
        state = curState.em;
        Reset();
    }
    public void leve2()
    {
        state = curState.tl;
        Reset();
    }



}
