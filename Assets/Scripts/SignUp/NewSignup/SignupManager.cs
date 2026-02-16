using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignupManager : MonoBehaviour
{
    public static SignupManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }
}