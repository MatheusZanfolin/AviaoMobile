using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StaticGameController
{
    public static List<GameObject> listaInimigo;
    public static List<GameObject> listaTiroAviao;    
    public static List<GameObject> listaTiroInimigo;

    public static int ultimoTiroAviao   = 0;
    public static int ultimoTiroInimigo = 0;
    public static int maxInimigos       = 10;

    private static int vidaAviao = 10;

    public static void CriarTiro(GameObject aviao, GameObject prefabTiroAviao)
    {
        bool podeCriarSemRepeticao = true;

        foreach (GameObject tiro in listaTiroAviao)
            if (System.Math.Abs(listaTiroAviao[listaTiroAviao.Count - 1].transform.position.y - aviao.transform.position.y) > 20)
            {
                podeCriarSemRepeticao = false;

                break;
            }

        if (podeCriarSemRepeticao)
        {
            ultimoTiroAviao++;

            GameObject tiroAviao = GameObject.Instantiate(prefabTiroAviao) as GameObject;

            tiroAviao.name = "Tiro" + ultimoTiroAviao;

            tiroAviao.transform.position = new Vector3(
                aviao.transform.position.x + 0.5f,
                aviao.transform.position.y,
                aviao.transform.position.z
            );

            tiroAviao.SetActive(true);

            listaTiroAviao.Add(tiroAviao);
        }
    }

    public static void CriarTiroInimigo(GameObject inimigo, GameObject prefabTiroInimigo)
    {
        if (prefabTiroInimigo != null)
        {
            ultimoTiroInimigo++;

            GameObject tiroInimigo = GameObject.Instantiate(prefabTiroInimigo) as GameObject;

            tiroInimigo.name = "TiroInimigo" + StaticGameController.ultimoTiroInimigo;

            tiroInimigo.tag = "TiroInimigo";

            tiroInimigo.transform.position = new Vector3(
                inimigo.transform.position.x,
                inimigo.transform.position.y - 1,
                inimigo.transform.position.z
            );

            tiroInimigo.SetActive(true);

            listaTiroInimigo.Add(tiroInimigo);
        }
    }

    public static void MoverTiros(float velocidade)
    {
        foreach (GameObject tiro in listaTiroAviao)
            tiro.transform.position = new Vector3(
                tiro.transform.position.x,
                tiro.transform.position.y + velocidade * Time.deltaTime,
                0
            );

        for (int i = listaTiroAviao.Count - 1; i >= 0; i--)
        {
            GameObject tiro = listaTiroAviao[i];

            if (tiro.transform.position.y > 8.5f)
            {
                listaTiroAviao[i].SetActive(false);
                tiro.SetActive(false);
                tiro.GetComponent<Renderer>().enabled = false;
                listaTiroAviao.RemoveAt(i);
            }
        }            
    }

    internal static void ReduzirVidaAviao()
    {
        vidaAviao--;

        if (vidaAviao <= 0)
            SceneManager.LoadScene("CenaGameOver");
    }

    public static void MoverTirosInimigos(float velocidade)
    {
        foreach (GameObject tiro in listaTiroInimigo)
        {
            tiro.transform.position = new Vector3(
                tiro.transform.position.x,
                tiro.transform.position.y - velocidade * Time.deltaTime,
                tiro.transform.position.z
            );
        }

        for (int i = listaTiroInimigo.Count - 1; i >= 0; i--)
        {
            GameObject tiro = listaTiroInimigo[i];

            if (tiro.transform.position.y < -4.5f)
            {
                listaTiroInimigo[i].SetActive(false);
                tiro.SetActive(tiro);
                tiro.GetComponent<Renderer>().enabled = false;
                listaTiroInimigo.RemoveAt(i);
            }
        }
    }

    public static void RemoverTiro(GameObject qualTiro)
    {
        for (int i = listaTiroAviao.Count - 1; i >= 0; i--)
        {
            GameObject tiro = listaTiroAviao[i];

            if (tiro.name == qualTiro.name)
            {
                listaTiroAviao[i].SetActive(false);
                tiro.SetActive(false);
                tiro.GetComponent<Renderer>().enabled = false;
                listaTiroAviao.RemoveAt(i);
            }
        }
    }

    public static void RemoverTiroInimigo(GameObject tiro)
    {
        for (int i = listaTiroInimigo.Count - 1; i >= 0; i--)
            if (listaTiroInimigo[i].name == tiro.name)
            {
                Escrever("Removendo tiro inimigo " + tiro.name);
                listaTiroInimigo[i].SetActive(false);
                tiro.SetActive(tiro);
                tiro.GetComponent<Renderer>().enabled = false;
                listaTiroInimigo.RemoveAt(i);
            }
    }

    public static void InimigoSaiDeCena(GameObject outro)
    {
        for (int i = 0; i < maxInimigos; i++)
            if (listaInimigo[i].name == outro.name)
            {
                Escrever(outro.name + " removido");

                outro.SetActive(false);
                outro.GetComponent<Renderer>().enabled = false;
                listaInimigo[i].SetActive(false);

                break;
            }
    }

    public static void SpawnInimigos(float xMinimo, float xMaximo)
    {
        float posicaoX = 0;

        GameObject inimigo = null;

        for (int i = 0; i < maxInimigos; i++)
            if (listaInimigo[i].activeSelf == false)
            {
                inimigo = StaticGameController.listaInimigo[i];
                posicaoX = UnityEngine.Random.Range(xMinimo, xMaximo);
                inimigo.transform.position = new Vector3(
                    posicaoX,
                    10f,
                    0
                );
                inimigo.SetActive(true);
                inimigo.GetComponent<Renderer>().enabled = true;
                StaticGameController.listaInimigo[i] = inimigo;

                inimigo.tag = "Inimigo";

                break;
            }
    }

    public static void CriarListaInimigos(GameObject prefabInimigo, int maximoInimigos)
    {
        maxInimigos = maximoInimigos;

        listaInimigo = new List<GameObject>();

        for (int i = 0; i < maxInimigos; i++)
        {
            GameObject inimigo = GameObject.Instantiate(prefabInimigo) as GameObject;

            inimigo.name = "Inimigo" + i;
            inimigo.SetActive(false);
            inimigo.GetComponent<Renderer>().enabled = true;
            listaInimigo.Add(inimigo);
        }
    }

    public static void DesativarInimigo(GameObject outro)
    {
        for (int i = 0; i < maxInimigos; i++)
            if (listaInimigo[i].name == outro.name)
            {
                Escrever(outro.name + " desativado por colisão");

                listaInimigo[i].SetActive(false);
                outro.gameObject.SetActive(false);
                listaInimigo[i].GetComponent<Renderer>().enabled = false;

                break;
            }
    }

    private static void Escrever(object mensagem)
    {
        Debug.Log("StaticGameController: " + mensagem);
    }
}
