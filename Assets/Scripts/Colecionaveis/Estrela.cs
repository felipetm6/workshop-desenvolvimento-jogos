using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estrela : MonoBehaviour
{
	private const string player = "Player";

	//Bandeira que define se a Estrela foi coletada, evitando uma coleta dupla
	private bool foiColetada;

	//Timer para desativar a Estrela
	private WaitForSeconds timerParaDesativar = new WaitForSeconds(.1f);

	//Cache do Renderer de Sprite desse objeto
	private SpriteRenderer spRenderer;
	//Cache da Fonte de Áudio desse objeto
	private AudioSource aSource;

	#region Metodos da Unity
	//Função de Evento chamada ao rodar a cena
	private void Awake()
	{
		//Pega o Componente SpriteRenderer nesse objeto (GetComponent) e atribui a spRenderer
		spRenderer = GetComponent<SpriteRenderer>();
		//Pega o Componente AudioSource nesse objeto (GetComponent) e atribui a aSource
		aSource = GetComponent<AudioSource>();
	}

	//Função de Evento chamada ao rodar a cena
	private void Start()
	{

	}

	//Função de Evento chamada sempre que um Colisor entra na Trigger da Porta
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Se o colisor tem a tag igual do jogador e a estrela ainda não foi coletada
		if (collision.gameObject.CompareTag(player) && !foiColetada)
		{
			ColetarEstrela();
		}
	}
	#endregion

	/// <summary>
	/// Método utilizado para Coletar essa Estrela
	/// </summary>
	private void ColetarEstrela()
	{
		foiColetada = true;

		//Manda a fonte de áudio tocar, utilizando o clipe salvo nela
		aSource.Play();

		StartCoroutine(Desativar());
	}

	/// <summary>
	/// Corotina que espera o tempo definido para desativar essa estrela e contabilizar sua adição
	/// </summary>
	/// <returns></returns>
	private IEnumerator Desativar()
	{
		yield return timerParaDesativar;

		ControladorJogo.Instancia.AdicionarEstrela();
		spRenderer.enabled = false;
	}
}
