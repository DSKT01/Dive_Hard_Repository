﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UberAudio;

#region Propuesta Diego
//public class Swipe : PassiveMechanics
//{
//    Vector2 intial, final, vecMov;
//    [SerializeField]
//    float minDis, magnitud;
//    Rigidbody2D me;
//    protected override void Execution() //start
//    {
//        me = GetComponent<Rigidbody2D>();
//    }
//    private void FixedUpdate()
//    {
//        if (Input.GetButtonDown("Fire1"))
//            intial = Input.mousePosition;
//        if (Input.GetButtonUp("Fire1"))
//        {
//            final = Input.mousePosition;
//            vecMov = (final - intial).normalized;
//            if (Vector2.Distance(final, intial) >= minDis)
//                me.AddForce(vecMov * magnitud, ForceMode2D.Impulse);
//        }
//    }
#endregion

public class Swipe : PassiveMechanics {

    //Puntero
    public RectTransform puntero;

	//Cosas para el counter del swipe
	[SerializeField]
	public GameObject swipeCounter;
	Image[] counterSprites = new Image[5];
	Vector3 counterPos = new Vector3 (-530 , 120 , 0);
	Color baseColor;

	bool tap, isUp, isDraging = false;
	int counter;
	float timer;
	float multiplicador = 1;

	[SerializeField]
	float coldDown, magnitud;
	[SerializeField]
	int maxCounter;

	Vector2 starTouch, swipeDelta;
	Rigidbody2D mRB;

	[SerializeField]
	GameObject BloodParticles;

    //Animacion Instrucciones
    Animator swipeAnim;
    bool hasSwiped = false;
    Lanzador lanzador;

	protected override void Execution()
	{
		mRB = GetComponent<Rigidbody2D>();
		counter = maxCounter;

		for (int i = 0; i < maxCounter; i++)
		{
			GameObject g = Instantiate(swipeCounter,GameObject.Find("Canvas").GetComponent<RectTransform>().position + counterPos - new Vector3(0,counterPos.y / 1.8f ,0)*i,Quaternion.EulerAngles(swipeCounter.transform.rotation.eulerAngles));
			g.transform.SetParent(GameObject.Find("Canvas").transform);
			counterSprites[i] = (g.GetComponent<Image>());
		}
		baseColor = counterSprites[0].color;
        swipeAnim = GameObject.Find("Swipe-Animator").GetComponent<Animator>();
        lanzador = GameObject.Find("Flecha").GetComponent<Lanzador>();
    }

    void Update()
    {
        tap = false;

        #region Mouse Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            isDraging = true;
            starTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Reset();
        }

        #endregion

        #region Mobile Inputs
        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                isDraging = true;
                if (starTouch == Vector2.zero)
                    starTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                Reset();
            }
        }

        #endregion

        swipeDelta = Vector2.zero;
        //Calcular Distancia
        if (isDraging)
        {

            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - starTouch;

            if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - starTouch;

        }


        //Pasar la Zona muerta
        if (swipeDelta.magnitude > 200 && counter > 0)
        {
            GetComponent<Player>().bloodInGame += (GetComponent<Player>().swipe * GetComponent<Player>().venenoMult); //suma sangre 
            mRB.AddForce(multiplicador * swipeDelta.normalized * magnitud, ForceMode2D.Impulse);
            counter--;
            BloodSplash(10, 100, 250, transform);

            AudioManager.Instance.Play("Swipe");
            for (int i = counter; i < maxCounter; i++)
            {
                counterSprites[i].color = new Color(baseColor.r, baseColor.g, baseColor.b, 0);
            }

            hasSwiped = true;

            swipeAnim.SetBool("swipe", false);

            Reset();
        }

        //CooldDown
        if (counter < maxCounter)
        {

            if (!isUp)
            {
                timer += Time.deltaTime;
                counterSprites[counter].color = new Color(baseColor.r, baseColor.g, baseColor.b, timer / coldDown);
                if (timer >= coldDown)
                    isUp = true;
            }
            else
            {
                timer = 0;
                isUp = false;
                counterSprites[counter].color = new Color(baseColor.r, baseColor.g, baseColor.b, 1);
                counter++;
            }
        }

        //print(counter);

        if (lanzador.launched == true)
        {
            if (hasSwiped == false)
            {
                StartCoroutine("SwipeInstruction");
            }
        }
        //Puntero
        if(counter != 0 )
        { 
            if (Input.touches.Length > 0)
                puntero.position = Input.touches[0].position;
            else
                puntero.position = new Vector3(3000, 0, 0);

            if (Input.GetMouseButton(0))
            {
                Vector3 tapPos = Input.mousePosition;
                puntero.position = new Vector3(tapPos.x, tapPos.y, transform.position.z);
            }
            else
                puntero.position = new Vector3(3000, 0, 0);
        }
        else
            puntero.position = new Vector3(3000, 0, 0);
    }

    IEnumerator SwipeInstruction()
    {


        yield return new WaitForSeconds(3);
        if (hasSwiped == false)
            swipeAnim.SetBool("swipe",true);
        
    }

    private void Reset()
	{
		starTouch = swipeDelta = Vector2.zero;
		isDraging = false;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.name == "Sigue PJ")
			this.enabled = false;

	}
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.name == "Sigue PJ")
            this.enabled = false;
    }

	protected void BloodSplash(float spawTime, float fuerzaDeLanzamiento, int cantidadDeParticulas, Transform player)
	{
		GameObject blood = Instantiate(BloodParticles, player.position, Quaternion.identity);
		ParticleSystem particle = blood.GetComponent<ParticleSystem>();



		particle.startSpeed = fuerzaDeLanzamiento;
		particle.emission.SetBurst(0, new ParticleSystem.Burst(0, (short)cantidadDeParticulas, (short)cantidadDeParticulas, 1, 0.010f));

		if (!particle.isPlaying)
			particle.Play();

		Destroy(blood, spawTime);
	}
}
