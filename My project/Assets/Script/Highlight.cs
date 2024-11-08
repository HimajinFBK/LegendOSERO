
using UnityEngine;

public class Highlight : MonoBehaviour
{

    [SerializeField]
    private Color normaLColor;

    [SerializeField]
    private Color mouseOverColor;


    private Material material;
    // Start is called before the first frame update
    private void Start()
    {
        material= GetComponent<MeshRenderer>().material;
        material.color=normaLColor;
    }

    private void OnMouseEnter()
    {
        material.color= mouseOverColor;
    }

    private void OnMouseExit()
    {
        material.color=normaLColor;
    }

    private void OnDestroy()
    {
      Destroy(material);
    }

}
