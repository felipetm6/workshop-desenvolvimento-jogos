using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Esmagador : MonoBehaviour
{
	//Variável que salva a posição inicial desse objeto no mundo
	private Vector3 posicaoInicial;
	//Bandeira que define se o inimigo está carregando ou não
	private bool estaRecarregando;

	//Timer para recarregar o Inimigo
	private WaitForSeconds tempoParaRecarregar = new WaitForSeconds(2f);

	//Cache da Fonte de Áudio desse objeto
	private AudioSource aSource;
	//Cache do Rigibody 2D desse objeto
	private Rigidbody2D rb2D;

	#region Metodos da Unity
	//Função de Evento chamada ao rodar a cena
	private void Awake()
	{
		//Pega o Componente AudioSource nesse objeto (GetComponent) e atribui a aSource
		aSource = GetComponent<AudioSource>();
		//Pega o Componente Rigidbody2D nesse objeto (GetComponent) e atribui a rb2D
		rb2D = GetComponent<Rigidbody2D>();
	}

	//Função de Evento chamada ao rodar a cena
	private void Start()
	{
		//Atribui a posição do transform à variável posicaoInicial
		posicaoInicial = transform.position;
	}

	//Função de Evento chamada ao rodar a cena
	private void Update()
	{
		//Se não estiver carregando, verifica a distância entre jogador e inimigo, e responde de acordo
		//Se estiver carregando, volta para sua posição inicial
		if (!estaRecarregando)
		{
			var dist = 0f;

			//Atribui a posição em X do jogador a variável alvo
			var alvo = Jogador.Instancia.transform.position.x;
			//Atribui a posição em X a variável posicao
			var posicao = transform.position.x;

			dist = alvo - posicao;

			if (dist < 0)
			{
				dist *= -1;
			}

			//Se a distancia for menor que 6, e o objeto estiver estático, libera seu ataque
			if (dist < 6f && rb2D.bodyType == RigidbodyType2D.Static)
			{
				//Liga a física no Rigibody2D
				rb2D.bodyType = RigidbodyType2D.Dynamic;
			}
		}
		else
		{
			//Pega uma posição entre position e posicaoInicial e atribiu a posição desse transform
			transform.position = Vector3.Lerp(transform.position, posicaoInicial, .5f * Time.deltaTime);

			var dist = Vector3.Distance(posicaoInicial, transform.position);

			//Se a dist for menor que 1, alcançou seu alvo
			if (dist < 1)
			{
				estaRecarregando = false;
			}
		}
	}

	//Função de Evento chamada sempre que o inimigo acerta um Colisor
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Chao"))
		{
			aSource.Play();
			StartCoroutine(Recarregar());
		}
	}
	#endregion

	/// <summary>
	/// Corotina que espera o tempo definido para o inimigo iniciar sua recuperação
	/// </summary>
	/// <returns></returns>
	private IEnumerator Recarregar()
	{
		yield return tempoParaRecarregar;

		rb2D.bodyType = RigidbodyType2D.Static;

		estaRecarregando = true;
	}
}
