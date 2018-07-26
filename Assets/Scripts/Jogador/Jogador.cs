using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Jogador : MonoBehaviour
{
	//Variável que será utilizada para o Singleton
	public static Jogador Instancia;

	private bool estaAndando;
	private float eixoHorizontal;
	private float tempoAndando;
	private float tempoParaVelMax = 3f;
	private float velMax = 5f;

	private bool estaPulando;
	private float forcaPulo = 5f;
	private float tempoPulo;
	private float tempoMaxPulo = 1f;
	private bool estaNoChao;

	[SerializeField]
	private GameObject animacaoMorte;
	private bool estaMorto;

	[SerializeField]
	private AudioClip somPulo;
	[SerializeField]
	private AudioClip somMorte;

	private Rigidbody2D rb2D;
	private Animator animator;
	private AudioSource aSource;
	private SpriteRenderer spRenderer;

	#region Metodos da Unity
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

		rb2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		aSource = GetComponent<AudioSource>();
		spRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		StartCoroutine(PiscarAleatoriamente());
	}

	private void Update()
	{
		if (estaMorto) return;

		ReceberEntradas();
		ProcessarEntradas();
		ProcessarAnimacao();
	}

	private void FixedUpdate()
	{
		if (estaMorto) return;

		var xSpeed = (rb2D.velocity.x) + (0.25f * eixoHorizontal);
		xSpeed = Mathf.Clamp(xSpeed, -velMax, velMax);
		xSpeed = (eixoHorizontal != 0) ? velMax * eixoHorizontal : 0;

		var ySpeed = rb2D.gravityScale;
		ySpeed += (estaPulando) ? -(.1f * Time.fixedDeltaTime) : (!estaNoChao) ? (.2f * Time.fixedDeltaTime) : 0;

		var movement = rb2D.velocity;
		movement.x = xSpeed;

		rb2D.velocity = movement;
		rb2D.gravityScale = ySpeed;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!collision.gameObject.CompareTag("Chao"))
		{
			if (estaPulando)
			{
				estaPulando = false;
			}

			if (collision.gameObject.CompareTag("Inimigo") || collision.gameObject.CompareTag("Morte") && !estaMorto)
			{
				Morrer();
			}
		}
		else
		{
			rb2D.gravityScale = 1;
			estaNoChao = true;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Inimigo") || collision.gameObject.CompareTag("Morte") && !estaMorto)
		{
			Morrer();
		}
	}
	#endregion

	private void ReceberEntradas()
	{
		eixoHorizontal = Input.GetAxis("Horizontal");

		if (Input.GetButtonDown("Jump") && estaNoChao)
		{
			estaNoChao = false;
			estaPulando = true;

			rb2D.velocity = new Vector2(rb2D.velocity.x, 8.7f);

			aSource.clip = somPulo;
			aSource.Play();
		}
		else if (Input.GetButtonUp("Jump"))
		{
			estaPulando = false;
		}
	}

	private void ProcessarEntradas()
	{
		if (eixoHorizontal != 0)
		{
			estaAndando = true;
			tempoAndando += Time.deltaTime;

			spRenderer.flipX = !(eixoHorizontal > 0);
		}
		else
		{
			tempoAndando = 0f;
			estaAndando = false;
		}

		if (estaPulando)
		{
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

	private void ProcessarAnimacao()
	{
		var estaAndando = estaNoChao && rb2D.velocity.x != 0;
		var animacaoPulando = !estaNoChao && rb2D.velocity.y > 0;
		var estaCaindo = !estaNoChao && rb2D.velocity.y < 0;

		animator.SetBool("Correndo", estaAndando);
		animator.SetBool("Pulando", animacaoPulando);
		animator.SetBool("Caindo", estaCaindo);
	}

	private void Morrer()
	{
		estaMorto = true;

		aSource.clip = somMorte;
		aSource.Play();

		spRenderer.enabled = false;
		animacaoMorte.SetActive(true);

		ControladorJogo.Instancia.ReiniciarJogo();
	}

	private IEnumerator PiscarAleatoriamente()
	{
		var delay = Random.Range(5f, 15f);

		yield return new WaitForSeconds(delay);

		animator.SetTrigger("Piscar");

		StartCoroutine(PiscarAleatoriamente());
	}
}