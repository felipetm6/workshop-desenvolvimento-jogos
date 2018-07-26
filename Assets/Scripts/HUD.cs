using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	//Cache do GameObject de Texto do Canvas para o Contador de Moedas
	[SerializeField]
	private Text contadorMoedas;

	//Cache do GameObject de Texto do Canvas para o Contador de Estrelas
	[SerializeField]
	private Text contadorEstrelas;

	#region Metodos da Unity
	//Função de Evento chamada sempre quando o GameObject é desativado
	private void OnDisable()
	{
		//Acessa a Instância da classe ControladorJogo (ControladorJogo.Instance) e remove a assinatura do evento MoedaColetada
		ControladorJogo.Instancia.EventoMoedaColetada -= AoPegarMoeda;
		//Acessa a Instância da classe ControladorJogo (ControladorJogo.Instance) e remove a assinatura do evento EstrelaColetada
		ControladorJogo.Instancia.EventoEstrelaColetada -= AoPegarEstrela;
	}

	//Função de Evento chamada ao rodar a cena
	private void Start()
	{
		//Acessa a Instância da classe ControladorJogo (ControladorJogo.Instance) e assina o evento MoedaColetada
		ControladorJogo.Instancia.EventoMoedaColetada += AoPegarMoeda;
		//Acessa a Instância da classe ControladorJogo (ControladorJogo.Instance) e assina o evento EstrelaColetada
		ControladorJogo.Instancia.EventoEstrelaColetada += AoPegarEstrela;
	}
	#endregion

	/// <summary>
	/// Método que é disparado no evento de Coleta de Moeda
	/// </summary>
	/// <param name="qtde">Quantidade de Moedas</param>
	private void AoPegarMoeda(int qtde)
	{
		//Atualiza o objeto de Texto do Canvas da Unity
		contadorMoedas.text = qtde.ToString();
	}

	/// <summary>
	/// Método que é disparado no evento de Coleta de Estrela
	/// </summary>
	/// <param name="qtde">Quantidade de Estrelas</param>
	private void AoPegarEstrela(int qtde)
	{
		//Atualiza o objeto de Texto do Canvas da Unity
		contadorEstrelas.text = qtde.ToString();
	}
}
