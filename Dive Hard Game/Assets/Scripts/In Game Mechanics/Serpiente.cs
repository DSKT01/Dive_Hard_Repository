﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serpiente : MundoConTienda
{
    [SerializeField]
    float poisonTime = 10;
    [SerializeField]
    float poisonMult = 0.5f;

    protected override void Action() //comportamiento pasivo
    {

    }

    protected override IEnumerator Activation(Player _jugador) //comportamiento activo
    {
        _jugador.Poison(poisonTime, poisonMult);
        Destroy(GetComponent<Serpiente>());
        //Aqui un sonido de envenenamiento
        //animación del choque con serpiente
        yield return null;
    }
}
