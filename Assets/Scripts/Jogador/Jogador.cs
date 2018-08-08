//#define EXERCICIO_AULA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Define que ao usar Random neste script, estamos nos referindo à função random da UnityEngine, e não a de System
using Random = UnityEngine.Random;

#if !EXERCICIO_AULA
public class Jogador : MonoBehaviour
{
	//Variável que será utilizada para o Singleton
	public static Jogador Instancia;

	//Variável que define qual a Força do Movimento (horizontal) da personagem
	private Vector2 forcaMovimento = new Vector2(0.25f, 8.7f);
	//Variável que recebe a informação de colisão do RaycastNonAlloc
	private RaycastHit2D[] results = new RaycastHit2D[16];

	//Bandeira utilizada para definir se a personagem está andando
	private bool estaAndando;
	//Variável que recebe o Input (Entrada) do Usuário
	private float eixoHorizontal;
	//Variável que salva o tempo que a personagem está andando
	private float tempoAndando;
	//Variável que define o tempo necessário para atingir a Velocidade Máxima
	private float tempoParaVelMax = 3f;
	//Variável que define qual a Velocidade Máxima da personagem
	private float velMax = 5f;

	//Bandeira utilizada para definir se a personagem está pulando
	private bool estaPulando;
	//Variável que salva o tempo que a personagem está pulando
	private float tempoPulo;
	//Variável que define o tempo necessário para encerrar o pulo
	private float tempoMaxPulo = 1f;
	//Bandeira utilizada para definir se a personagem está no chão
	private bool estaNoChao;

	//Cache do GameObject que contém os pedaços do personagem que fazem o efeito de Morte
	[SerializeField]
	private GameObject animacaoMorte;
	//Bandeira utilizada para definir se a personagem está morta
	private bool estaMorto;

	//Cache do AudioClip utilizado para sonorizar o Pulo
	[SerializeField]
	private AudioClip somPulo;
	//Cache do AudioClip utilizado para sonorizar a Morte
	[SerializeField]
	private AudioClip somMorte;

	//Cache do Rigidbody2D desse objeto
	private Rigidbody2D rb2D;
	//Cache do Animator desse objeto
	private Animator animator;
	//Cache da Audio Source desse objeto
	private AudioSource aSource;
	//Cache do Sprite Renderer desse objeto
	private SpriteRenderer spRenderer;

	#region Metodos da Unity
	//Função de Evento chamada ao rodar a cena
	private void Awake()
	{
		//Esse é um método de estabelecer um Singleton utilizado na Unity
		//Se a Instância for nula, ela é igual esse objeto. Se ela for diferente desse objeto, então há dois Jogadores na cena, e esse objeto pode ser apagado tranquilamente
		if (Instancia == null)
		{
			Instancia = this;
		}
		else if (Instancia != this)
		{
			Destroy(gameObject);
		}

		//Pega o Componente Rigibody2D nesse objeto (GetComponent) e atribui a rb2D
		rb2D = GetComponent<Rigidbody2D>();
		//Pega o Componente Animator nesse objeto (GetComponent) e atribui a animator
		animator = GetComponent<Animator>();
		//Pega o Componente AudioSource nesse objeto (GetComponent) e atribui a aSource
		aSource = GetComponent<AudioSource>();
		//Pega o Componente SpriteRenderer nesse objeto (GetComponent) e atribui a spRenderer
		spRenderer = GetComponent<SpriteRenderer>();
	}

	//Função de Evento chamada ao rodar a cena
	private void Start()
	{
		//Inicia a Coroutine que fará a personagem piscar
		StartCoroutine(PiscarAleatoriamente());
	}

	//Função de Evento chamada todo frame
	//O Update é recomendado para coletar a Entrada (Input) do usuário
	private void Update()
	{
		//Se estiver morto, não é necessário captar ou processar a Entrada
		if (estaMorto) return;

		ReceberEntradas();
		ProcessarEntradas();
		ProcessarAnimacao();
	}

	//Função de Evento chamada uma vez por frame
	//O FixedUpdate é recomendado para realizar movimentações e ações relacionadas à Física
	private void FixedUpdate()
	{
		//Se estiver morto, não é necessário fazer a movimentação
		if (estaMorto) return;

		//Chama a Função que verifica se o Jogador está no chão ou não
		VerificarChao();

		//Cria uma variável velocidadeX, que corresponde a velocidade no eixo X do Rigidbody2D junto com a Entrada do usuário
		var velocidadeX = 0f;

		//Se o eixoHorizontal (Entrada) for diferente de zero, define que a velocidade em X é a velocidade máxima
		//Se o eixoHorizontal (Entrada) for igual a zero, define que a velocidade em X também será zero (através da multiplicação)
		velocidadeX = velMax * eixoHorizontal;

		//Uma forma de fazer o pulo utilizando a física da Unity é alterar o impacto da gravidade nesse Rigidbody
		//Atribui o impacto da gravidade atual na variável velocidadeY
		var velocidadeY = rb2D.gravityScale;

		//Se estiver pulando
		if (estaPulando)
		{
			//Se ele está pulando, a gravidade deve diminuir
			velocidadeY -= .1f * Time.fixedDeltaTime;
		}
		else
		{
			//Se não está pulando e não está no chão, logo ele está caindo
			//Se ele está caindo, a gravidade deve aumentar
			if (!estaNoChao)
			{
				velocidadeY += .2f * Time.fixedDeltaTime;
			}
		}

		//Cria uma variável que recebe a velocidade atual do Rigidbody2D
		//Isso é feito para evitar utilizar new Vector2(), porque o código ficaria maior e menos legível
		//A única variável da velocidade que deverá ser alterada é a X
		var movimento = rb2D.velocity;
		movimento.x = velocidadeX;

		//Define que a velocidade do Rigidbody2D é igual à nova velocidade em X e a antiga velocidade em Y
		rb2D.velocity = movimento;
		//Atualiza o funcionamento da gravidade do Rigidbody2D
		rb2D.gravityScale = velocidadeY;
	}

	//Função de Evento chamada sempre que um Colisor colide com outro
	private void OnCollisionEnter2D(Collision2D collision)
	{
		//Se o objeto de colisão tiver uma tag de Inimigo ou morte e o jogador não estiver morto, chama a função Morrer
		if (collision.gameObject.CompareTag("Inimigo") || collision.gameObject.CompareTag("Morte") && !estaMorto)
		{
			Morrer();
		}
	}

	//Função de Evento chamada sempre que um Colisor entra em um Trigger
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Se a área de Trigger tiver uma tag de Inimigo ou morte e o jogador não estiver morto, chama a função Morrer
		if (collision.gameObject.CompareTag("Inimigo") || collision.gameObject.CompareTag("Morte") && !estaMorto)
		{
			Morrer();
		}
	}
	#endregion

	/// <summary>
	/// Função responsável por receber todas as Entradas (Inputs) do Usuário
	/// </summary>
	private void ReceberEntradas()
	{
		//Atribui a Entrada do Eixo Horizontal (padrão da Unity) à variável
		eixoHorizontal = Input.GetAxis("Horizontal");

		//Se apertar o botão de Pulo e estiver no chão, configura o pulo
		if (Input.GetButtonDown("Jump") && estaNoChao)
		{
			estaNoChao = false;
			estaPulando = true;

			//Configura a velocidade do Rigidbody2D para adicionar o impulso de pulo
			rb2D.velocity = new Vector2(rb2D.velocity.x, forcaMovimento.y);

			//Define que o Clip da Source agora é o som de Pulo dessa personagem, e toca em seguida
			aSource.clip = somPulo;
			aSource.Play();
		}
		//Se soltar o botão de Pulo
		else if (Input.GetButtonUp("Jump"))
		{
			estaPulando = false;
		}
	}

	/// <summary>
	/// Função responsável por tratar as Entradas recebidas
	/// </summary>
	private void ProcessarEntradas()
	{
		//Se o Eixo Horizontal for diferente de zero, há movimento
		if (eixoHorizontal != 0)
		{
			//Define a bandeira de movimento
			estaAndando = true;
			//Adiciona na variável de cache da duração do movimento o tempo de duração
			tempoAndando += Time.deltaTime;

			//A propriedade flipX define se a Sprite desse SpriteRenderer é invertida no eixo horizontal
			//Se o EixoHorizontal não for maior que zero, flipX é verdadeiro
			spRenderer.flipX = !(eixoHorizontal > 0);
		}
		else
		{
			tempoAndando = 0f;
			estaAndando = false;
		}

		//Se estiver pulando
		if (estaPulando)
		{
			//Se o tempo do pulo for maior do que o tempo máximo de pulo
			if (tempoPulo > tempoMaxPulo)
			{
				tempoPulo = 0f;
				estaPulando = false;
			}
			else
			{
				tempoPulo += Time.deltaTime;
			}
		}
	}

	/// <summary>
	/// Função responsável pela Animação
	/// </summary>
	private void ProcessarAnimacao()
	{
		//Atribui a definição de Andando a variável estaAndando
		//Se a personagem está no chão e eixoHorizontal (que contém a Entrada do jogador) é diferente de zero
		var estaAndando = estaNoChao && eixoHorizontal != 0;
		//Atribui a definição de Pulando a variável animacaoPulando
		//Se a personagem não está no chão e a velocidade do Rigidbody2D (correspondente à física) é maior que zero (ou seja, está subindo)
		var animacaoPulando = !estaNoChao && rb2D.velocity.y > 0;
		//Atribui a definição de Caindo a variável estaCaindo
		//Se a personagem não está no chão e a velocidade do Rigidbody2D (correspondente à física) é maior que zero (ou seja, está caindo)
		var estaCaindo = !estaNoChao && rb2D.velocity.y < 0;

		//Para definir uma variável do tipo Bool no animator, é preciso utilizar o código animator.SetBool("NomeDaVariavel", bool)
		//Atribui o valor estaAndando para a variável do Animator Correndo. Se a personagem está andando, a animação Correndo fica true e é chamada, se não, fica false e é encerrada
		animator.SetBool("Correndo", estaAndando);
		//Atribui o valor estaPulando para a variável do Animator Pulando. Se a personagem está pulando, a animação Pulando fica true e é chamada, se não, fica false e é encerrada
		animator.SetBool("Pulando", animacaoPulando);
		//Atribui o valor estaAndando para a variável do Animator Caindo. Se a personagem está caindo, a animação Caindo fica true e é chamada, se não, fica false e é encerrada
		animator.SetBool("Caindo", estaCaindo);
	}

	private void VerificarChao()
	{
		results = new RaycastHit2D[16];

		if (Physics2D.RaycastNonAlloc(transform.position, -Vector3.up, results, 0.25f) > 0)
		{
			foreach (var result in results)
			{
				if (result.transform)
				{
					if (!result.transform.CompareTag("Chao"))
					{
						rb2D.gravityScale = 1;
						estaPulando = false;
						estaNoChao = false;
					}
					else
					{
						rb2D.gravityScale = 1;
						estaPulando = false;
						estaNoChao = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Função responsável por fazer todas as ações necessárias na morte do jogador
	/// </summary>
	private void Morrer()
	{
		estaMorto = true;

		//Define que o Clip da Source agora é o som de Morte dessa personagem, e toca em seguida
		aSource.clip = somMorte;
		aSource.Play();

		//Desliga o SpriteRenderer para que a animação de morte possa acontecer apropriadamente
		spRenderer.enabled = false;
		animacaoMorte.SetActive(true);

		//Manda o Controlador de Jogo reiniciar o jogo
		ControladorJogo.Instancia.ReiniciarJogo();
	}

	/// <summary>
	/// Coroutine responsável por fazer o personagem piscar em intervalos aleatórios
	/// </summary>
	/// <returns></returns>
	private IEnumerator PiscarAleatoriamente()
	{
		//Define um valor aleatório entre 5 e 15 e atribui a uma variável
		//Random.Range(minimo, maximo)
		var delay = Random.Range(5f, 15f);

		//Chama um intervalo que dura delay segundos
		yield return new WaitForSeconds(delay);

		//Chama a animação de piscar no Animator
		//Quando a variável é definida como Trigger no Animator, ela deve ser chamada como animator.SetTrigger("NomeDaVariavel")
		animator.SetTrigger("Piscar");

		//Recomeça essa Coroutine
		StartCoroutine(PiscarAleatoriamente());
	}
}

#else
public class Jogador : MonoBehaviour
{
	//Variável que será utilizada para o Singleton
	public static Jogador Instancia;

#region Metodos da Unity
	//Função de Evento chamada ao rodar a cena
	private void Awake()
	{
		//Esse é um método de estabelecer um Singleton utilizado na Unity
		//Se a Instância for nula, ela é igual esse objeto. Se ela for diferente desse objeto, então há dois Jogadores na cena, e esse objeto pode ser apagado tranquilamente
		if (Instancia == null)
		{
			Instancia = this;
		}
		else if (Instancia != this)
		{
			Destroy(gameObject);
		}
	}

	//Função de Evento chamada ao rodar a cena
	private void Start()
	{

	}

	//Função de Evento chamada todo frame
	//O Update é recomendado para coletar a Entrada (Input) do usuário
	private void Update()
	{

	}

	//Função de Evento chamada uma vez por frame
	//O FixedUpdate é recomendado para realizar movimentações e ações relacionadas à Física
	private void FixedUpdate()
	{

	}

	//Função de Evento chamada sempre que um Colisor colide com outro
	private void OnCollisionEnter2D(Collision2D collision)
	{

	}

	//Função de Evento chamada sempre que um Colisor entra em um Trigger
	private void OnTriggerEnter2D(Collider2D collision)
	{

	}
#endregion
}
#endif