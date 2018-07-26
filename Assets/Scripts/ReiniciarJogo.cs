using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReiniciarJogo : MonoBehaviour
{
	//Cache do timer utilizado para reiniciar o jogo
	private WaitForSeconds timerParaReiniciar = new WaitForSeconds(2f);

	#region Metodos da Unity
	//Função de Evento que roda uma vez ao início da cena
	private void Start()
	{
		StartCoroutine(Reiniciar());
	}
	#endregion

	/// <summary>
	/// Invoca a corotina para reiniciar o jogo, e reinicia ao final do tempo
	/// </summary>
	/// <returns></returns>
	private IEnumerator Reiniciar()
	{
		yield return timerParaReiniciar;

		SceneManager.LoadScene("Mundo-01");
	}
}