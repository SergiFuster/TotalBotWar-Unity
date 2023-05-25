using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompressibleMenu : MonoBehaviour
{
    #region PUBLIC PROPERTIES
    [Header("Image Settings")]
    public GameObject MenuImage;
    public Sprite FoldedSprite;
    public Sprite UnfoldedSprite;
    [Header("Other Settings")]
    [Tooltip("Father of the options that you want to be folded/unfolded")]
    public GameObject Folder;
    [Range(100, 200)]
    public int Speed;
    #endregion
    #region PRIVATE PROPERTIES
    private bool Folded = false;
    private GridLayoutGroup gridLayout;
    private int originalPaddingTopValue;
    private float originalSpacingValueY;
    #endregion
    #region PRIVATE METHODS

    // Start is called before the first frame update
    void Start()
    {
        gridLayout = Folder.GetComponent<GridLayoutGroup>();
        originalPaddingTopValue = gridLayout.padding.top;
        originalSpacingValueY = gridLayout.spacing.y;
    }


    private IEnumerator Folding()
    {
        while(gridLayout.padding.top != 0 || gridLayout.spacing.y != -gridLayout.cellSize.y)
        {
            if (gridLayout.padding.top != 0) gridLayout.padding.top--;
            if (gridLayout.spacing.y != -gridLayout.cellSize.y) gridLayout.spacing = new Vector2(gridLayout.spacing.x, gridLayout.spacing.y - 1);
            yield return new WaitForSeconds(1 / Speed);
        }
        Folder.SetActive(false);
    }

    private IEnumerator Unfolding()
    {
        Folder.SetActive(true);
        while (gridLayout.padding.top != originalPaddingTopValue || gridLayout.spacing.y != originalSpacingValueY)
        {
            if (gridLayout.padding.top != originalPaddingTopValue) gridLayout.padding.top++;
            if (gridLayout.spacing.y != originalSpacingValueY) gridLayout.spacing = new Vector2(gridLayout.spacing.x, gridLayout.spacing.y + 1);
            yield return new WaitForSeconds(1 / Speed);
        }
    }

    private void Fold()
    {
        StartCoroutine(Folding());
        MenuImage.GetComponent<Image>().sprite = FoldedSprite;
        Folded = true;
    }

    private void Unfold()
    {
        StartCoroutine(Unfolding());
        MenuImage.GetComponent<Image>().sprite = UnfoldedSprite;
        Folded = false;
    }
    #endregion
    #region PUBLIC METHODS
    public void OnClick()
    {
        if (Folded) Unfold();
        else Fold();
    }
    #endregion
}
