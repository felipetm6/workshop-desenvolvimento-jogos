using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bau : MonoBehaviour
{
	private const string player = "Player";
	private const string animacaoAbrir = "Abrir";

	//Bandeira que define se o jogador está na área do Baú
	private bool jogadorEstaNaArea;
	//Bandeira que define se o baú já foi aberto, evitando um evento duplo
	private bool foiAberto;

	//Cache do Animator desse objeto
	private Animator animator;
	//Cache da Fonte de Áudio do Baú
	[SerializeField]
	private AudioSource bauAudioSource;
	//Cache da Fonte de Áudio da Estrela dentro do Baú
	[SerializeField]
	private AudioSource estrelaAudioSource;

	#region Metodos da Unity
	//Função de Evento chamada ao rodar a cena
	private void Awake()
	{
		//Pega o Componente Animator nesse objeto (GetComponent) e atribui a animator
		animator = GetComponent<Animator>();
	}

	//Função de Evento chamada ao rodar a cena
	private void Start()
	{

	}

	//Funçã de Evento chamada uma vez por frame, todo frame
	private void Update()
	{
		//Se o jogador está dentro da área da porta e a saída está liberada (estrelasColetadas > estrelasNecessarias)
		if (jogadorEstaNaArea && !foiAberto)
		{
			//Se o jogador aperta para cima (Seta para cima ou W)
			if (Input.GetAxis("Vertical") > 0)
			{
				AbrirBau();
			}
		}
	}

	//Função de Evento chamada sempre que um Colisor entra na Trigger da Porta
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Verifica se o Colisor tem a mesma tag do Jogador
		if (collision.gameObject.CompareTag(player))
		{
			jogadorEstaNaArea = true;
		}
	}

	//Função de Evento chamada sempre que um Colisor sai da Trigger da Porta
	private void OnTriggerExit2D(Collider2D collision)
	{
		//Verifica se o Colisor tem a mesma tag do Jogador
		if (collision.gameObject.CompareTag(player))
		{
			jogadorEstaNaArea = false;
		}
	}
	#endregion

	/// <summary>
	/// Método a ser executado para abrir o Baú
	/// </summary>
	private void AbrirBau()
	{
		foiAberto = true;

		//Envia um Trigger para o Animator para que a animação seja executada
		animator.SetTrigger(animacaoAbrir);
	}

	/// <summary>
	/// Método para tocar o Áudio de coleta da Estrela e contabilizá-la
	/// </summary>
	public void AdicionarEstrela()
	{
		estrelaAudioSource.Play();

		ControladorJogo.Instancia.AdicionarEstrela();
	}
}
