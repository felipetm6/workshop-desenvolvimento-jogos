using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorJogo : MonoBehaviour
{
	//Instância que torna esse script acessível por outros scripts sem uma referência
	public static ControladorJogo Instancia;

	//Declara a Assinatura de um método para utilizá-lo como Evento
	public delegate void EstrelaColetada(int qtde);
	//Declara o Evento que será assinado por outros scripts
	public event EstrelaColetada EventoEstrelaColetada;
	//Variável de controle da quantidade de estrelas
	private int qtdeEstrelas;

	//Declara a Assinatura de um método para utilizá-lo como Evento
	public delegate void MoedaColetada(int qtde);
	//Declara o Evento que será assinado por outros scripts
	public event MoedaColetada EventoMoedaColetada;
	//Variável de controle da quantidade de moedas
	private int qtdeMoedas;

	//Recebe o nome da cena atual
	private string cenaAtual;

	//Cache do timer utilizado para reiniciar o jogo
	private WaitForSeconds timerParaReiniciar = new WaitForSeconds(1f);

	#region Metodos da Unity
	//Função de Evento chamada ao rodar a cena
	private void Awake()
	{
		//Esta estrutura de controle serve para configurar a instância dessa classe
		if (Instancia == null)
		{
			Instancia = this;
		}
		else if (Instancia != this)
		{
			Destroy(gameObject);
		}

		//Acessa o Gerenciador de Cenas (SceneManager) para pegar o nome (.name) da Cena Ativa (GetActiveScene())
		cenaAtual = SceneManager.GetActiveScene().name;
	}
	#endregion

	/// <summary>
	/// Acresce a quantidade de Moedas e dispara o Evento
	/// </summary>
	public void AdicionarMoeda()
	{
		//Acresce a quantidade de moedas
		qtdeMoedas++;

		//Se alguém assinou o evento (condição), enviar um Evento com a quantidade de moedas como Argumento
		if (EventoMoedaColetada != null)
		{
			EventoMoedaColetada(qtdeMoedas);
		}
	}

	/// <summary>
	/// Acresce a quantidade de Estrelas e dispara o Evento
	/// </summary>
	public void AdicionarEstrela()
	{
		//Acresce a quantidade de estrelas
		qtdeEstrelas++;

		//Se alguém assinou o evento (condição), enviar um Evento com a quantidade de estrelas como Argumento
		if (EventoEstrelaColetada != null)
		{
			EventoEstrelaColetada(qtdeEstrelas);
		}
	}

	/// <summary>
	/// Efetua toda a lógica para a finalização do jogo
	/// </summary>
	public void GanharJogo()
	{
		if (cenaAtual == "Mundo-01")
		{
			SceneManager.LoadScene("Mundo-02");
		}
		else
		{
			SceneManager.LoadScene("Final");
		}
	}

	/// <summary>
	/// Reinicia o jogo na cena atual
	/// </summary>
	public void ReiniciarJogo()
	{
		StartCoroutine(Reiniciar());
	}

	/// <summary>
	/// Invoca a corotina para reiniciar o jogo, e reinicia ao final do tempo
	/// </summary>
	/// <returns></returns>
	private IEnumerator Reiniciar()
	{
		yield return timerParaReiniciar;

		SceneManager.LoadScene(cenaAtual);
	}
}
