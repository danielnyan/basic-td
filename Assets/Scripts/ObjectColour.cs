using UnityEngine;

[ExecuteInEditMode]
public class ObjectColour : MonoBehaviour
{
    //Allows each object to be set a colour, and the changes can be previewed in Edit mode
    //This script is specific to this project, as the blocks would have a solid colour which
    //would not be changed mid-game.

    public Color Colour; //The desired colour for the block, exposed in the editor
    public bool previewChanges = true; //Shows whether changes in colour should be reflected in Edit Mode

    //Sets the object colour to the colour as desired when the game starts, in case "Preview Changes"
    //is turned off
    private void Awake()
    {
        //Creates a new variable tempMaterial which has the properties of the object's current material
        //After which, it sets the temporary material's colour to the desired colour
        Material tempMaterial = new Material(GetComponent<Renderer>().sharedMaterial)
        {
            color = Colour
        };
        GetComponent<Renderer>().sharedMaterial = tempMaterial;
    }

    private void Update()
    {
        //You may turn Preview Changes off because the GetComponent process can be quite expensive
        if (previewChanges)
        {
            Material tempMaterial = new Material(GetComponent<Renderer>().sharedMaterial)
            {
                color = Colour
            };
            GetComponent<Renderer>().sharedMaterial = tempMaterial;
        }
    }
}
