using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porta : MonoBehaviour
{
	private const string player = "Player";

	//Define a quantidade de estrelas necessárias para liberar o fim do jogo
	[SerializeField]
	private int estrelasNecessarias = 5;

	//Bandeira que define se o jogador está na área da Porta
	private bool jogadorEstaNaArea;
	//Bandeira que define se a quantidade de estrelas 
	private bool saidaLiberada;

	//Cache para o GameObject que representa o bloqueio da porta
	[SerializeField]
	private GameObject bloqueio;

	#region Metodos da Unity
	//Função de Evento chamada sempre quando o GameObject é ativado
	private void OnEnable()
	{
		AoPegarEstrela(0);
	}

	//Função de Evento chamada sempre quando o GameObject é desativado
	private void OnDisable()
	{
		//Acessa a Instância da classe ControladorJogo (ControladorJogo.Instance) e remove a assinatura do evento EstrelaColetada
		ControladorJogo.Instancia.EventoEstrelaColetada -= AoPegarEstrela;
	}

	//Função de Evento chamada ao rodar a cena
	private void Start()
	{
		//Acessa a Instância da classe ControladorJogo (ControladorJogo.Instance) e assina o evento EstrelaColetada
		ControladorJogo.Instancia.EventoEstrelaColetada += AoPegarEstrela;
	}

	//Funçã de Evento chamada uma vez por frame, todo frame
	private void Update()
	{
		//Se o jogador está dentro da área da porta e a saída está liberada (estrelasColetadas > estrelasNecessarias)
		if (jogadorEstaNaArea && saidaLiberada)
		{
			//Se o jogador aperta para cima (Seta para cima ou W)
			if (Input.GetAxis("Vertical") > 0)
			{
				//Chama o método de Vitória dentro da Instância de ControladorJogo
				ControladorJogo.Instancia.GanharJogo();
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
	/// Método que é disparado no evento de Coleta de Estrela
	/// </summary>
	/// <param name="qtde">Quantidade de Estrelas</param>
	private void AoPegarEstrela(int qtde)
	{
		//Se a quantidade de estrelas coletadas for maior do que a necessária, remover o bloqueio e ajustar a bandeira
		if (qtde >= estrelasNecessarias)
		{
			bloqueio.SetActive(false);
			saidaLiberada = true;
		}
	}
}
