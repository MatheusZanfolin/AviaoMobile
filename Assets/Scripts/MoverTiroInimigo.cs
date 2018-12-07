using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverTiroInimigo : MonoBehaviour {
    public float velocidadeTiroInimigo = 3f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        StaticGameController.MoverTirosInimigos(velocidadeTiroInimigo);
	}
}
