using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDice : MonoBehaviour
{
    public GameObject diceGameObject;
    public GameObject superKingIconGameObject;
    public Sprite[] diceFace;
    public Sprite zeroFace;
    public PolygonCollider2D diceCollider;
    public Color normalColor;
    public Color hiddenColor;

    public int diceID;
    GameLogic GameLogicRef;
    AudioFX AudioFXRef;
    float timeForUnrollNRoll;

    public void OnMouseDown()
    {
        //Debug.Log("I AM CLICKED => " + gameObject.name);
        //Disable my all ongoing tweens and perform click
        StopTapDiceTween();
        GameLogicRef.GenerateDiceCount(diceID);
        if (GameLogicRef.TURN_INDEX == 0)
        {
            PhotonController.instance.RedlayerDice(diceID);
        }
        else
        {
            PhotonController.instance.GreenlayerDice(diceID);
        }
        
       
    }

    public void PlayRollDiceTween(int rolledDiceFace)
    {
        //DEBUG
        //if (rolledDiceFace >= 6)
        //{
        //	rolledDiceFace = 5;
        //}
        diceGameObject.transform.localScale = Vector2.one;
        diceGameObject.transform.localRotation = Quaternion.identity;

        LeanTween.rotateZ(diceGameObject, -720F, timeForUnrollNRoll).setEaseOutQuart();
        AudioFXRef.DiceUnRoll();
        LeanTween.scale(diceGameObject, Vector2.one * 0.1F, timeForUnrollNRoll).setEaseOutQuart().setOnComplete(() =>
        {
            diceGameObject.GetComponent<SpriteRenderer>().sprite = diceFace[rolledDiceFace - 1];
            LeanTween.rotateZ(diceGameObject, 720F, timeForUnrollNRoll).setEaseOutQuart();
            AudioFXRef.DiceRoll();
            LeanTween.scale(diceGameObject, Vector2.one, timeForUnrollNRoll).setEaseOutQuart();
        });
    }

    public void SetDiceProperties(int _id, GameLogic _gameLogicRef, AudioFX _AudioFXRef)
    {
        diceID = _id;
        GameLogicRef = _gameLogicRef;
        AudioFXRef = _AudioFXRef;
        gameObject.SetActive(true);
        timeForUnrollNRoll = GameLogicRef.SPEED_FOR_ROLLDICE / 2.0F;
    }

    public void EnableCollider()
    {
        if (!diceCollider.enabled)
        {
            diceCollider.enabled = true;
        }
    }

    public void DisableCollider()
    {
        if (diceCollider.enabled)
        {
            diceCollider.enabled = false;
        }
    }

    public void PlayTapDiceTween()
    {
        superKingIconGameObject.SetActive(true);
        LeanTween.scale(superKingIconGameObject, Vector2.one * 0.3F, 1F).setEase(LeanTweenType.punch).setLoopCount(-1);
    }

    public void StopTapDiceTween()
    {
        superKingIconGameObject.SetActive(false);

        if (LeanTween.isTweening(superKingIconGameObject))
        {
            LeanTween.cancel(superKingIconGameObject, false);
            superKingIconGameObject.transform.localScale = Vector2.one * 0.6F;
        }
    }

    public void ShowDice()
    {
        diceGameObject.GetComponent<SpriteRenderer>().color = normalColor;
        diceGameObject.GetComponent<SpriteRenderer>().sprite = zeroFace;

        superKingIconGameObject.GetComponent<SpriteRenderer>().color = normalColor;
    }

    public void HideDice()
    {
        diceGameObject.GetComponent<SpriteRenderer>().color = hiddenColor;
        diceGameObject.GetComponent<SpriteRenderer>().sprite = zeroFace;

        superKingIconGameObject.GetComponent<SpriteRenderer>().color = hiddenColor;
        superKingIconGameObject.SetActive(true);
    }

    public void InitializeScaleAndRotation()
    {
        diceGameObject.transform.localScale = Vector2.one;
        diceGameObject.transform.localRotation = Quaternion.identity;
    }
}
