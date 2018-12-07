using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoverAviao : MonoBehaviour {
    public const float VELOCIDADE_AVIAO_HORIZONTAL = 3.0f;
    public const float VELOCIDADE_AVIAO_VERTICAL   = 1.0f;

    private bool podeAtirar = true;

    private long framesUltimoTiro = 60;

    public AudioClip somDeExplosao;

    public GameObject prefabTiroAviao;
    public float velocidadeTiroAviao = 4f;
    public float taxaDeTiros = 0.05f;
    private float proximoTiro = 0.0f;

    public float taxaSpawnInimigos = 2;    
    public float xMaximo = 4;
    public float xMinimo = -4;
    private float taxaSpawnAtual;
    public int maxInimigos = 10;
    public GameObject prefabInimigo;

    // Use this for initialization
    void Start () {
        StaticGameController.listaInimigo     = new List<GameObject>();
        StaticGameController.listaTiroAviao   = new List<GameObject>();
        StaticGameController.listaTiroInimigo = new List<GameObject>();

        StaticGameController.CriarListaInimigos(prefabInimigo, maxInimigos);
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_ANDROID || UNITY_WP8
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
#endif

        VerificarCriacaoDeInimigos();

        AcessarDadosDoAcelerometro();

        TratarSetasDeDirecao(gameObject);

        StaticGameController.MoverTiros(3f);

        if (PodeAtirar())
            VerificarCriacaoDeTiros();
	}

    private void VerificarCriacaoDeTiros()
    {
        bool criarTiro = false;
        if (Input.GetKey(KeyCode.Space))
            criarTiro = true;

        if (Input.GetButton("Fire1") && Time.deltaTime > proximoTiro)
        {
            criarTiro = true;

            proximoTiro = Time.deltaTime + taxaDeTiros;
        }

        if (Input.touchCount >= 1)
        {
            Touch toque = Input.GetTouch(0);

            if (toque.phase == TouchPhase.Began)
                criarTiro = true;
        }

        if (criarTiro)
        {
            podeAtirar = false;

            framesUltimoTiro = 0;

            StaticGameController.CriarTiro(gameObject, prefabTiroAviao);            
        }            
    }

    private bool PodeAtirar()
    {
        if (framesUltimoTiro >= 60)
            return true;

        framesUltimoTiro++;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
#if UNITY_ANDROID || UNITY_WP8
#endif

        if (outro.gameObject.tag == "Inimigo" || outro.tag == "Inimigo")
        {
            AudioSource.PlayClipAtPoint(somDeExplosao, new Vector3(1, 1, 1), 20);

#if UNITY_ANDROID
            Handheld.Vibrate();
#endif

            StaticGameController.DesativarInimigo(outro.gameObject);

            if (gameObject.tag == "TiroAviao")
                StaticGameController.RemoverTiro(gameObject);
        }
        else if (outro.tag == "TiroInimigo")
            StaticGameController.RemoverTiroInimigo(outro.gameObject);

        if (!(outro.tag == "TiroAviao" || outro.gameObject.tag == "TiroAviao"))
            StaticGameController.ReduzirVidaAviao();
    }

    void Escrever(object mensagem)
    {
        Debug.Log("MoverAviao: " + mensagem);
    }

    void TratarSetasDeDirecao(GameObject aviao)
    {
        float eixoHorizontal = Input.GetAxis("Horizontal");
        float eixoVertical = Input.GetAxis("Vertical");

        aviao.transform.position = new Vector3(
            aviao.transform.position.x + eixoHorizontal * VELOCIDADE_AVIAO_HORIZONTAL * Time.deltaTime,
            aviao.transform.position.y + eixoVertical   * VELOCIDADE_AVIAO_VERTICAL   * Time.deltaTime,
            aviao.transform.position.z
        );

        if (eixoHorizontal > 0)
        {
            if (aviao.transform.position.x > 3.9f)
                aviao.transform.position = new Vector3(
                    3.9f,
                    aviao.transform.position.y,
                    aviao.transform.position.z
                );
        }
        else if (eixoHorizontal < 0)
        {
            if (aviao.transform.position.x < -4.9f)
                aviao.transform.position = new Vector3(
                    -4.9f,
                    aviao.transform.position.y,
                    aviao.transform.position.z
                );
        }

        if (eixoVertical > 0)
        {
            if (aviao.transform.position.y > 8.0f)
                SceneManager.LoadScene("CenaGanhou");
        }
        else if (eixoVertical < 0)
            if (aviao.transform.position.y < -3.9f)
                aviao.transform.position = new Vector3(
                    aviao.transform.position.x,
                    -3.9f,
                    aviao.transform.position.z
                );
    }

    void AcessarDadosDoAcelerometro()
    {
        Vector3 aceleracao = Input.acceleration;

        if (Mathf.Abs(aceleracao.x) > 0.5f)
        {
            Vector3 direcao = Vector3.zero;

            direcao.x = aceleracao.x;
            direcao.y = aceleracao.y;

            transform.Translate(direcao * VELOCIDADE_AVIAO_VERTICAL * Time.deltaTime);

            if (transform.position.x > 3.9f)
                transform.position = new Vector3(3.9f, transform.position.y, transform.position.z);
            else if (transform.position.x < -3.9f)
                transform.position = new Vector3(-3.9f, transform.position.y, transform.position.z);

            if (transform.position.y > 6.5f)
                SceneManager.LoadScene("CenaGanhou");
            else if (transform.position.y < -6.5f)
                transform.position = new Vector3(transform.position.x, -6.5f, transform.position.z);
        }
    }

    void VerificarCriacaoDeInimigos()
    {
        taxaSpawnAtual += Time.deltaTime;

        if (taxaSpawnAtual > taxaSpawnInimigos)
        {
            taxaSpawnAtual = 0;

            StaticGameController.SpawnInimigos(xMinimo, xMaximo);
        }
    }
}
