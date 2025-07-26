using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BonusGameSuitCase : MonoBehaviour
{
    [SerializeField] private SocketIOManager socketManager; 

    [SerializeField] private Button suitcase;
    [SerializeField] private Sprite empty_case;
    [SerializeField] private Sprite filled_case_cash;
    [SerializeField] private Sprite filled_case_gold;
    [SerializeField] private Sprite case_normal;
    [SerializeField] private Color32 text_color;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image case_image_bttom;
    [SerializeField] private Image case_image_up;
    [SerializeField] private ImageAnimation imageAnimation;
    [SerializeField] private BonusController _bonusManager;

    [SerializeField]
    internal bool isOpen;
    internal int index=0;

    private Tween shakeTween;

    void Start()
    {
        if (suitcase) suitcase.onClick.RemoveAllListeners();
        if (suitcase) suitcase.onClick.AddListener(OpenCase);
    }

    internal void ResetCase(int i)
    {
        index = i;
        if (case_image_up) case_image_up.sprite = case_normal;
        isOpen = false;
        text.gameObject.SetActive(false);
    }

    void OpenCase()
    {
        if (isOpen)
            return;
        if (_bonusManager.isAnimating)
            return;
        _bonusManager.enableRayCastPanel(true);
        _bonusManager.isAnimating = true;
       
        imageAnimation.StartAnimation();

        StartCoroutine(setCase());
    }

    void PopulateCase()
    {
        double value = socketManager.bonusData.payload.payout;
        
        if (value == 0)
        {
            case_image_bttom.sprite = empty_case;
            text.text = "game over";
        }
        else
        {
            case_image_bttom.sprite = filled_case_cash;
            text.text = socketManager.bonusData.payload.winAmount.ToString();
            _bonusManager.totalWin += socketManager.bonusData.payload.winAmount;
        }
    }

    IEnumerator setCase()
    {
        
       if(socketManager!=null) socketManager.OnBonusCollect(index);
       
        if (socketManager != null) yield return new WaitUntil(() => socketManager.isResultdone);
         yield return new WaitUntil(() => !imageAnimation.isplaying);
        yield return new WaitForSeconds(0.3f);
      
        PopulateCase();

        text.gameObject.SetActive(true);
        text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, text_color);
        isOpen = true;
        if (text.text == "game over")
        {
            _bonusManager.enableRayCastPanel(true);
            _bonusManager.PlayWinLooseSound(false);
            yield return new WaitForSeconds(1f);
            _bonusManager.GameOver();
        }
        else
        {
            _bonusManager.PlayWinLooseSound(true);
            _bonusManager.enableRayCastPanel(false);
        }
        _bonusManager.isAnimating = false;
    }

}
