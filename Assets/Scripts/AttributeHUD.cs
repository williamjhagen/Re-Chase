using UnityEngine;
using System.Collections;

public class AttributeHUD : MonoBehaviour {
    public NPC npc;
    public GameObject aggressionHUD;
    public GameObject energyHUD;
    public GameObject braveryHUD;
    public GameObject spontaneityHUD;

    private RectTransform aggressionBar;
    private RectTransform energyBar;
    private RectTransform braveryBar;
    private RectTransform spontaneityBar;

    private Vector3 aggressionBasePos;
    private Vector3 energyBasePos;
    private Vector3 braveryBasePos;
    private Vector3 spontaneityBasePos;

    private float aggressionMax;
    private float energyMax;
    private float braveryMax;
    private float spontaneityMax;

    // Use this for initialization
    void Start () {
       aggressionBar = ((aggressionHUD.GetComponentsInChildren<CanvasRenderer>()[2]).transform as RectTransform);
       energyBar = energyHUD.GetComponentsInChildren<CanvasRenderer>()[2].transform as RectTransform;
       braveryBar = braveryHUD.GetComponentsInChildren<CanvasRenderer>()[2].transform as RectTransform;
       spontaneityBar = spontaneityHUD.GetComponentsInChildren<CanvasRenderer>()[2].transform as RectTransform;
       aggressionMax = aggressionBar.rect.width;
       energyMax = energyBar.rect.width;
       braveryMax = braveryBar.rect.width;
       spontaneityMax = spontaneityBar.rect.width;

       aggressionBasePos = spontaneityBar.localPosition ;
       energyBasePos = energyBar.localPosition;
       braveryBasePos = braveryBar.localPosition;
       spontaneityBasePos = spontaneityBar.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
       aggressionBar.sizeDelta = new Vector2(aggressionMax * ((npc.Aggression + 1)/2), aggressionBar.rect.height);
       aggressionBar.localPosition = new Vector2(aggressionBasePos.x - (aggressionMax - (aggressionMax * ((npc.Aggression + 1) / 2)))/2, aggressionBasePos.y);
       energyBar.sizeDelta = new Vector2(energyMax * ((npc.Energy + 1)/2), energyBar.rect.height);
       energyBar.localPosition = new Vector2(energyBasePos.x - (energyMax - (energyMax * ((npc.Energy + 1) / 2))) / 2, energyBasePos.y);
       braveryBar.sizeDelta = new Vector2(braveryMax * ((npc.Closer + 1)/2), braveryBar.rect.height);
       braveryBar.localPosition = new Vector2(braveryBasePos.x - (braveryMax - (braveryMax * ((npc.Closer + 1) / 2))) / 2, braveryBasePos.y);
       spontaneityBar.sizeDelta = new Vector2(spontaneityMax * ((npc.Spontaneity + 1)/2), spontaneityBar.rect.height);
       spontaneityBar.localPosition = new Vector2(spontaneityBasePos.x - (spontaneityMax - (spontaneityMax * ((npc.Spontaneity + 1) / 2))) / 2, spontaneityBasePos.y);
    }
}
