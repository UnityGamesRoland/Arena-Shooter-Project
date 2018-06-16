using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class UI_Layer
{
	public string layerName;
	[FoldoutGroup("Details", false)] public int layerIndex;
    [FoldoutGroup("Details", false)] public int parentLayerIndex;
    [FoldoutGroup("Details", false)] public GameObject layerObject;
    [FoldoutGroup("Details", false)] public UI_Element firstElement;
}

public class UI_Manager : MonoBehaviour
{
	public UI_Layer[] layers;

    public bool interactable;
    public bool isKeybinding;
    [HideInInspector] public UI_Layer selectedLayer;
	[HideInInspector] public UI_Element selectedElement;

    private float executionInputHoldTimer;
    private float executionInputTimer;
    private bool controlsGamePause = false;

    public PauseManager pause;
    private UI_SceneManager sceneManager;

	#region Singleton And References
	public static UI_Manager Instance {get; private set;}
	private void Awake()
	{
		if(Instance == null) Instance = this;

        foreach (UI_Layer layer in layers) layer.layerObject.SetActive(false);
        MoveToLayer(0);
    }
	#endregion

	private void Start()
	{
		//Check if the UI manager should control the pause manager as well.
		pause = PauseManager.Instance;
        sceneManager = UI_SceneManager.Instance;

		if (pause != null) controlsGamePause = true;
        if (!controlsGamePause) interactable = true;
	}

	private void Update()
	{
        //Return if we are initializing or loading a level.
        if (!sceneManager.isInitialized || sceneManager.isLoading) return;

        //Check if the keybinder mode is active.
        if(isKeybinding)
        {
            //Make sure that the selected element is a keybinder element.
            if(selectedElement.elementType != UI_Element_Type.keybinderElement)
            {
                isKeybinding = false;
                return;
            }

            //Execute the keybinder event on the selected element.
            selectedElement.KeybinderEvent.Invoke();
            return;
        }

        //First check for navigation input, then execution input.
        GetNavigationInput();
        GetExecutionInput();
    }

    private void GetNavigationInput()
	{
		//Moving to next layer: Below <selectedLayer>
		if(Input.GetKeyDown(KeyCode.Escape)) HandleEscapeInput();

		//Check if we can interact with the UI.
		if(!interactable) return;

		//Moving to next UI element: Below <selectedElement>
		if(Input.GetKeyDown(KeyCode.DownArrow)) SetSelectedElement(false);

		//Moving to next UI element: Above <selectedElement>
		if(Input.GetKeyDown(KeyCode.UpArrow)) SetSelectedElement(true);
	}

	private void GetExecutionInput()
	{
        //Check if we can interact with the UI.
        if (!interactable)
        {
            executionInputHoldTimer = 0;
            return;
        }

		if(selectedElement.elementType == UI_Element_Type.buttonElement)
		{
			//Execute UI element: Single Channel Click Event.
			if(Input.GetKeyDown(KeyCode.Return)) selectedElement.ClickEvent.Invoke();
		}

		else if (selectedElement.elementType == UI_Element_Type.selectorElement)
        {
			//Execute UI element: Negative (Left) Direction Event.
			if(Input.GetKeyDown(KeyCode.LeftArrow)) selectedElement.NegativeEvent.Invoke();

			//Execute UI element: Positive (Right) Direction Event.
			if(Input.GetKeyDown(KeyCode.RightArrow)) selectedElement.PositiveEvent.Invoke();
		}

        else if (selectedElement.elementType == UI_Element_Type.sliderElement)
        {
            //Check if the execution input keys are released.
            if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                //Reset the hold timer and wait for the next frame.
                executionInputHoldTimer = 0f;
                return;
            }

            //Execute UI element: Negative (Left) Direction Event.
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //Execute the event once and start counting the hold timer.
                if (executionInputHoldTimer == 0) selectedElement.NegativeEvent.Invoke();
                executionInputHoldTimer += Time.unscaledDeltaTime;

                //Switch to continuous execution mode.
                if (executionInputHoldTimer > 0.3f)
                {
                    if(Time.realtimeSinceStartup > executionInputTimer)
                    {
                        executionInputTimer = Time.realtimeSinceStartup + 0.03f;
                        selectedElement.NegativeEvent.Invoke();
                    }
                }
            }

            //Execute UI element: Positive (Right) Direction Event.
            else if(Input.GetKey(KeyCode.RightArrow))
            {
                //Execute the event once and start counting the hold timer.
                if(executionInputHoldTimer == 0) selectedElement.PositiveEvent.Invoke();
                executionInputHoldTimer += Time.unscaledDeltaTime;

                //Switch to continuous execution mode.
                if (executionInputHoldTimer > 0.3f)
                {
                    if (Time.realtimeSinceStartup > executionInputTimer)
                    {
                        executionInputTimer = Time.realtimeSinceStartup + 0.03f;
                        selectedElement.PositiveEvent.Invoke();
                    }
                }
            }
        }

        else if (selectedElement.elementType == UI_Element_Type.keybinderElement)
        {
            //Execute UI element: Single Channel Click Event.
            if (Input.GetKeyDown(KeyCode.Return) && !isKeybinding) selectedElement.ClickEvent.Invoke();
        }
    }

	private void HandleEscapeInput()
	{
		//Get the selected layer's index.
		int selectedLayerIndex = GetSelectedLayerIndex();

		//Check if the UI manager controls the pause manager.
		if(controlsGamePause)
		{
			//Check if the game is paused.
			if(pause.isPaused)
			{
				//If the selected layer is not the base layer, descend one layer.
				if(selectedLayerIndex > 0)
				{
					MoveToPreviousLayer();
					return;
				}

				//If the selected layer is the base layer, continue the game.
				else if(selectedLayerIndex == 0)
				{
					pause.ContinueGame();
					return;
				}
			}

			//Check if the game is not paused.
			else
			{
				//Prevent game from being paused after player died.
				if(PlayerManager.Instance.isDead) return;

				//Pause the game and move to base layer.
				pause.PauseGame();
				MoveToLayer(0);
			}
		}

		//The UI manager doesn't control the pause manager.
		else
		{
			//If the selected layer is not the base layer, descend one layer.
			if(selectedLayerIndex > 0)
			{
				MoveToPreviousLayer();
				return;
			}
		}
	}

	public void MoveToLayer(int index)
	{
		//Check if there is a layer selected.
		if(selectedLayer.layerName != "")
		{
			//Disable the current layer's element holder object.
			selectedElement.outlineObject.SetActive(false);
			selectedLayer.layerObject.SetActive(false);
		}

		//Loop through the layer array.
		for(int i = 0; i < layers.Length; i++)
		{
			//Check if the layer index is matching.
			if(layers[i].layerIndex == index)
			{
				//Update the selected layer and its corresponding elements.
				selectedLayer = layers[i];
				selectedLayer.layerObject.SetActive(true);
				selectedElement = layers[i].firstElement;
				selectedElement.outlineObject.SetActive(true);
				break;
			}
		}

        //Reset the hold timer.
        executionInputHoldTimer = 0f;
    }

	public void MoveToPreviousLayer()
	{
		//Check if there is a layer selected.
		if(selectedLayer.layerName != "")
		{
			//Currently selected layer is the base layer. Return.
			if(selectedLayer.parentLayerIndex == -1) return;

			//Get the target layer weight.
			int targetLayerIndex = selectedLayer.parentLayerIndex;

			//Loop through the layer array.
			for(int i = 0; i < layers.Length; i++)
			{
				//Check if the layer index is matching.
				if(layers[i].layerIndex == targetLayerIndex)
				{
					//Update the selected layer and its corresponding elements.
					MoveToLayer(i);
					break;
				}
			}
		}

		//No layer selected. Returning to base layer.
		else MoveToLayer(0);
	}

	public int GetSelectedLayerIndex()
	{
		//Store the final outcome of the method.
		int index = -1;

		//Check if there is a selected layer and get it's index.
		if(selectedLayer.layerName != "") index = selectedLayer.layerIndex;

		//Return the layer index.
		return index;
	}

	private void SetSelectedElement(bool upMovement)
	{
		//Check if there is a selected element that we can start from.
		if(selectedElement != null)
		{
			//Get the next UI element based on the given direction.
			UI_Element nextElement = upMovement ? selectedElement.elementAbove : selectedElement.elementBelow;

			//Check if there is a new element.
			if(nextElement != null)
			{
				//Disable the currently selected element's outline object, update the selected element and enable it's outline object in this exact order.
				selectedElement.outlineObject.SetActive(false);
				selectedElement = nextElement;
				selectedElement.outlineObject.SetActive(true);
			}

            //Reset the hold timer.
            executionInputHoldTimer = 0f;
        }

		//Selected element not specified.
		else
		{
			//Selected layer not specified. Returning to base layer.
			if(selectedLayer == null) MoveToLayer(0);

			//Update the selected element and it's outline object.
			selectedElement = selectedLayer.firstElement;
			selectedElement.outlineObject.SetActive(true);

            //Reset the hold timer.
            executionInputHoldTimer = 0f;
        }
	}

	public void SetSelectedElement(UI_Element newElement)
	{
		//Disable the currently selected element's outline object.
		if(selectedElement != null) selectedElement.outlineObject.SetActive(false);

		//Update the selected element and it's outline object.
		selectedElement = newElement;
		selectedElement.outlineObject.SetActive(true);

        //Reset the hold timer.
        executionInputHoldTimer = 0f;
	}
}
