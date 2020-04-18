using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHandler : Singleton<DialogueHandler>
{
    [SerializeField]
    private SequentialText text;

    private void Start()
    {
        text.PlayMessage("fuel burns up quicker at higher speeds. go too fast and you'll waste it. but go too slow and the RAILWORKERS will make a pass at you.");
    }
}
