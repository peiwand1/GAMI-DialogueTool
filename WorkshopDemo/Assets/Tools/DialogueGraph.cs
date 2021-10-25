using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";
    private MiniMap _miniMap;
    
    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolBar();
        GenerateMiniMap();
        GenerateBlackboard();
    }

    private void GenerateBlackboard()
    {
        var _blackboard = new Blackboard(_graphView);
        _blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
        
        _blackboard.editTextRequested = (blackboard1, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (_graphView.ExposedProperties.Any(x => x.PropertyName.ToLower().Equals(newValue.ToLower())))
            {
                EditorUtility.DisplayDialog("Error", "This property name already exists, pleas choose another one!",
                    "OK");
                return;
            }

            var propertyIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName.Equals(oldPropertyName));
            _graphView.ExposedProperties[propertyIndex].PropertyName = newValue;
            ((BlackboardField)element).text = newValue;
        };

        _blackboard.SetPosition(new Rect(10, 30, 200, 140));
        _graphView.Add(_blackboard);
        _graphView.Blackboard = _blackboard;
        _graphView.AddPropertySearchWindow(_graphView.EditorWindow);
    }

    private void GenerateMiniMap()
    {
        _miniMap = new MiniMap{ anchored = true };
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(position.width - 10, 30));
        _miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        _graphView.Add(_miniMap);
    }

    private void OnDisable()
    {
        rootVisualElement.Clear();
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolBar()
    {
        var toolbar = new Toolbar();
        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });
        toolbar.Add(new Button(() => _graphView.OpenSearchWindow()) { text = "New Node" });
        toolbar.Add(new Button(() => NewGraph()) { text = "New Graph" });

        rootVisualElement.Add(toolbar);
    }

    private void NewGraph()
    {
        OnDisable();
        OnEnable();
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name.", "OK");
            return;
        }
        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
        {
            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }
    }
}
