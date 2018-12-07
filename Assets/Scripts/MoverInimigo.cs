using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverInimigo : MonoBehaviour {
    public float velocidadeInimigo = 2.0f;

    public GameObject prefabTiroInimigo;

    public AudioClip somDeExplosao;

    private bool colidindo = false;

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter2D(Collider2D outro)
    {
        #if Unity_ANDROID || UNITY_WP8
            if (Input.GetKeyDown(KeyCode.Escape))
                Applcation.Quit();
        #endif

        if (outro.tag == "TiroAviao" || outro.gameObject.tag == "TiroAviao")
        {
            AudioSource.PlayClipAtPoint(somDeExplosao, new Vector3(0, 0, 0), 20);
            
            StaticGameController.DesativarInimigo(gameObject);
        }

        #if UNITY_ANDROID
            Handheld.Vibrate();
        #endif        
    }

    // Update is called once per frame
    void Update () {
#if Unity_ANDROID || UNITY_WP8
            if (Input.GetKeyDown(KeyCode.Escape))
                Applcation.Quit();
#endif

        if (gameObject.tag == "Inimigo")
        {
            transform.position += new Vector3(
                0,
                - velocidadeInimigo * Time.deltaTime,
                0
            );

            if (Random.Range(0, 60) == 0)
                StaticGameController.CriarTiroInimigo(gameObject, prefabTiroInimigo);

            if (transform.position.y < -4.5f)
            {
                gameObject.SetActive(false);
                StaticGameController.InimigoSaiDeCena(gameObject);
            }
        }
    }
}
